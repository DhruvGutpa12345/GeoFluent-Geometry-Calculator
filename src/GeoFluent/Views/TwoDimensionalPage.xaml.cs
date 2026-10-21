using System.Globalization;
using GeoFluent.Math;
using GeoFluent.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

namespace GeoFluent.Views;

public sealed partial class TwoDimensionalPage : Page
{
    private ShapeDefinition _activeShape = Formulas2D.All[0];
    private MethodDefinition _activeMethod = Formulas2D.All[0].Methods[0];
    private readonly Dictionary<string, (TextBox InputBox, ComboBox UnitBox, InputDefinition Def)> _inputs = [];
    private ShapeCalculationResult? _latestResult;
    private bool _isBuildingUi;

    public TwoDimensionalPage()
    {
        InitializeComponent();
        Loaded += TwoDimensionalPage_Loaded;
    }

    private void TwoDimensionalPage_Loaded(object sender, RoutedEventArgs e)
    {
        PopulateOutputUnitPickers();
        PopulateShapes();

        // Check if there is a pending restore from history
        if (HistoryService.PendingRestore != null && HistoryService.PendingRestore.Category == "2D")
        {
            RestoreFromHistory(HistoryService.PendingRestore);
            HistoryService.PendingRestore = null;
        }
    }

    private void PopulateOutputUnitPickers()
    {
        _isBuildingUi = true;
        CmbOutputLengthUnit.Items.Clear();
        foreach (var u in UnitConverter.LengthUnits)
        {
            CmbOutputLengthUnit.Items.Add(new ComboBoxItem { Content = u.Symbol, Tag = u.Id });
        }
        CmbOutputLengthUnit.SelectedIndex = 1; // "cm"

        CmbOutputAreaUnit.Items.Clear();
        foreach (var u in UnitConverter.AreaUnits)
        {
            CmbOutputAreaUnit.Items.Add(new ComboBoxItem { Content = u.Symbol, Tag = u.Id });
        }
        CmbOutputAreaUnit.SelectedIndex = 1; // "cm²"
        _isBuildingUi = false;
    }

    private void PopulateShapes(string filter = "")
    {
        _isBuildingUi = true;
        CmbShape.Items.Clear();

        var filtered = string.IsNullOrWhiteSpace(filter)
            ? Formulas2D.All
            : Formulas2D.All.Where(s => s.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                       s.Description.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();

        if (filtered.Count == 0) filtered = Formulas2D.All;

        foreach (var s in filtered)
        {
            CmbShape.Items.Add(new ComboBoxItem { Content = s.Name, Tag = s.Id });
        }

        CmbShape.SelectedIndex = 0;
        _activeShape = filtered[0];
        _isBuildingUi = false;
        PopulateMethods();
    }

    private void TxtSearchShape_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isBuildingUi)
        {
            PopulateShapes(TxtSearchShape.Text);
        }
    }

    private void CmbShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isBuildingUi || CmbShape.SelectedItem is not ComboBoxItem item) return;
        string? id = item.Tag?.ToString();
        var found = Formulas2D.All.FirstOrDefault(s => s.Id == id);
        if (found != null)
        {
            _activeShape = found;
            TxtHeaderTitle.Text = found.Name;
            TxtHeaderDesc.Text = found.Description;
            PopulateMethods();
        }
    }

    private void PopulateMethods()
    {
        _isBuildingUi = true;
        CmbMethod.Items.Clear();

        foreach (var m in _activeShape.Methods)
        {
            CmbMethod.Items.Add(new ComboBoxItem { Content = m.Name, Tag = m.Id });
        }

        CmbMethod.SelectedIndex = 0;
        _activeMethod = _activeShape.Methods[0];
        TxtMethodDesc.Text = _activeMethod.Description;
        MethodCard.Visibility = _activeShape.Methods.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

        _isBuildingUi = false;
        BuildInputFields();
    }

    private void CmbMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isBuildingUi || CmbMethod.SelectedItem is not ComboBoxItem item) return;
        string? id = item.Tag?.ToString();
        var found = _activeShape.Methods.FirstOrDefault(m => m.Id == id);
        if (found != null)
        {
            _activeMethod = found;
            TxtMethodDesc.Text = found.Description;
            BuildInputFields();
        }
    }

    private void BuildInputFields()
    {
        _isBuildingUi = true;
        InputsPanel.Children.Clear();
        _inputs.Clear();
        ErrorBar.IsOpen = false;
        ToastBar.IsOpen = false;

        foreach (var def in _activeMethod.Inputs)
        {
            Grid row = new() { Margin = new Thickness(0, 2, 0, 2) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });

            TextBlock lbl = new()
            {
                Text = def.Label,
                Style = (Style)Application.Current.Resources["BodyTextBlockStyle"],
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(lbl, 0);

            TextBox txt = new()
            {
                Text = def.DefaultValue.ToString(CultureInfo.InvariantCulture),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };
            txt.TextChanged += (s, e) =>
            {
                if (!_isBuildingUi && ChkAutoCalc.IsChecked == true)
                    Calculate();
            };
            txt.KeyDown += (s, e) =>
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                    Calculate();
            };
            Grid.SetColumn(txt, 1);

            ComboBox unitCombo = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(8, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            if (def.UnitType == UnitType.Count)
            {
                unitCombo.Items.Add(new ComboBoxItem { Content = "count", Tag = "count" });
                unitCombo.SelectedIndex = 0;
                unitCombo.IsEnabled = false;
            }
            else
            {
                var units = UnitConverter.GetUnits(def.UnitType);
                int selIdx = 0;
                for (int i = 0; i < units.Length; i++)
                {
                    unitCombo.Items.Add(new ComboBoxItem { Content = units[i].Symbol, Tag = units[i].Id });
                    if (def.UnitType == UnitType.Length && units[i].Id == "cm") selIdx = i;
                    if (def.UnitType == UnitType.Angle && units[i].Id == "deg") selIdx = i;
                }
                unitCombo.SelectedIndex = selIdx;
                unitCombo.SelectionChanged += (s, e) =>
                {
                    if (!_isBuildingUi && ChkAutoCalc.IsChecked == true)
                        Calculate();
                };
            }
            Grid.SetColumn(unitCombo, 2);

            row.Children.Add(lbl);
            row.Children.Add(txt);
            row.Children.Add(unitCombo);
            InputsPanel.Children.Add(row);

            _inputs[def.Id] = (txt, unitCombo, def);
        }

        _isBuildingUi = false;
        Calculate();
    }

    private void BtnCalculate_Click(object sender, RoutedEventArgs e) => Calculate();

    private void Calculate()
    {
        try
        {
            ErrorBar.IsOpen = false;
            Dictionary<string, double> baseVals = [];

            foreach (var kvp in _inputs)
            {
                var (txt, combo, def) = kvp.Value;
                if (!double.TryParse(txt.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                {
                    throw new FormatException($"Invalid numerical format for '{def.Label}'.");
                }
                if (val < def.Min || val > def.Max)
                {
                    throw new ArgumentOutOfRangeException(def.Id, $"Value for '{def.Label}' must be between {def.Min} and {def.Max}.");
                }

                string unitId = (combo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
                double baseVal = UnitConverter.ToBase(val, unitId, def.UnitType);
                baseVals[kvp.Key] = baseVal;
            }

            _latestResult = _activeMethod.Calculate(baseVals);
            RenderOutputs(_latestResult);
            RenderSteps(_latestResult);
            RenderCanvas(_latestResult);

            ResultsCard.Visibility = Visibility.Visible;
            StepsCard.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            ErrorBar.Message = ex.Message;
            ErrorBar.IsOpen = true;
            ResultsCard.Visibility = Visibility.Collapsed;
            StepsCard.Visibility = Visibility.Collapsed;
        }
    }

    private void RenderOutputs(ShapeCalculationResult result)
    {
        OutputsPanel.Children.Clear();
        string lenUnit = (CmbOutputLengthUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm";
        string areaUnit = (CmbOutputAreaUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm2";
        int prec = UserSettingsService.Precision;

        foreach (var outP in result.Outputs)
        {
            string targetUnit = outP.UnitType switch
            {
                UnitType.Area => areaUnit,
                UnitType.Length => lenUnit,
                UnitType.Angle => "deg",
                _ => ""
            };

            string formatted = outP.Format(targetUnit, prec);

            Grid row = new() { Margin = new Thickness(0, 3, 0, 3) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(220) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock lbl = new()
            {
                Text = outP.Label,
                Style = (Style)Application.Current.Resources["BodyTextBlockStyle"],
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"],
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(lbl, 0);

            TextBlock val = new()
            {
                Text = formatted,
                Style = outP.IsPrimary ? (Style)Application.Current.Resources["SubtitleTextBlockStyle"] : (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                Foreground = outP.IsPrimary ? (Brush)Application.Current.Resources["AccentFillColorDefaultBrush"] : (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"],
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(val, 1);

            Border badge = new()
            {
                CornerRadius = new CornerRadius(4),
                Background = (Brush)Application.Current.Resources["SubtleFillColorSecondaryBrush"],
                Padding = new Thickness(8, 2, 8, 2),
                Margin = new Thickness(8, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock { Text = outP.Symbol, Style = (Style)Application.Current.Resources["CaptionTextBlockStyle"] }
            };
            Grid.SetColumn(badge, 2);

            Button copyBtn = new()
            {
                Content = new FontIcon { Glyph = "\uE8C8", FontSize = 12 },
                Padding = new Thickness(6, 4, 6, 4),
                VerticalAlignment = VerticalAlignment.Center
            };
            copyBtn.Click += (s, e) =>
            {
                var pkg = new DataPackage();
                pkg.SetText(formatted);
                Clipboard.SetContent(pkg);
                ShowToast($"Copied '{outP.Label}' result: {formatted}");
            };
            Grid.SetColumn(copyBtn, 3);

            row.Children.Add(lbl);
            row.Children.Add(val);
            row.Children.Add(badge);
            row.Children.Add(copyBtn);
            OutputsPanel.Children.Add(row);
        }
    }

    private void RenderSteps(ShapeCalculationResult result)
    {
        StepsPanel.Children.Clear();
        for (int i = 0; i < result.Steps.Count; i++)
        {
            var step = result.Steps[i];
            StackPanel card = new() { Spacing = 4, Margin = new Thickness(0, 4, 0, 6) };

            TextBlock title = new()
            {
                Text = $"{i + 1}. {step.Title}",
                Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                Foreground = (Brush)Application.Current.Resources["AccentFillColorDefaultBrush"]
            };

            TextBlock formula = new()
            {
                Text = $"Formula: {step.Formula}",
                FontFamily = new FontFamily("Consolas, Cascadia Code"),
                Style = (Style)Application.Current.Resources["CaptionTextBlockStyle"],
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            };

            TextBlock subst = new()
            {
                Text = $"Substitution: {step.Substitution}",
                FontFamily = new FontFamily("Consolas, Cascadia Code"),
                Style = (Style)Application.Current.Resources["BodyTextBlockStyle"]
            };

            card.Children.Add(title);
            card.Children.Add(formula);
            card.Children.Add(subst);
            StepsPanel.Children.Add(card);
        }
    }

    private void RenderCanvas(ShapeCalculationResult result)
    {
        GeometryCanvas.Children.Clear();
        var points = result.PolygonPoints;
        if (points == null || points.Count < 3)
        {
            TxtCanvasInfo.Text = "No vector polygon available for this mode.";
            return;
        }

        double minX = points.Min(p => p.X);
        double maxX = points.Max(p => p.X);
        double minY = points.Min(p => p.Y);
        double maxY = points.Max(p => p.Y);

        double w = maxX - minX;
        double h = maxY - minY;
        if (w <= 0 || h <= 0) return;

        double canvasW = GeometryCanvas.Width;
        double canvasH = GeometryCanvas.Height;
        double padding = 30;

        double scale = System.Math.Min((canvasW - padding * 2) / w, (canvasH - padding * 2) / h);
        double offsetX = padding + (canvasW - padding * 2 - w * scale) / 2.0 - minX * scale;
        double offsetY = canvasH - (padding + (canvasH - padding * 2 - h * scale) / 2.0) + minY * scale;

        // Inscribed / Circumscribed guide circles
        if (ChkShowGuides.IsChecked == true)
        {
            double cx = points.Average(p => p.X) * scale + offsetX;
            double cy = offsetY - points.Average(p => p.Y) * scale;

            if (result.Circumradius > 0)
            {
                double rC = result.Circumradius * scale;
                Ellipse circCircle = new()
                {
                    Width = rC * 2,
                    Height = rC * 2,
                    Stroke = new SolidColorBrush(ColorHelper.FromArgb(180, 167, 139, 250)),
                    StrokeThickness = 1.5,
                    StrokeDashArray = [4, 4]
                };
                Canvas.SetLeft(circCircle, cx - rC);
                Canvas.SetTop(circCircle, cy - rC);
                GeometryCanvas.Children.Add(circCircle);
            }

            if (result.Inradius > 0)
            {
                double rIn = result.Inradius * scale;
                Ellipse inCircle = new()
                {
                    Width = rIn * 2,
                    Height = rIn * 2,
                    Stroke = new SolidColorBrush(ColorHelper.FromArgb(200, 52, 211, 153)),
                    StrokeThickness = 1.5,
                    StrokeDashArray = [3, 3]
                };
                Canvas.SetLeft(inCircle, cx - rIn);
                Canvas.SetTop(inCircle, cy - rIn);
                GeometryCanvas.Children.Add(inCircle);
            }
        }

        // Polygon
        Polygon polygon = new()
        {
            Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 0, 120, 215)),
            StrokeThickness = 2.5,
            Fill = new SolidColorBrush(ColorHelper.FromArgb(40, 96, 205, 255))
        };

        foreach (var p in points)
        {
            double sx = p.X * scale + offsetX;
            double sy = offsetY - p.Y * scale;
            polygon.Points.Add(new Point(sx, sy));

            // Vertex marker
            Ellipse dot = new()
            {
                Width = 6,
                Height = 6,
                Fill = new SolidColorBrush(Colors.White),
                Stroke = new SolidColorBrush(Colors.DodgerBlue),
                StrokeThickness = 1.5
            };
            Canvas.SetLeft(dot, sx - 3);
            Canvas.SetTop(dot, sy - 3);
            GeometryCanvas.Children.Add(dot);
        }

        GeometryCanvas.Children.Add(polygon);
        TxtCanvasInfo.Text = $"{_activeShape.Name} ({points.Count} vertices, scale: {scale:F1}x)";
    }

    private void ChkShowGuides_Toggled(object sender, RoutedEventArgs e)
    {
        if (_latestResult != null) RenderCanvas(_latestResult);
    }

    private void CmbOutputUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isBuildingUi && _latestResult != null)
            RenderOutputs(_latestResult);
    }

    private void BtnCopyReport_Click(object sender, RoutedEventArgs e)
    {
        if (_latestResult == null) return;
        var pkg = new DataPackage();
        pkg.SetText(GenerateMarkdownReport());
        Clipboard.SetContent(pkg);
        ShowToast("Full Markdown geometry report copied to clipboard!");
    }

    private void BtnSaveHistory_Click(object sender, RoutedEventArgs e)
    {
        if (_latestResult == null) return;

        List<SavedInputRecord> inps = [];
        foreach (var kvp in _inputs)
        {
            var (txt, combo, def) = kvp.Value;
            double.TryParse(txt.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double v);
            string uId = (combo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
            string uSym = (combo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            inps.Add(new(kvp.Key, def.Label, v, uId, uSym));
        }

        List<SavedOutputRecord> outs = [];
        string lenUnit = (CmbOutputLengthUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm";
        string areaUnit = (CmbOutputAreaUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm2";

        foreach (var o in _latestResult.Outputs)
        {
            string tu = o.UnitType switch { UnitType.Area => areaUnit, UnitType.Length => lenUnit, UnitType.Angle => "deg", _ => "" };
            outs.Add(new(o.Label, o.Format(tu, UserSettingsService.Precision), o.Symbol));
        }

        HistoryService.Add(_activeShape.Id, _activeShape.Name, "2D", _activeMethod.Id, _activeMethod.Name, inps, outs, _latestResult.Steps);
        ShowToast($"Calculation for {_activeShape.Name} saved to history!");
    }

    private string GenerateMarkdownReport()
    {
        if (_latestResult == null) return "";
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"# GeoFluent 2D Report: {_activeShape.Name}");
        sb.AppendLine($"**Method**: {_activeMethod.Name}");
        sb.AppendLine($"**Date**: {DateTime.Now:F}");
        sb.AppendLine();
        sb.AppendLine("## Inputs");
        sb.AppendLine("| Parameter | Value | Unit |");
        sb.AppendLine("|---|---|---|");
        foreach (var kvp in _inputs)
        {
            var (txt, combo, def) = kvp.Value;
            string uSym = (combo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            sb.AppendLine($"| {def.Label} | {txt.Text} | {uSym} |");
        }
        sb.AppendLine();
        sb.AppendLine("## Outputs");
        sb.AppendLine("| Property | Value | Symbol |");
        sb.AppendLine("|---|---|---|");
        string lenUnit = (CmbOutputLengthUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm";
        string areaUnit = (CmbOutputAreaUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm2";
        foreach (var o in _latestResult.Outputs)
        {
            string tu = o.UnitType switch { UnitType.Area => areaUnit, UnitType.Length => lenUnit, UnitType.Angle => "deg", _ => "" };
            sb.AppendLine($"| {o.Label} | {o.Format(tu, UserSettingsService.Precision)} | {o.Symbol} |");
        }
        sb.AppendLine();
        sb.AppendLine("## Derivations");
        for (int i = 0; i < _latestResult.Steps.Count; i++)
        {
            var s = _latestResult.Steps[i];
            sb.AppendLine($"### {i + 1}. {s.Title}");
            sb.AppendLine($"- Formula: `{s.Formula}`");
            sb.AppendLine($"- Substitution: {s.Substitution}");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private void RestoreFromHistory(FullHistoryEntry entry)
    {
        var shape = Formulas2D.All.FirstOrDefault(s => s.Id == entry.ShapeId);
        if (shape == null) return;

        _activeShape = shape;
        TxtHeaderTitle.Text = shape.Name;
        TxtHeaderDesc.Text = shape.Description;

        // Select shape combo
        for (int i = 0; i < CmbShape.Items.Count; i++)
        {
            if ((CmbShape.Items[i] as ComboBoxItem)?.Tag?.ToString() == shape.Id)
            {
                CmbShape.SelectedIndex = i;
                break;
            }
        }

        PopulateMethods();

        var method = shape.Methods.FirstOrDefault(m => m.Id == entry.MethodId);
        if (method != null)
        {
            _activeMethod = method;
            for (int i = 0; i < CmbMethod.Items.Count; i++)
            {
                if ((CmbMethod.Items[i] as ComboBoxItem)?.Tag?.ToString() == method.Id)
                {
                    CmbMethod.SelectedIndex = i;
                    break;
                }
            }
            BuildInputFields();
        }

        // Restore input values and units
        _isBuildingUi = true;
        foreach (var savedInp in entry.Inputs)
        {
            if (_inputs.TryGetValue(savedInp.Key, out var tuple))
            {
                tuple.InputBox.Text = savedInp.Value.ToString(CultureInfo.InvariantCulture);
                for (int u = 0; u < tuple.UnitBox.Items.Count; u++)
                {
                    if ((tuple.UnitBox.Items[u] as ComboBoxItem)?.Tag?.ToString() == savedInp.UnitId)
                    {
                        tuple.UnitBox.SelectedIndex = u;
                        break;
                    }
                }
            }
        }
        _isBuildingUi = false;
        Calculate();
        ShowToast($"Restored '{shape.Name}' calculation from history.");
    }

    private void ShowToast(string message)
    {
        ToastBar.Message = message;
        ToastBar.IsOpen = true;
    }
}

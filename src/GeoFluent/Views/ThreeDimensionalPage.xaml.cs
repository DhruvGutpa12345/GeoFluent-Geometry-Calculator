using System.Globalization;
using GeoFluent.Math;
using GeoFluent.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.DataTransfer;

namespace GeoFluent.Views;

public sealed partial class ThreeDimensionalPage : Page
{
    private ShapeDefinition _activeShape = Formulas3D.All[0];
    private MethodDefinition _activeMethod = Formulas3D.All[0].Methods[0];
    private readonly Dictionary<string, (TextBox InputBox, ComboBox UnitBox, InputDefinition Def)> _inputs = [];
    private ShapeCalculationResult? _latestResult;
    private bool _isBuildingUi;
    private readonly DispatcherTimer _rotateTimer = new();

    public ThreeDimensionalPage()
    {
        InitializeComponent();
        _rotateTimer.Interval = TimeSpan.FromMilliseconds(33); // ~30 fps
        _rotateTimer.Tick += (s, e) =>
        {
            if (ChkAutoRotate.IsChecked == true)
            {
                double nextYaw = (SliderYaw.Value + 1.0) % 360.0;
                SliderYaw.Value = nextYaw;
            }
        };

        Loaded += ThreeDimensionalPage_Loaded;
        Unloaded += (s, e) => _rotateTimer.Stop();
    }

    private void ThreeDimensionalPage_Loaded(object sender, RoutedEventArgs e)
    {
        PopulateOutputUnitPickers();
        PopulateShapes();
        _rotateTimer.Start();

        // Check if there is a pending restore from history
        if (HistoryService.PendingRestore != null && HistoryService.PendingRestore.Category == "3D")
        {
            RestoreFromHistory(HistoryService.PendingRestore);
            HistoryService.PendingRestore = null;
        }
    }

    private void PopulateOutputUnitPickers()
    {
        _isBuildingUi = true;
        CmbOutputVolumeUnit.Items.Clear();
        foreach (var u in UnitConverter.VolumeUnits)
        {
            CmbOutputVolumeUnit.Items.Add(new ComboBoxItem { Content = u.Symbol, Tag = u.Id });
        }
        CmbOutputVolumeUnit.SelectedIndex = 1; // "cm³"

        CmbOutputAreaUnit.Items.Clear();
        foreach (var u in UnitConverter.AreaUnits)
        {
            CmbOutputAreaUnit.Items.Add(new ComboBoxItem { Content = u.Symbol, Tag = u.Id });
        }
        CmbOutputAreaUnit.SelectedIndex = 1; // "cm²"

        CmbOutputLengthUnit.Items.Clear();
        foreach (var u in UnitConverter.LengthUnits)
        {
            CmbOutputLengthUnit.Items.Add(new ComboBoxItem { Content = u.Symbol, Tag = u.Id });
        }
        CmbOutputLengthUnit.SelectedIndex = 1; // "cm"
        _isBuildingUi = false;
    }

    private void PopulateShapes(string filter = "")
    {
        _isBuildingUi = true;
        CmbShape.Items.Clear();

        var filtered = string.IsNullOrWhiteSpace(filter)
            ? Formulas3D.All
            : Formulas3D.All.Where(s => s.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                       s.Description.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();

        if (filtered.Count == 0) filtered = Formulas3D.All;

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
        var found = Formulas3D.All.FirstOrDefault(s => s.Id == id);
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

            var units = UnitConverter.GetUnits(def.UnitType);
            int selIdx = 0;
            for (int i = 0; i < units.Length; i++)
            {
                unitCombo.Items.Add(new ComboBoxItem { Content = units[i].Symbol, Tag = units[i].Id });
                if (def.UnitType == UnitType.Length && units[i].Id == "cm") selIdx = i;
                if (def.UnitType == UnitType.Volume && units[i].Id == "cm3") selIdx = i;
            }
            unitCombo.SelectedIndex = selIdx;
            unitCombo.SelectionChanged += (s, e) =>
            {
                if (!_isBuildingUi && ChkAutoCalc.IsChecked == true)
                    Calculate();
            };
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
            Render3DVisualizer(_latestResult);

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
        string volUnit = (CmbOutputVolumeUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm3";
        string areaUnit = (CmbOutputAreaUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm2";
        string lenUnit = (CmbOutputLengthUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm";
        int prec = UserSettingsService.Precision;

        foreach (var outP in result.Outputs)
        {
            string targetUnit = outP.UnitType switch
            {
                UnitType.Volume => volUnit,
                UnitType.Area => areaUnit,
                UnitType.Length => lenUnit,
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

    private void Render3DVisualizer(ShapeCalculationResult result)
    {
        Geometry3DCanvas.Children.Clear();
        var lines = result.SolidLines;
        if (lines == null || lines.Count == 0) return;

        double yaw = (SliderYaw.Value * System.Math.PI) / 180.0;
        double pitch = (SliderPitch.Value * System.Math.PI) / 180.0;

        double cosY = System.Math.Cos(yaw), sinY = System.Math.Sin(yaw);
        double cosP = System.Math.Cos(pitch), sinP = System.Math.Sin(pitch);

        double canvasW = Geometry3DCanvas.Width;
        double canvasH = Geometry3DCanvas.Height;
        double cx = canvasW / 2.0;
        double cy = canvasH / 2.0;

        // Find bounding radius to compute scale
        double maxCoord = 0.001;
        foreach (var line in lines)
        {
            maxCoord = System.Math.Max(maxCoord, System.Math.Abs(line.Start.X));
            maxCoord = System.Math.Max(maxCoord, System.Math.Abs(line.Start.Y));
            maxCoord = System.Math.Max(maxCoord, System.Math.Abs(line.Start.Z));
            maxCoord = System.Math.Max(maxCoord, System.Math.Abs(line.End.X));
            maxCoord = System.Math.Max(maxCoord, System.Math.Abs(line.End.Y));
            maxCoord = System.Math.Max(maxCoord, System.Math.Abs(line.End.Z));
        }

        double scale = (System.Math.Min(canvasW, canvasH) * 0.38) / maxCoord;
        const double cameraDist = 4.0;

        foreach (var l in lines)
        {
            // Transform Start Point
            double x1 = l.Start.X * cosY + l.Start.Z * sinY;
            double z1 = -l.Start.X * sinY + l.Start.Z * cosY;
            double y1 = l.Start.Y * cosP - z1 * sinP;
            double zFinal1 = l.Start.Y * sinP + z1 * cosP;

            double persp1 = cameraDist / (cameraDist + (zFinal1 / maxCoord));
            double sx1 = cx + x1 * scale * persp1;
            double sy1 = cy - y1 * scale * persp1;

            // Transform End Point
            double x2 = l.End.X * cosY + l.End.Z * sinY;
            double z2 = -l.End.X * sinY + l.End.Z * cosY;
            double y2 = l.End.Y * cosP - z2 * sinP;
            double zFinal2 = l.End.Y * sinP + z2 * cosP;

            double persp2 = cameraDist / (cameraDist + (zFinal2 / maxCoord));
            double sx2 = cx + x2 * scale * persp2;
            double sy2 = cy - y2 * scale * persp2;

            // Depth coloring
            double avgZ = (zFinal1 + zFinal2) / (2.0 * maxCoord);
            byte alpha = (byte)System.Math.Clamp(130 + (int)(avgZ * 90), 80, 255);

            Line lineElem = new()
            {
                X1 = sx1,
                Y1 = sy1,
                X2 = sx2,
                Y2 = sy2,
                Stroke = new SolidColorBrush(ColorHelper.FromArgb(alpha, 96, 205, 255)),
                StrokeThickness = avgZ > 0 ? 1.8 : 1.1
            };

            Geometry3DCanvas.Children.Add(lineElem);
        }
    }

    private void SliderRotation_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (_latestResult != null) Render3DVisualizer(_latestResult);
    }

    private void ChkAutoRotate_Toggled(object sender, RoutedEventArgs e)
    {
        if (ChkAutoRotate.IsChecked == true) _rotateTimer.Start();
        else _rotateTimer.Stop();
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
        ShowToast("Full Markdown 3D solid report copied to clipboard!");
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
        string volUnit = (CmbOutputVolumeUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm3";
        string areaUnit = (CmbOutputAreaUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm2";
        string lenUnit = (CmbOutputLengthUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm";

        foreach (var o in _latestResult.Outputs)
        {
            string tu = o.UnitType switch { UnitType.Volume => volUnit, UnitType.Area => areaUnit, UnitType.Length => lenUnit, _ => "" };
            outs.Add(new(o.Label, o.Format(tu, UserSettingsService.Precision), o.Symbol));
        }

        HistoryService.Add(_activeShape.Id, _activeShape.Name, "3D", _activeMethod.Id, _activeMethod.Name, inps, outs, _latestResult.Steps);
        ShowToast($"Calculation for {_activeShape.Name} saved to history!");
    }

    private string GenerateMarkdownReport()
    {
        if (_latestResult == null) return "";
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"# GeoFluent 3D Solid Report: {_activeShape.Name}");
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
        string volUnit = (CmbOutputVolumeUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm3";
        string areaUnit = (CmbOutputAreaUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm2";
        string lenUnit = (CmbOutputLengthUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "cm";
        foreach (var o in _latestResult.Outputs)
        {
            string tu = o.UnitType switch { UnitType.Volume => volUnit, UnitType.Area => areaUnit, UnitType.Length => lenUnit, _ => "" };
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
        var shape = Formulas3D.All.FirstOrDefault(s => s.Id == entry.ShapeId);
        if (shape == null) return;

        _activeShape = shape;
        TxtHeaderTitle.Text = shape.Name;
        TxtHeaderDesc.Text = shape.Description;

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

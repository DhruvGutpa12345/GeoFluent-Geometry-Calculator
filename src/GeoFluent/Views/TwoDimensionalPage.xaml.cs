using System.Globalization;
using GeoFluent.Math;
using GeoFluent.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;

namespace GeoFluent.Views;

public sealed partial class TwoDimensionalPage : Page
{
    private readonly Dictionary<string, TextBox> _inputBoxes = [];

    public TwoDimensionalPage()
    {
        InitializeComponent();
        Loaded += (s, e) => RebuildInputs();
    }

    private void CmbShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
        {
            RebuildInputs();
        }
    }

    private void RebuildInputs()
    {
        InputsPanel.Children.Clear();
        _inputBoxes.Clear();
        ErrorBar.IsOpen = false;
        ResultsCard.Visibility = Visibility.Collapsed;
        StepsCard.Visibility = Visibility.Collapsed;
        GeometryCanvas.Children.Clear();

        string tag = (CmbShape.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Triangle";

        switch (tag)
        {
            case "Triangle":
                AddInputField("Side a", "a", "3");
                AddInputField("Side b", "b", "4");
                AddInputField("Side c", "c", "5");
                break;
            case "Rectangle":
                AddInputField("Length (l)", "l", "8");
                AddInputField("Width (w)", "w", "5");
                break;
            case "Square":
                AddInputField("Side (a)", "a", "6");
                break;
            case "Circle":
                AddInputField("Radius (r)", "r", "5");
                break;
            case "Parallelogram":
                AddInputField("Base side (a)", "a", "10");
                AddInputField("Slant side (b)", "b", "6");
                AddInputField("Height (h)", "h", "5");
                break;
            case "Rhombus":
                AddInputField("Diagonal 1 (d₁)", "d1", "8");
                AddInputField("Diagonal 2 (d₂)", "d2", "6");
                break;
            case "Trapezoid":
                AddInputField("Base a", "a", "10");
                AddInputField("Base b", "b", "6");
                AddInputField("Height (h)", "h", "4");
                break;
            case "Polygon":
                AddInputField("Number of sides (n)", "n", "6");
                AddInputField("Side length (s)", "s", "4");
                break;
            case "Ellipse":
                AddInputField("Semi-major axis (a)", "a", "8");
                AddInputField("Semi-minor axis (b)", "b", "5");
                break;
        }

        // Trigger default calculation
        PerformCalculation();
    }

    private void AddInputField(string label, string key, string defaultValue)
    {
        StackPanel panel = new() { Spacing = 4 };
        TextBlock tb = new()
        {
            Text = label,
            Style = (Style)Application.Current.Resources["BodyTextBlockStyle"]
        };
        TextBox box = new()
        {
            Text = defaultValue,
            Width = 260,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        box.KeyDown += (s, e) =>
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                PerformCalculation();
            }
        };

        _inputBoxes[key] = box;
        panel.Children.Add(tb);
        panel.Children.Add(box);
        InputsPanel.Children.Add(panel);
    }

    private void BtnCalculate_Click(object sender, RoutedEventArgs e)
    {
        PerformCalculation();
    }

    private void PerformCalculation()
    {
        try
        {
            ErrorBar.IsOpen = false;
            string tag = (CmbShape.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Triangle";

            ShapeCalculationResult result;
            string shapeName = (CmbShape.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Shape";

            switch (tag)
            {
                case "Triangle":
                    double a = ReadVal("a");
                    double b = ReadVal("b");
                    double c = ReadVal("c");
                    result = Formulas2D.TriangleHeron(a, b, c);
                    break;
                case "Rectangle":
                    result = Formulas2D.Rectangle(ReadVal("l"), ReadVal("w"));
                    break;
                case "Square":
                    result = Formulas2D.Square(ReadVal("a"));
                    break;
                case "Circle":
                    result = Formulas2D.Circle(ReadVal("r"));
                    break;
                case "Parallelogram":
                    result = Formulas2D.Parallelogram(ReadVal("a"), ReadVal("b"), ReadVal("h"));
                    break;
                case "Rhombus":
                    result = Formulas2D.Rhombus(ReadVal("d1"), ReadVal("d2"));
                    break;
                case "Trapezoid":
                    result = Formulas2D.Trapezoid(ReadVal("a"), ReadVal("b"), ReadVal("h"));
                    break;
                case "Polygon":
                    int n = (int)System.Math.Round(ReadVal("n"));
                    result = Formulas2D.RegularPolygon(n, ReadVal("s"));
                    break;
                case "Ellipse":
                    result = Formulas2D.Ellipse(ReadVal("a"), ReadVal("b"));
                    break;
                default:
                    return;
            }

            OutputsList.ItemsSource = result.Outputs;
            StepsList.ItemsSource = result.Steps;
            ResultsCard.Visibility = Visibility.Visible;
            StepsCard.Visibility = Visibility.Visible;

            // Draw shape on canvas
            RenderCanvas(result.PolygonPoints);

            // Record into history
            var primary = result.Outputs.FirstOrDefault(o => o.IsPrimary);
            string primaryText = primary.Label != null ? $"{primary.Label}: {primary.DisplayValue}" : "";
            HistoryService.Add(shapeName, "2D", primaryText);
        }
        catch (Exception ex)
        {
            ErrorBar.Message = ex.Message;
            ErrorBar.IsOpen = true;
            ResultsCard.Visibility = Visibility.Collapsed;
            StepsCard.Visibility = Visibility.Collapsed;
        }
    }

    private double ReadVal(string key)
    {
        if (_inputBoxes.TryGetValue(key, out var tb) &&
            double.TryParse(tb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
        {
            return val;
        }
        throw new FormatException($"Invalid numerical value for parameter '{key}'.");
    }

    private void RenderCanvas(IReadOnlyList<Point2D>? points)
    {
        GeometryCanvas.Children.Clear();
        if (points == null || points.Count < 3) return;

        double minX = points.Min(p => p.X);
        double maxX = points.Max(p => p.X);
        double minY = points.Min(p => p.Y);
        double maxY = points.Max(p => p.Y);

        double w = maxX - minX;
        double h = maxY - minY;
        if (w <= 0 || h <= 0) return;

        double canvasW = GeometryCanvas.Width;
        double canvasH = GeometryCanvas.Height;
        double padding = 20;

        double scale = System.Math.Min((canvasW - padding * 2) / w, (canvasH - padding * 2) / h);
        double offsetX = (canvasW - w * scale) / 2.0 - minX * scale;
        double offsetY = (canvasH - h * scale) / 2.0 - minY * scale;

        Polygon polygon = new()
        {
            Stroke = new SolidColorBrush(Colors.DodgerBlue),
            StrokeThickness = 2,
            Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(50, 0, 120, 215))
        };

        foreach (var p in points)
        {
            polygon.Points.Add(new Point(p.X * scale + offsetX, canvasH - (p.Y * scale + offsetY)));
        }

        GeometryCanvas.Children.Add(polygon);
    }
}

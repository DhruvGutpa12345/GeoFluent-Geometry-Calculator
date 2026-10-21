using System.Globalization;
using GeoFluent.Math;
using GeoFluent.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GeoFluent.Views;

public sealed partial class ThreeDimensionalPage : Page
{
    private readonly Dictionary<string, TextBox> _inputBoxes = [];

    public ThreeDimensionalPage()
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

        string tag = (CmbShape.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Sphere";

        switch (tag)
        {
            case "Sphere":
                AddInputField("Radius (r)", "r", "5");
                break;
            case "Hemisphere":
                AddInputField("Radius (r)", "r", "5");
                break;
            case "Cylinder":
                AddInputField("Base radius (r)", "r", "4");
                AddInputField("Height (h)", "h", "10");
                break;
            case "Cone":
                AddInputField("Base radius (r)", "r", "4");
                AddInputField("Height (h)", "h", "9");
                break;
            case "Cube":
                AddInputField("Edge length (a)", "a", "5");
                break;
            case "Prism":
                AddInputField("Length (l)", "l", "8");
                AddInputField("Width (w)", "w", "5");
                AddInputField("Height (h)", "h", "4");
                break;
            case "Pyramid":
                AddInputField("Base side (a)", "a", "6");
                AddInputField("Height (h)", "h", "8");
                break;
            case "Torus":
                AddInputField("Major radius (R - ring center to tube center)", "R", "8");
                AddInputField("Minor radius (r - tube radius)", "r", "2");
                break;
        }

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
            string tag = (CmbShape.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Sphere";
            string shapeName = (CmbShape.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Solid";

            ShapeCalculationResult result;

            switch (tag)
            {
                case "Sphere":
                    result = Formulas3D.Sphere(ReadVal("r"));
                    break;
                case "Hemisphere":
                    result = Formulas3D.Hemisphere(ReadVal("r"));
                    break;
                case "Cylinder":
                    result = Formulas3D.Cylinder(ReadVal("r"), ReadVal("h"));
                    break;
                case "Cone":
                    result = Formulas3D.Cone(ReadVal("r"), ReadVal("h"));
                    break;
                case "Cube":
                    result = Formulas3D.Cube(ReadVal("a"));
                    break;
                case "Prism":
                    result = Formulas3D.RectangularPrism(ReadVal("l"), ReadVal("w"), ReadVal("h"));
                    break;
                case "Pyramid":
                    result = Formulas3D.RegularPyramid(ReadVal("a"), ReadVal("h"));
                    break;
                case "Torus":
                    result = Formulas3D.Torus(ReadVal("R"), ReadVal("r"));
                    break;
                default:
                    return;
            }

            OutputsList.ItemsSource = result.Outputs;
            StepsList.ItemsSource = result.Steps;
            ResultsCard.Visibility = Visibility.Visible;
            StepsCard.Visibility = Visibility.Visible;

            // Record into history
            var primary = result.Outputs.FirstOrDefault(o => o.IsPrimary);
            string primaryText = primary.Label != null ? $"{primary.Label}: {primary.DisplayValue}" : "";
            HistoryService.Add(shapeName, "3D", primaryText);
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
}

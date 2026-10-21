using System.Globalization;
using GeoFluent.Math;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GeoFluent.Views;

public sealed partial class UnitConverterPage : Page
{
    private bool _isUpdating;

    public UnitConverterPage()
    {
        InitializeComponent();
        Loaded += (s, e) => PopulateUnits();
    }

    private void CmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
        {
            PopulateUnits();
        }
    }

    private void PopulateUnits()
    {
        _isUpdating = true;
        CmbFromUnit.Items.Clear();
        CmbToUnit.Items.Clear();

        UnitType category = GetCurrentCategory();
        var units = UnitConverter.GetUnits(category);

        foreach (var u in units)
        {
            CmbFromUnit.Items.Add(new ComboBoxItem { Content = u.Name, Tag = u.Id });
            CmbToUnit.Items.Add(new ComboBoxItem { Content = u.Name, Tag = u.Id });
        }

        if (CmbFromUnit.Items.Count > 0) CmbFromUnit.SelectedIndex = 0;
        if (CmbToUnit.Items.Count > 1) CmbToUnit.SelectedIndex = 1;
        else if (CmbToUnit.Items.Count > 0) CmbToUnit.SelectedIndex = 0;

        _isUpdating = false;
        PerformConversion();
    }

    private UnitType GetCurrentCategory()
    {
        string tag = (CmbCategory.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Length";
        return tag switch
        {
            "Area" => UnitType.Area,
            "Volume" => UnitType.Volume,
            "Angle" => UnitType.Angle,
            _ => UnitType.Length
        };
    }

    private void CmbFromUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isUpdating) PerformConversion();
    }

    private void CmbToUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isUpdating) PerformConversion();
    }

    private void TxtFromValue_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isUpdating) PerformConversion();
    }

    private void BtnSwap_Click(object sender, RoutedEventArgs e)
    {
        _isUpdating = true;
        int temp = CmbFromUnit.SelectedIndex;
        CmbFromUnit.SelectedIndex = CmbToUnit.SelectedIndex;
        CmbToUnit.SelectedIndex = temp;
        _isUpdating = false;
        PerformConversion();
    }

    private void PerformConversion()
    {
        if (CmbFromUnit.SelectedItem is not ComboBoxItem fromItem ||
            CmbToUnit.SelectedItem is not ComboBoxItem toItem)
            return;

        string fromId = fromItem.Tag?.ToString() ?? "";
        string toId = toItem.Tag?.ToString() ?? "";
        UnitType type = GetCurrentCategory();

        if (double.TryParse(TxtFromValue.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
        {
            double converted = UnitConverter.Convert(val, fromId, toId, type);
            TxtToValue.Text = UnitConverter.FormatNumber(converted, 6);
            TxtConversionFormula.Text = $"{val} {fromId} = {TxtToValue.Text} {toId}";
        }
        else
        {
            TxtToValue.Text = "—";
            TxtConversionFormula.Text = "";
        }
    }
}

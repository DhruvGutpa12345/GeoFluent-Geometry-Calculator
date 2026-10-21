using System.Globalization;
using GeoFluent.Math;
using GeoFluent.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace GeoFluent.Views;

public sealed partial class UnitConverterPage : Page
{
    private bool _isUpdating;
    private UnitType _activeCategory = UnitType.Length;

    public UnitConverterPage()
    {
        InitializeComponent();
        Loaded += (s, e) => PopulateUnits();
    }

    private void BtnCat_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
        {
            _activeCategory = tag switch
            {
                "Area" => UnitType.Area,
                "Volume" => UnitType.Volume,
                "Angle" => UnitType.Angle,
                _ => UnitType.Length
            };

            // Update pill button styles
            BtnCatLength.Style = _activeCategory == UnitType.Length ? (Style)Application.Current.Resources["AccentButtonStyle"] : (Style)Application.Current.Resources["DefaultButtonStyle"];
            BtnCatArea.Style = _activeCategory == UnitType.Area ? (Style)Application.Current.Resources["AccentButtonStyle"] : (Style)Application.Current.Resources["DefaultButtonStyle"];
            BtnCatVolume.Style = _activeCategory == UnitType.Volume ? (Style)Application.Current.Resources["AccentButtonStyle"] : (Style)Application.Current.Resources["DefaultButtonStyle"];
            BtnCatAngle.Style = _activeCategory == UnitType.Angle ? (Style)Application.Current.Resources["AccentButtonStyle"] : (Style)Application.Current.Resources["DefaultButtonStyle"];

            PopulateUnits();
        }
    }

    private void PopulateUnits()
    {
        _isUpdating = true;
        CmbFromUnit.Items.Clear();
        CmbToUnit.Items.Clear();

        var units = UnitConverter.GetUnits(_activeCategory);

        foreach (var u in units)
        {
            CmbFromUnit.Items.Add(new ComboBoxItem { Content = $"{u.Name} [{u.Symbol}]", Tag = u.Id });
            CmbToUnit.Items.Add(new ComboBoxItem { Content = $"{u.Name} [{u.Symbol}]", Tag = u.Id });
        }

        if (CmbFromUnit.Items.Count > 0) CmbFromUnit.SelectedIndex = 0;
        if (CmbToUnit.Items.Count > 1) CmbToUnit.SelectedIndex = 1;
        else if (CmbToUnit.Items.Count > 0) CmbToUnit.SelectedIndex = 0;

        _isUpdating = false;
        PerformConversion();
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

        if (double.TryParse(TxtFromValue.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
        {
            double converted = UnitConverter.Convert(val, fromId, toId, _activeCategory);
            string formatted = UnitConverter.FormatNumber(converted, UserSettingsService.Precision);
            string fromSym = UnitConverter.GetSymbol(fromId, _activeCategory);
            string toSym = UnitConverter.GetSymbol(toId, _activeCategory);

            TxtToValue.Text = formatted;
            TxtConversionFormula.Text = $"{val} {fromSym} = {formatted} {toSym}";
        }
        else
        {
            TxtToValue.Text = "—";
            TxtConversionFormula.Text = "";
        }
    }

    private void BtnCopyResult_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtToValue.Text) || TxtToValue.Text == "—") return;
        string toId = (CmbToUnit.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "";
        string toSym = UnitConverter.GetSymbol(toId, _activeCategory);
        string textToCopy = $"{TxtToValue.Text} {toSym}";

        var pkg = new DataPackage();
        pkg.SetText(textToCopy);
        Clipboard.SetContent(pkg);

        ToastBar.Message = $"Copied '{textToCopy}' to clipboard!";
        ToastBar.IsOpen = true;
    }
}

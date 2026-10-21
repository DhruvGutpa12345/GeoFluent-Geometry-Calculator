using System.Reflection;
using GeoFluent.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GeoFluent.Views;

public sealed partial class SettingsPage : Page
{
    private bool _isInitializing = true;

    public SettingsPage()
    {
        InitializeComponent();
        Loaded += SettingsPage_Loaded;
    }

    private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        _isInitializing = true;

        // Display version
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        TxtAppVersion.Text = version != null
            ? $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}"
            : "1.6.0.1";

        // Current Theme
        ThemeRadioGroup.SelectedIndex = UserSettingsService.CurrentTheme switch
        {
            ElementTheme.Dark => 1,
            ElementTheme.Light => 2,
            _ => 0
        };

        // Precision
        int prec = UserSettingsService.Precision;
        for (int i = 0; i < CmbPrecision.Items.Count; i++)
        {
            if ((CmbPrecision.Items[i] as ComboBoxItem)?.Tag?.ToString() == prec.ToString())
            {
                CmbPrecision.SelectedIndex = i;
                break;
            }
        }
        if (CmbPrecision.SelectedIndex < 0) CmbPrecision.SelectedIndex = 2; // 4 decimals

        _isInitializing = false;
    }

    private void ThemeRadioGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing) return;

        ElementTheme targetTheme = ThemeRadioGroup.SelectedIndex switch
        {
            1 => ElementTheme.Dark,
            2 => ElementTheme.Light,
            _ => ElementTheme.Default
        };

        UserSettingsService.CurrentTheme = targetTheme;

        if (App.MainWindowInstance?.Content is FrameworkElement root)
        {
            root.RequestedTheme = targetTheme;
        }

        UserSettingsService.NotifyChanged();
    }

    private void CmbPrecision_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing || CmbPrecision.SelectedItem is not ComboBoxItem item) return;
        if (int.TryParse(item.Tag?.ToString(), out int prec))
        {
            UserSettingsService.Precision = prec;
            UserSettingsService.NotifyChanged();
        }
    }
}

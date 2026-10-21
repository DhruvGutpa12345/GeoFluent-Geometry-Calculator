using GeoFluent.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GeoFluent;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        try
        {
            InitializeComponent();

            // Native TitleBar integration
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            // Navigate to default page
            ContentFrame.Navigate(typeof(TwoDimensionalPage));
        }
        catch (Exception ex)
        {
            string logPath = System.IO.Path.Combine(AppContext.BaseDirectory, "mainwindow_crash.log");
            System.IO.File.WriteAllText(logPath, $"[MainWindow Exception] {ex}\nStackTrace:\n{ex.StackTrace}");
            throw;
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
            return;
        }

        if (args.SelectedItemContainer is NavigationViewItem item)
        {
            string? tag = item.Tag?.ToString();
            Type pageType = tag switch
            {
                "2D" => typeof(TwoDimensionalPage),
                "3D" => typeof(ThreeDimensionalPage),
                "Converter" => typeof(UnitConverterPage),
                "History" => typeof(HistoryPage),
                _ => typeof(TwoDimensionalPage)
            };

            ContentFrame.Navigate(pageType);
        }
    }
}

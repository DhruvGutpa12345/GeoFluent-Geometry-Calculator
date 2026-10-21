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

            // Sync navigation selection when Frame navigates
            ContentFrame.Navigated += ContentFrame_Navigated;

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

            if (ContentFrame.CurrentSourcePageType != pageType)
            {
                ContentFrame.Navigate(pageType);
            }
        }
    }

    private void ContentFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        if (e.SourcePageType == typeof(SettingsPage))
        {
            NavView.SelectedItem = NavView.SettingsItem;
        }
        else
        {
            string tag = e.SourcePageType switch
            {
                var t when t == typeof(TwoDimensionalPage) => "2D",
                var t when t == typeof(ThreeDimensionalPage) => "3D",
                var t when t == typeof(UnitConverterPage) => "Converter",
                var t when t == typeof(HistoryPage) => "History",
                _ => "2D"
            };

            foreach (var item in NavView.MenuItems)
            {
                if (item is NavigationViewItem nvi && nvi.Tag?.ToString() == tag)
                {
                    NavView.SelectedItem = nvi;
                    break;
                }
            }
        }
    }
}

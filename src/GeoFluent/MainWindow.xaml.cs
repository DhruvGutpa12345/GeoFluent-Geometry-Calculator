using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GeoFluent;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Native TitleBar integration
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        // Display live build version from Assembly / Version.props
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        TxtBuildVersion.Text = version != null
            ? $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}"
            : "1.6.0.1";
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            // Settings & About
            return;
        }

        if (args.SelectedItemContainer is NavigationViewItem item)
        {
            string? tag = item.Tag?.ToString();
            // Route to appropriate view
        }
    }
}

using System.Reflection;
using Microsoft.UI.Xaml.Controls;

namespace GeoFluent.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();

        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        TxtAppVersion.Text = version != null
            ? $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}"
            : "1.6.0.1";
    }
}

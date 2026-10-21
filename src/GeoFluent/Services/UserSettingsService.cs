using Microsoft.UI.Xaml;

namespace GeoFluent.Services;

public static class UserSettingsService
{
    public static ElementTheme CurrentTheme { get; set; } = ElementTheme.Default;

    public static int Precision { get; set; } = 4;

    public static string DefaultLengthUnit { get; set; } = "cm";
    public static string DefaultAreaUnit { get; set; } = "cm2";
    public static string DefaultVolumeUnit { get; set; } = "cm3";
    public static string DefaultAngleUnit { get; set; } = "deg";

    public static event Action? SettingsChanged;

    public static void NotifyChanged()
    {
        SettingsChanged?.Invoke();
    }
}

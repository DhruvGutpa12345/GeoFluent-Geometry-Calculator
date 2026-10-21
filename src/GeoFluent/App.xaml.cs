using Microsoft.UI.Xaml;

namespace GeoFluent;

public partial class App : Application
{
    private Window? _mainWindow;

    public App()
    {
        UnhandledException += (sender, e) =>
        {
            try
            {
                string log = $"[UnhandledException] {DateTime.Now}: {e.Message}\n{e.Exception}\nStackTrace:\n{e.Exception?.StackTrace}";
                string path = System.IO.Path.Combine(AppContext.BaseDirectory, "crash.log");
                System.IO.File.WriteAllText(path, log);
            }
            catch { }
        };

        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            _mainWindow = new MainWindow();
            _mainWindow.Activate();
        }
        catch (Exception ex)
        {
            System.IO.File.WriteAllText("crash.log", $"[OnLaunched Exception] {ex}");
            throw;
        }
    }
}

using Microsoft.UI.Xaml;
using Velopack;

namespace MoscoviumThree;

public partial class App : Application
{
    public App()
    {
        VelopackApp.Build().Run();
        this.InitializeComponent();
        this.UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        try
        {
            string logPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "MoscoviumCrash.txt");
            System.IO.File.WriteAllText(logPath, e.Exception.ToString());
        }
        catch { }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            m_window = new MainWindow();
            m_window.Activate();
        }
        catch (System.Exception ex)
        {
            try
            {
                string logPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "MoscoviumCrash.txt");
                System.IO.File.WriteAllText(logPath, ex.ToString());
            }
            catch { }
            // Throw so app crashes and OS knows, but we have the log
            throw; 
        }
    }

    public static Window? m_window;
}

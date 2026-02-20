using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.IO;

namespace MoscoviumThree.Pages;

public sealed partial class CustomizationPage : Page
{
    public CustomizationPage()
    {
        this.InitializeComponent();
    }

    private void InstallOpenShell_Click(object sender, RoutedEventArgs e)
    {
        string installerPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Bundled", "OpenShellSetup_4_4_196.exe");
        RunInstaller(installerPath);
    }

    private void InstallNileSoft_Click(object sender, RoutedEventArgs e)
    {
        string installerPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Bundled", "setup-x64.msi");
        RunInstaller(installerPath);
    }

    private void InstallStartAllBack_Click(object sender, RoutedEventArgs e)
    {
        string installerPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Bundled", "StartAllBackSetup.exe");
        RunInstaller(installerPath);
    }

    private void ResetStartAllBack_Click(object sender, RoutedEventArgs e)
    {
        string scriptPath = Path.Combine(AppContext.BaseDirectory, "StartAllBack.ps1");
        if (File.Exists(scriptPath))
        {
            try
            {
                Process.Start(new ProcessStartInfo("powershell.exe", $"-ExecutionPolicy Bypass -File \"{scriptPath}\"") { UseShellExecute = true, Verb = "runas" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to run script: {ex.Message}");
            }
        }
        else
        {
             Debug.WriteLine("StartAllBack.ps1 not found.");
        }
    }

    private void InstallExplorerPatcher_Click(object sender, RoutedEventArgs e)
    {
        string installerPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Bundled", "ep_setup.exe");
        RunInstaller(installerPath);
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch { }
    }

    private void RunInstaller(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to run installer: {ex.Message}");
            }
        }
        else
        {
             Debug.WriteLine($"Installer not found at: {path}");
        }
    }
}

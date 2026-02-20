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
        // TODO: Implement OpenShell installation logic (e.g., download and run installer silently)
        // For now, we'll open the GitHub release page as a placeholder
        OpenUrl("https://github.com/Open-Shell/Open-Shell-Menu/releases");
    }

    private void InstallNileSoft_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Implement NileSoft Shell installation logic
        // Placeholder: Open website
        OpenUrl("https://nilesoft.org/");
    }

    private void InstallStartAllBack_Click(object sender, RoutedEventArgs e)
    {
        OpenUrl("https://www.startallback.com/");
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
        OpenUrl("https://github.com/valinet/ExplorerPatcher/releases");
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch { }
    }
}

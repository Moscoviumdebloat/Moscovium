using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace MoscoviumThree.Helpers;

public static class SetupAutomationHelper
{
    public static async Task<bool> InstallWingetAppAsync(string wingetId, Action<string>? progressCallback = null)
    {
        progressCallback?.Invoke($"Installing {wingetId}...");
        
        var startInfo = new ProcessStartInfo
        {
            FileName = "winget",
            Arguments = $"install --id {wingetId} --silent --accept-package-agreements --accept-source-agreements",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process == null) return false;
            
            await process.WaitForExitAsync();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to install {wingetId}: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> RunWindowsUpdateAsync(Action<string>? progressCallback = null)
    {
        progressCallback?.Invoke("Running Windows Update...");
        
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell",
            Arguments = "-Command \"Install-Module PSWindowsUpdate -Force -Confirm:$false; Get-WindowsUpdate -Install -AcceptAll -AutoReboot:$false\"",
            UseShellExecute = true,
            Verb = "runas", // Requires admin
            CreateNoWindow = false // Let user see the update window
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process == null) return false;
            
            await process.WaitForExitAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}

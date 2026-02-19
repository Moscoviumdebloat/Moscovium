using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Helpers;

namespace MoscoviumThree.Pages;

public sealed partial class ToolboxPage : Page
{
    public ToolboxPage()
    {
        this.InitializeComponent();
    }

    private void BtnCTT_Click(object sender, RoutedEventArgs e)
    {
        ProcessHelper.RunElevatedPowerShellRaw("iwr -useb https://christitus.com/win | iex");
    }

    private async void BtnSAB_Click(object sender, RoutedEventArgs e)
    {
        try 
        {
            string sabPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "StartAllBack.ps1");
            if (System.IO.File.Exists(sabPath))
            {
                 // Run hidden/separate process to verify
                 var proc = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                 {
                     FileName = "powershell.exe",
                     Arguments = $"-ExecutionPolicy Bypass -File \"{sabPath}\"",
                     UseShellExecute = true,
                     Verb = "runas" 
                 });

                 if (proc != null)
                 {
                     await System.Threading.Tasks.Task.Run(() => proc.WaitForExit());
                     
                     // Prompt for restart
                     var restartDialog = new ContentDialog
                     {
                         Title = "Restart Required",
                         Content = "StartAllBack trial reset has been applied. You need to restart your computer for changes to take effect.\n\nRestart now?",
                         PrimaryButtonText = "Restart Now",
                         CloseButtonText = "Later",
                         DefaultButton = ContentDialogButton.Primary,
                         XamlRoot = this.XamlRoot
                     };

                     var result = await restartDialog.ShowAsync();
                     if (result == ContentDialogResult.Primary)
                     {
                         System.Diagnostics.Process.Start("shutdown", "/r /t 0");
                     }
                 }
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "StartAllBack.ps1 not found.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
        catch (System.Exception ex)
        {
             var dialog = new ContentDialog
             {
                 Title = "Error",
                 Content = $"Failed to run script: {ex.Message}",
                 CloseButtonText = "OK",
                 XamlRoot = this.XamlRoot
             };
             await dialog.ShowAsync();
        }
    }

    private void BtnWin11_Click(object sender, RoutedEventArgs e)
    {
        ProcessHelper.RunElevatedPowerShellRaw("& ([scriptblock]::Create((irm \"https://debloat.raphi.re/\")))");
    }

    private void BtnMAS_Click(object sender, RoutedEventArgs e)
    {
        ProcessHelper.RunElevatedPowerShellRaw("irm https://get.activated.win | iex");
    }





}

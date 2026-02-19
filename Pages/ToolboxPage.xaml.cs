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
                 // Run elevated via ProcessHelper to ensure secure argument handling
                 var proc = ProcessHelper.RunElevatedPowerShellRaw($"& '{sabPath.Replace("'", "''")}'");

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

    private async void BtnWifi_Click(object sender, RoutedEventArgs e)
    {
        // Network Optimization Popup
        var rbBetter = new RadioButton 
        { 
            Content = "Better WiFi (Disable TCP Autotuning)", 
            Tag = "Better",
            Margin = new Thickness(0, 4, 0, 4)
        };
        var rbDefault = new RadioButton 
        { 
            Content = "Default (Restore TCP Autotuning)", 
            Tag = "Default",
            Margin = new Thickness(0, 4, 0, 4)
        };

        var stack = new StackPanel { Spacing = 4 };
        stack.Children.Add(rbBetter);
        stack.Children.Add(rbDefault);

        var dialog = new ContentDialog
        {
            Title = "Network Optimizations",
            Content = stack,
            PrimaryButtonText = "Apply",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            if (rbBetter.IsChecked == true)
            {
                ProcessHelper.RunElevatedPowerShellRaw("netsh int tcp set global autotuninglevel=disabled");
            }
            else if (rbDefault.IsChecked == true)
            {
                ProcessHelper.RunElevatedPowerShellRaw("netsh int tcp set global autotuninglevel=normal");
            }
        }
    }

    private async void BtnDisableDynamicTick_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ProcessHelper.RunElevated("bcdedit", "/set disabledynamictick yes");

            var dialog = new ContentDialog
            {
                Title = "Success",
                Content = "The command 'bcdedit /set disabledynamictick yes' has been executed.\nA system restart is recommended for changes to take full effect.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
        catch (System.Exception ex)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = $"Failed to execute command: {ex.Message}",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}

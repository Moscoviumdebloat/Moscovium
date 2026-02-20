using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Helpers;
using System;
using System.Threading.Tasks;

namespace MoscoviumThree.Pages;

public sealed partial class OptimizationsPage : Page
{
    public OptimizationsPage()
    {
        this.InitializeComponent();
    }

    private async void BtnRaphiDebloat_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string[] debloatArgs =
            {
                "-Silent", "-RemoveApps", "-RemoveGamingApps", "-DisableTelemetry",
                "-DisableBing", "-DisableSuggestions", "-DisableLockscreenTips",
                "-RevertContextMenu", "-TaskbarAlignLeft", "-HideSearchTb",
                "-DisableWidgets", "-DisableCopilot", "-ClearStartAllUsers",
                "-DisableDVR", "-DisableStartRecommended", "-ExplorerToThisPC",
                "-DisableMouseAcceleration", "-DisableDesktopSpotlight",
                "-DisableSettings365Ads", "-DisableSettingsHome",
                "-DisablePaintAI", "-DisableNotepadAI", "-DisableStickyKeys"
            };

            var arguments = "&([scriptblock]::Create((irm \"https://debloat.raphi.re/\"))) -RunDefaults " +
                            string.Join(" ", debloatArgs);
            
            ProcessHelper.RunElevatedPowerShellRaw(arguments);
            
            await ShowInfo("Raphi debloat script has been launched.");
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to run Raphi debloat: {ex.Message}");
        }
    }

    private async void BtnChrisTitus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var debloatJson = ResourceHelper.ExtractToTemp("Debloat.json", "Debloat.json");
            ProcessHelper.RunPowerShellCommand("irm 'https://christitus.com/win' -OutFile \"$env:TEMP\\winutil.ps1\"");
            
            // Short delay to ensure download
            await Task.Delay(2000);
            
            ProcessHelper.RunPowerShellCommand($"& \"$env:TEMP\\winutil.ps1\" -Config '{debloatJson}' -Run");
            
            await ShowInfo("Chris Titus WinUtil (Automated) has been launched.");
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to run WinUtil: {ex.Message}");
        }
    }

    private async void BtnNetwork_Click(object sender, RoutedEventArgs e)
    {
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

    private async void BtnDynamicTick_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ProcessHelper.RunElevated("bcdedit", "/set disabledynamictick yes");

            await ShowInfo("The command 'bcdedit /set disabledynamictick yes' has been executed.\nA system restart is recommended for changes to take full effect.");
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to execute command: {ex.Message}");
        }
    }

    private async void BtnWin32Priority_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Dictionary tweak: Use reg.exe to handle elevation prompts automatically
            ProcessHelper.RunElevated("reg", "add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\PriorityControl\" /v Win32PrioritySeparation /t REG_DWORD /d 22 /f");
            
            await ShowInfo("Registry command executed.\nA restart is required for changes to take effect.");
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to launch registry command: {ex.Message}");
        }
    }

    private async Task ShowError(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Error",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }

    private async Task ShowInfo(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Success",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }
}

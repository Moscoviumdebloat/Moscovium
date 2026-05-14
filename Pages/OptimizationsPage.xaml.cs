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
            
            // Use the official single-command approach: download + execute with config in one shot
            var command = $"iex \"& {{ $(irm christitus.com/win) }} -Config '{debloatJson}' -Run\"";
            ProcessHelper.RunElevatedPowerShellRaw(command);
            
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
        var rbApply = new RadioButton
        {
            Content = "Disable Dynamic Tick (Apply)",
            Tag = "Apply",
            IsChecked = true,
            Margin = new Thickness(0, 4, 0, 4)
        };
        var rbRevert = new RadioButton
        {
            Content = "Re-enable Dynamic Tick (Revert)",
            Tag = "Revert",
            Margin = new Thickness(0, 4, 0, 4)
        };

        var stack = new StackPanel { Spacing = 4 };
        stack.Children.Add(rbApply);
        stack.Children.Add(rbRevert);

        var dialog = new ContentDialog
        {
            Title = "Dynamic Tick",
            Content = stack,
            PrimaryButtonText = "Apply",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result != ContentDialogResult.Primary) return;

        try
        {
            if (rbApply.IsChecked == true)
            {
                ProcessHelper.RunElevated("bcdedit", "/set disabledynamictick yes");
                await ShowInfo("Dynamic tick has been disabled.\nA system restart is recommended for changes to take full effect.");
            }
            else if (rbRevert.IsChecked == true)
            {
                ProcessHelper.RunElevated("bcdedit", "/deletevalue disabledynamictick");
                await ShowInfo("Dynamic tick has been re-enabled (reverted to default).\nA system restart is recommended for changes to take full effect.");
            }
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to execute command: {ex.Message}");
        }
    }

    private async void BtnWin32Priority_Click(object sender, RoutedEventArgs e)
    {
        var rbApply = new RadioButton
        {
            Content = "Set Win32PrioritySeparation to 22 (Apply)",
            Tag = "Apply",
            IsChecked = true,
            Margin = new Thickness(0, 4, 0, 4)
        };
        var rbRevert = new RadioButton
        {
            Content = "Restore Win32PrioritySeparation to default (Revert)",
            Tag = "Revert",
            Margin = new Thickness(0, 4, 0, 4)
        };

        var stack = new StackPanel { Spacing = 4 };
        stack.Children.Add(rbApply);
        stack.Children.Add(rbRevert);

        var dialog = new ContentDialog
        {
            Title = "Win32 Priority Separation",
            Content = stack,
            PrimaryButtonText = "Apply",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result != ContentDialogResult.Primary) return;

        try
        {
            if (rbApply.IsChecked == true)
            {
                ProcessHelper.RunElevated("reg", "add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\PriorityControl\" /v Win32PrioritySeparation /t REG_DWORD /d 22 /f");
                await ShowInfo("Win32PrioritySeparation set to 22.\nA restart is required for changes to take effect.");
            }
            else if (rbRevert.IsChecked == true)
            {
                ProcessHelper.RunElevated("reg", "add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\PriorityControl\" /v Win32PrioritySeparation /t REG_DWORD /d 2 /f");
                await ShowInfo("Win32PrioritySeparation restored to default (2).\nA restart is required for changes to take effect.");
            }
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

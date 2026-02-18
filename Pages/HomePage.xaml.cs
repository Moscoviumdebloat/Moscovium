using System.Diagnostics;
using System.IO.Compression;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Helpers;

namespace MoscoviumThree.Pages;

public sealed partial class HomePage : Page
{


    public HomePage()
    {
        this.InitializeComponent();
    }

    private async void BtnDebloat_Click(object sender, RoutedEventArgs e)
    {
        // Define options for shell modification
        var rbOption1 = new RadioButton 
        { 
            Content = "Install Explorer Patcher, OpenShell, and Nilesoft Shell", 
            IsChecked = true, 
            Tag = "Option1",
            Margin = new Thickness(0, 4, 0, 4)
        };
        var rbOption2 = new RadioButton 
        { 
            Content = "Install StartAllBack (Trial/License required)", 
            Tag = "Option2",
            Margin = new Thickness(0, 4, 0, 4)
        };
        var rbOption3 = new RadioButton 
        { 
            Content = "No shell modifications (Debloat only)", 
            Tag = "Option3",
            Margin = new Thickness(0, 4, 0, 4)
        };

        var stack = new StackPanel { Spacing = 4 };
        stack.Children.Add(rbOption1);
        stack.Children.Add(rbOption2);
        stack.Children.Add(rbOption3);

        var dialog = new ContentDialog
        {
            Title = "Debloat & Shell Options",
            Content = stack,
            PrimaryButtonText = "Run Selected",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result != ContentDialogResult.Primary) return;

        // 1. Handle Shell Installs
        if (rbOption1.IsChecked == true)
        {
            try
            {
                // OpenShell
                var openShellPath = ResourceHelper.ExtractToTemp("OpenShellSetup_4_4_196.exe", "OpenShellSetup_4_4_196.exe");
                ProcessHelper.Run(openShellPath);

                // Explorer Patcher
                var epPath = ResourceHelper.ExtractToTemp("ep_setup.exe", "ep_setup.exe");
                ProcessHelper.Run(epPath);

                // Nilesoft Shell
                var nsPath = ResourceHelper.ExtractToTemp("setup-x64.msi", "nilesoft_setup-x64.msi");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "msiexec.exe",
                    Arguments = $"/i \"{nsPath}\"",
                    UseShellExecute = true
                })?.WaitForExit(); // Wait for MSI to finish if possible, though Process.Start might return null if reused
            }
            catch (Exception ex)
            {
                await ShowError($"Failed to extract/run installers: {ex.Message}");
            }
        }
        else if (rbOption2.IsChecked == true)
        {
            try 
            {
                // Run StartAllBack.ps1
                string sabPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "StartAllBack.ps1");
                if (System.IO.File.Exists(sabPath))
                {
                     // Run hidden/separate process to verify
                     Process.Start(new ProcessStartInfo
                     {
                         FileName = "powershell.exe",
                         Arguments = $"-ExecutionPolicy Bypass -File \"{sabPath}\"",
                         UseShellExecute = true,
                         Verb = "runas" // Ensure admin
                     })?.WaitForExit();
                }
                else
                {
                    await ShowError("StartAllBack.ps1 not found.");
                }
            }
            catch (Exception ex)
            {
                await ShowError($"Failed to run StartAllBack script: {ex.Message}");
            }
        }

        // 2. Run debloat scripts (Raphi Silent + WinUtil)
        // Note: StartAllBack script might have restarted explorer, so we proceed.
        
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
        
        // Use RunElevatedPowerShellRaw but maybe wait?
        // Current ProcessHelper Implementation implies fire-and-forget or output capture?
        // RunElevatedPowerShellRaw uses Process.Start with verbs.
        ProcessHelper.RunElevatedPowerShellRaw(arguments);

        // ChrisTitus WinUtil with debloat config
        try
        {
            var debloatJson = ResourceHelper.ExtractToTemp("Debloat.json", "Debloat.json");
            ProcessHelper.RunPowerShellCommand("irm 'https://christitus.com/win' -OutFile \"$env:TEMP\\winutil.ps1\"");
            
            // Wait a bit is risky but fine for now
            await Task.Delay(2000);
            
            ProcessHelper.RunPowerShellCommand($"& \"$env:TEMP\\winutil.ps1\" -Config '{debloatJson}' -Run");
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to run WinUtil: {ex.Message}");
        }
    }

    private async void BtnWallpaper_Click(object sender, RoutedEventArgs e)
    {
        // Style selection dialog
        var styleCombo = new RadioButtons
        {
            Header = "Select wallpaper fit style:",
            Items = { "Fill", "Fit", "Stretch", "Tile", "Center", "Span" },
            SelectedIndex = 0
        };

        var dialog = new ContentDialog
        {
            Title = "Wallpaper Settings",
            Content = styleCombo,
            PrimaryButtonText = "Choose Image",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result != ContentDialogResult.Primary) return;

        var selectedStyle = styleCombo.SelectedItem?.ToString() ?? "Fill";

        // File picker
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.FileTypeFilter.Add(".bmp");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");

        // Initialize picker with window handle
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Current as App != null
            ? ((App)App.Current).GetType().GetField("m_window",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(App.Current) as Microsoft.UI.Xaml.Window
            : null);

        if (hwnd != IntPtr.Zero)
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            WallpaperHelper.Set(file.Path, selectedStyle);
            await ShowInfo("Wallpaper set successfully!");
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to set wallpaper: {ex.Message}");
        }
    }



    private void BtnRestartExplorer_Click(object sender, RoutedEventArgs e)
    {
        foreach (var proc in Process.GetProcessesByName("explorer"))
        {
            proc.Kill();
        }
        Process.Start(@"C:\Windows\explorer.exe");
    }

    private async void BtnRuntime_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var extractPath = ResourceHelper.ExtractZipToTemp(
                "Visual-C-Runtimes-All-in-One-Nov-2025.zip",
                "VisualCDistributables");
            ProcessHelper.Run(Path.Combine(extractPath, "install_all.bat"));
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to install runtimes: {ex.Message}");
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

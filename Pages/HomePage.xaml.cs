using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Helpers;
using Windows.ApplicationModel.DataTransfer;

namespace MoscoviumThree.Pages;

public sealed partial class HomePage : Page
{
    private static readonly HttpClient _httpClient = new();

    public HomePage()
    {
        this.InitializeComponent();
    }

    private async void BtnDebloat_Click(object sender, RoutedEventArgs e)
    {
        // Ask about optional shell tools
        var dialog = new ContentDialog
        {
            Title = "Debloat Windows",
            Content = "Would you also like to install Explorer Patcher, OpenShell, and Nilesoft Shell with the debloat script?",
            PrimaryButtonText = "Yes, install all",
            SecondaryButtonText = "No, debloat only",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.None) return;

        if (result == ContentDialogResult.Primary)
        {
            // Install optional shell enhancements
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
                });
            }
            catch (Exception ex)
            {
                await ShowError($"Failed to extract installers: {ex.Message}");
            }
        }

        // Run debloat scripts
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

        // ChrisTitus WinUtil with debloat config
        try
        {
            var debloatJson = ResourceHelper.ExtractToTemp("Debloat.json", "Debloat.json");
            ProcessHelper.RunPowerShellCommand("irm 'https://christitus.com/win' -OutFile \"$env:TEMP\\winutil.ps1\"");
            await Task.Delay(1500);
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

    private async void BtnYaboCfg_Click(object sender, RoutedEventArgs e)
    {
        var folders = SteamPathHelper.FindCfgFolders();
        if (folders.Count == 0)
        {
            await ShowError("Could not find CS2 cfg folder.");
            return;
        }

        try
        {
            var response = await _httpClient.GetAsync("https://raw.githubusercontent.com/Yabosen/YabosenCFG/main/yabosen.cfg");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();

            foreach (var folder in folders)
            {
                var dest = Path.Combine(folder, "yabosen.cfg");
                File.WriteAllBytes(dest, data);
            }

            await ShowInfo($"yabosen.cfg installed to {folders.Count} location(s):\n" +
                          string.Join("\n", folders));
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to download config: {ex.Message}");
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

    private void DropZone_DragOver(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Drop .cfg files";
        }
    }

    private async void DropZone_Drop(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        if (!e.DataView.Contains(StandardDataFormats.StorageItems)) return;

        var items = await e.DataView.GetStorageItemsAsync();
        var folders = SteamPathHelper.FindCfgFolders();

        if (folders.Count == 0)
        {
            await ShowError("Could not find CS2 cfg folder.");
            return;
        }

        foreach (var item in items)
        {
            if (item is Windows.Storage.StorageFile file)
            {
                if (!file.FileType.Equals(".cfg", StringComparison.OrdinalIgnoreCase))
                {
                    await ShowError($"'{file.Name}' is not a .cfg file.");
                    continue;
                }

                foreach (var folder in folders)
                {
                    var dest = Path.Combine(folder, file.Name);
                    File.Copy(file.Path, dest, true);
                }

                await ShowInfo($"'{file.Name}' copied to {folders.Count} CS2 cfg location(s).");
            }
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

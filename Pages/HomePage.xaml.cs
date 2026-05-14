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

    private async void BtnAutomateSetup_Click(object sender, RoutedEventArgs e)
    {
        var setupControl = new SetupAutomationDialog();

        var dialog = new ContentDialog
        {
            Title = "PC Setup Automation",
            Content = setupControl,
            PrimaryButtonText = "Run Setup",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary) return;

        // Start progressive execution UI
        var progressControl = new StackPanel { Spacing = 12 };
        var infoText = new TextBlock { Text = "Starting automation...", TextWrapping = TextWrapping.Wrap };
        var progressBar = new ProgressBar { IsIndeterminate = true };
        progressControl.Children.Add(infoText);
        progressControl.Children.Add(progressBar);

        var progressDialog = new ContentDialog
        {
            Title = "Automating Setup...",
            Content = progressControl,
            XamlRoot = this.XamlRoot
        };

        _ = progressDialog.ShowAsync();

        try
        {
            // Update UI Helper Action
            Action<string> updateProgress = (msg) =>
            {
                DispatcherQueue.TryEnqueue(() => infoText.Text = msg);
            };

            // 1. Windows Updates
            if (setupControl.RunWindowsUpdate)
            {
                updateProgress("Running Windows Update. This may open a new window...");
                await SetupAutomationHelper.RunWindowsUpdateAsync(updateProgress);
            }

            // 2. Winget Apps
            var wingetApps = setupControl.GetSelectedWingetApps();
            if (wingetApps.Count > 0)
            {
                foreach (var app in wingetApps)
                {
                    updateProgress($"Installing {app} via Winget...");
                    await SetupAutomationHelper.InstallWingetAppAsync(app, updateProgress);
                }
            }

            // 3. VC++ Runtimes
            if (setupControl.InstallVCRuntimes)
            {
                updateProgress("Installing VC++ Runtimes...");
                var extractPath = ResourceHelper.ExtractZipToTemp("Visual-C-Runtimes-All-in-One-Nov-2025.zip", "VisualCDistributables");
                ProcessHelper.Run(System.IO.Path.Combine(extractPath, "install_all.bat"));
            }

            // 4. Tweaks (Launch externally)
            if (setupControl.RunChrisTitus)
            {
                updateProgress("Launching Chris Titus WinUtil...");
                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"irm christitus.com/win | iex\"",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(startInfo);
            }

            if (setupControl.RunRaphi)
            {
                updateProgress("Launching Raphi Debloat...");
                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"iwr -useb https://win11debloat.raphire.com/ | iex\"",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(startInfo);
            }

            progressDialog.Hide();
            await ShowInfo("Setup Automation Completed Successfully!\nNote: Some system tweaks or Windows Updates may require a manual restart to take effect.");
        }
        catch (Exception ex)
        {
            progressDialog.Hide();
            await ShowError($"An error occurred during setup: {ex.Message}");
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
            SecondaryButtonText = "Control Panel",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Secondary)
        {
            try
            {
                Process.Start(new ProcessStartInfo("powershell.exe", "-NoExit -Command control") { UseShellExecute = true });
            }
            catch { }
            return;
        }

        if (result != ContentDialogResult.Primary) return;

        var selectedStyle = styleCombo.SelectedItem?.ToString() ?? "Fill";

        // File picker
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.FileTypeFilter.Add(".bmp");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");

        // Initialize picker with window handle
        var window = App.m_window;
        if (window == null) return;

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
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

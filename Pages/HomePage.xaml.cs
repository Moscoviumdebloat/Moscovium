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

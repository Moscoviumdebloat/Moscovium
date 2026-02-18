using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Helpers;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System;

namespace MoscoviumThree.Pages;

public sealed partial class CS2Page : Page
{
    private static readonly HttpClient _httpClient = new();

    public CS2Page()
    {
        this.InitializeComponent();
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

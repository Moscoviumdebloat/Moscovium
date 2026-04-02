using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MoscoviumThree.Helpers;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace MoscoviumThree.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        try
        {
            // Load settings
            if (SettingsHelper.AppsInstallPath != null)
            {
                PathTextBox.Text = SettingsHelper.AppsInstallPath;
            }
            
             AutoUpdateAppsToggle.IsOn = SettingsHelper.AutoUpdateApps;
            AutoUpdateMoscoviumToggle.IsOn = SettingsHelper.AutoUpdateMoscovium;
            GitHubTokenBox.Password = SettingsHelper.GitHubToken ?? "";

            // Set version text to dynamic version
            var versionText = (FindName("TxtAppVersion") as TextBlock);
            if (versionText != null)
            {
                versionText.Text = $"Moscovium v{UpdateService.GetCurrentVersion()}";
            }
        }
        catch (System.Exception ex)
        {
             // Log
             string logPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "MoscoviumSettingsCrash.txt");
             System.IO.File.WriteAllText(logPath, ex.ToString());
        }
    }

    private async void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var windowHash = WindowNative.GetWindowHandle(App.m_window);
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");
            InitializeWithWindow.Initialize(folderPicker, windowHash);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                SettingsHelper.AppsInstallPath = folder.Path;
                PathTextBox.Text = folder.Path;
            }
        }
        catch { }
    }

    private void AutoUpdateAppsToggle_Toggled(object sender, RoutedEventArgs e)
    {
        SettingsHelper.AutoUpdateApps = AutoUpdateAppsToggle.IsOn;
    }

    private void AutoUpdateMoscoviumToggle_Toggled(object sender, RoutedEventArgs e)
    {
        SettingsHelper.AutoUpdateMoscovium = AutoUpdateMoscoviumToggle.IsOn;
    }

    private void GitHubTokenBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        SettingsHelper.GitHubToken = GitHubTokenBox.Password;
    }

    private async void BtnCheckForUpdates_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BtnCheckForUpdates.IsEnabled = false;
            TxtUpdateStatus.Text = "Checking for updates...";
            UpdateProgressBar.Visibility = Visibility.Collapsed;
            UpdateProgressBar.Value = 0;

            var update = await UpdateService.CheckForUpdateAsync();

            if (update == null)
            {
                TxtUpdateStatus.Text = "Moscovium is up to date.";
                BtnCheckForUpdates.IsEnabled = true;
                return;
            }

            TxtUpdateStatus.Text = $"Downloading v{update.TargetFullRelease.Version}...";
            UpdateProgressBar.Visibility = Visibility.Visible;

            await UpdateService.DownloadUpdateAsync(update, progress =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    UpdateProgressBar.Value = progress;
                });
            });

            TxtUpdateStatus.Text = "Update downloaded. Restarting...";

            // Confirm restart prompt
            var dialog = new ContentDialog
            {
                Title = "Update Ready",
                Content = $"Version {update.TargetFullRelease.Version} has been downloaded.\nWould you like to restart Moscovium now to apply the update?",
                PrimaryButtonText = "Restart Now",
                SecondaryButtonText = "Later",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                UpdateService.ApplyUpdateAndRestart(update);
            }
            else
            {
                TxtUpdateStatus.Text = "Update ready to install.";
                BtnCheckForUpdates.IsEnabled = true;
            }
        }
        catch (System.Exception ex)
        {
            TxtUpdateStatus.Text = "Error checking for updates.";
            BtnCheckForUpdates.IsEnabled = true;
            System.Diagnostics.Debug.WriteLine($"[Settings] Update click failed: {ex.Message}");
        }
    }
}

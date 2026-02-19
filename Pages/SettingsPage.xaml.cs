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
}

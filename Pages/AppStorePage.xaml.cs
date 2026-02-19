using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MoscoviumThree.Helpers;
using MoscoviumThree.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Compression;

namespace MoscoviumThree.Pages;

public sealed partial class AppStorePage : Page
{
    public ObservableCollection<AppStoreItem> Apps { get; } = new();
    private List<AppStoreItem> _allApps = new();
    private readonly GitHubService _gitHubService;

    public AppStorePage()
    {
        this.InitializeComponent();
        _gitHubService = new GitHubService();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        try
        {
            if (_allApps.Count == 0)
            {
                await LoadAppsAsync();
            }
        }
        catch (Exception ex)
        {
             string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MoscoviumStoreCrash.txt");
             File.WriteAllText(logPath, ex.ToString());
        }
    }

    private async Task LoadAppsAsync()
    {
        LoadingRing.IsActive = true;
        LoadingRing.Visibility = Visibility.Visible;
        Apps.Clear();
        _allApps.Clear();

        try
        {
            // Fetch from both organizations
            var betterApps = await _gitHubService.GetAppsFromOrgAsync("Better-Dev-Team");
            var adApps = await _gitHubService.GetAppsFromOrgAsync("Anti-Depressants-Dev-Team");

            _allApps.AddRange(betterApps);
            _allApps.AddRange(adApps);

            // Check installed status
            var installPath = SettingsHelper.AppsInstallPath;
            foreach (var app in _allApps)
            {
                // Simple check: if folder with app name exists
                if (Directory.Exists(Path.Combine(installPath, app.Name)))
                {
                    app.IsInstalled = true;
                    app.Status = "Installed";
                }
            }

            UpdateFilteredList();
        }
        catch (Exception ex)
        {
             // Log error or show dialog
             Debug.WriteLine($"Error loading apps: {ex.Message}");
             string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MoscoviumAppsLoadError.txt");
             File.WriteAllText(logPath, ex.ToString());
             
             // Show error in UI
             var tokenStatus = string.IsNullOrEmpty(SettingsHelper.GitHubToken) ? "No Token" : "Token Present";
             ErrorMessageTextBlock.Text = $"Error ({tokenStatus}): {ex.Message}";
             ErrorMessageTextBlock.Visibility = Visibility.Visible;
        }
        finally
        {
            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
            
            if (Apps.Count == 0)
            {
                EmptyStatePanel.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyStatePanel.Visibility = Visibility.Collapsed;
                ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateFilteredList();
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadAppsAsync();
    }

    private void UpdateFilteredList()
    {
        if (AppsGridView == null || FilterComboBox == null) return;

        if (FilterComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is string tag)
        {
            Apps.Clear();
            IEnumerable<AppStoreItem> filtered = tag switch
            {
                "better" => _allApps.Where(a => a.Author == "Better-Dev-Team"),
                "antidepressants" => _allApps.Where(a => a.Author == "Anti-Depressants-Dev-Team"),
                _ => _allApps
            };

            foreach (var app in filtered)
            {
                Apps.Add(app);
            }
            
            // Re-bind GridView source if needed, though ObservableCollection should handle it.
            // But since we clear and add, it might be flickering. 
            // Better to set ItemsSource directly? 
            // WinUI x:Bind to ObservableCollection is usually fine.
            AppsGridView.ItemsSource = Apps;
        }
    }

    private async void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is AppStoreItem app)
        {
            if (string.IsNullOrEmpty(app.DownloadUrl))
            {
                app.Status = "No download available";
                return;
            }

            app.IsDownloading = true;
            app.Status = "Downloading...";
            app.DownloadProgress = 0;

            try
            {
                var downloadPath = SettingsHelper.AppsInstallPath;
                Directory.CreateDirectory(downloadPath);

                var fileName = Path.GetFileName(new Uri(app.DownloadUrl).LocalPath);
                var activeFilePath = Path.Combine(downloadPath, fileName);

                using (var client = new HttpClient())
                {
                    // Basic progress reporting
                    using (var response = await client.GetAsync(app.DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                        var canReportProgress = totalBytes != -1;

                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(activeFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            var buffer = new byte[8192];
                            var bytesRead = 0;
                            var totalRead = 0L;

                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalRead += bytesRead;

                                if (canReportProgress)
                                {
                                    app.DownloadProgress = (double)totalRead / totalBytes * 100;
                                }
                            }
                        }
                    }
                }

                app.Status = "Installing...";
                // Install Logic
                if (activeFilePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    var extractPath = Path.Combine(downloadPath, Path.GetFileNameWithoutExtension(fileName));
                    if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);
                    ZipFile.ExtractToDirectory(activeFilePath, extractPath);
                    
                    // Try to find an exe inside to launch? Or just open folder
                    var exe = Directory.GetFiles(extractPath, "*.exe").FirstOrDefault();
                    if (exe != null)
                    {
                        Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true });
                    }
                    else
                    {
                        Process.Start(new ProcessStartInfo(extractPath) { UseShellExecute = true });
                    }
                }
                else if (activeFilePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || activeFilePath.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo(activeFilePath) { UseShellExecute = true });
                }

                app.Status = "Installed";
                app.IsInstalled = true;
            }
            catch (Exception ex)
            {
                app.Status = "Error";
                Debug.WriteLine($"Install failed: {ex.Message}");
            }
            finally
            {
                app.IsDownloading = false;
            }
        }
    }

    private void UninstallButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is AppStoreItem app)
        {
             try
             {
                 var downloadPath = SettingsHelper.AppsInstallPath;
                 // Assuming folder name matches repo name or some convention. 
                 // For now, let's guess based on app Name or similar.
                 // In Install we used zip filename. To be robust, we should store install path in the item if possible,
                 // but for now let's try to delete the probable folder.
                 
                 // Ideally we'd scan for the folder.
                 // Let's assume folder name = App Name (cleaned up) or we need a better tracking system.
                 // Simplification: Check if a folder starting with app Name exists?
                 
                 // Better approach for zip installs:
                 var probablePath = Path.Combine(downloadPath, app.Name);
                 if (Directory.Exists(probablePath))
                 {
                     Directory.Delete(probablePath, true);
                 }
                 else
                 {
                     // Try finding close match or zip name... tricky without persisting metadata.
                     // Fallback: Notify user to delete manually for now if not found.
                     app.Status = "Manual delete required";
                     return;
                 }
                 
                 app.IsInstalled = false;
                 app.Status = "Available";
             }
             catch
             {
                 app.Status = "Uninstall failed";
             }
        }
    }
}

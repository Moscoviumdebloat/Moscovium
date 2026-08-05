using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Compression;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MoscoviumThree.Helpers;
using MoscoviumThree.Models;
using Windows.Storage.Pickers;

namespace MoscoviumThree.Pages;

public sealed partial class HomePage : Page
{
    private const string GlyphRunning = "\uE895";
    private const string GlyphSuccess = "\uE73E";
    private const string GlyphFailed = "\uEA39";
    private const string GlyphSkipped = "\uE738";
    private const string GlyphInfo = "\uE946";

    private static readonly SolidColorBrush BrushRunning = new(Microsoft.UI.Colors.Gray);
    private static readonly SolidColorBrush BrushSuccess = new(Microsoft.UI.Colors.LightGreen);
    private static readonly SolidColorBrush BrushFailed = new(Microsoft.UI.Colors.LightCoral);
    private static readonly SolidColorBrush BrushSkipped = new(Microsoft.UI.Colors.Khaki);
    private static readonly SolidColorBrush BrushInfo = new(Microsoft.UI.Colors.Silver);

    private sealed class LogEntry
    {
        public string Text { get; set; } = "";
        public string Glyph { get; set; } = GlyphInfo;
        public SolidColorBrush GlyphColor { get; set; } = BrushInfo;
    }

    public HomePage()
    {
        this.InitializeComponent();
        Loaded += HomePage_Loaded;
    }

    private void HomePage_Loaded(object sender, RoutedEventArgs e)
    {
        var profilePath = App.LaunchSetupProfilePath;
        if (profilePath == null) return;
        App.LaunchSetupProfilePath = null;

        DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                var profile = profilePath.Length == 0
                    ? SetupProfileStore.LoadDefault()
                    : SetupProfileStore.Load(profilePath);

                if (profile == null)
                {
                    await ShowError($"Setup profile not found: \"{profilePath}\".\nOpen Moscovium and configure the PC Setup Automation first.");
                    return;
                }

                await RunSetupAsync(profile);
            }
            catch (Exception ex)
            {
                await ShowError($"Setup failed: {ex.Message}");
            }
        });
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

        var profile = setupControl.GetProfile();
        SetupProfileStore.SaveDefault(profile);
        await RunSetupAsync(profile);
    }

    /// <summary>
    /// Executes a setup profile with a live log and result summary.
    /// Used by both the home button and the --setup command line.
    /// </summary>
    public async Task RunSetupAsync(SetupProfile profile)
    {
        var log = new ObservableCollection<LogEntry>();
        var headerText = new TextBlock { Text = "Starting automation...", TextWrapping = TextWrapping.Wrap };
        var progressBar = new ProgressBar { IsIndeterminate = true, Margin = new Thickness(0, 0, 0, 8) };

        var listControl = new ItemsControl
        {
            ItemsSource = log,
            ItemTemplate = (DataTemplate)Resources["LogEntryTemplate"]
        };
        var scrollViewer = new ScrollViewer
        {
            Content = listControl,
            MaxHeight = 320,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        var panel = new StackPanel { Spacing = 8, Width = 640 };
        panel.Children.Add(headerText);
        panel.Children.Add(progressBar);
        panel.Children.Add(scrollViewer);

        var progressDialog = new ContentDialog
        {
            Title = "Automating Setup...",
            Content = panel,
            XamlRoot = this.XamlRoot
        };

        _ = progressDialog.ShowAsync();

        var logLines = new List<string> { $"[{DateTime.Now:HH:mm:ss}] Setup automation started" };
        var failedApps = new List<string>();
        var countersLock = new object();
        int installedCount = 0, skippedCount = 0, failedCount = 0;

        void AppendLogLine(string text)
        {
            lock (countersLock)
            {
                logLines.Add($"[{DateTime.Now:HH:mm:ss}] {text}");
            }
        }

        void UpdateEntry(LogEntry entry, string text, string glyph, SolidColorBrush brush)
        {
            entry.Text = text;
            entry.Glyph = glyph;
            entry.GlyphColor = brush;
            DispatcherQueue.TryEnqueue(() =>
            {
                log[log.IndexOf(entry)] = entry;
                scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            });
        }

        void AddInfo(string text)
        {
            var entry = new LogEntry { Text = text, Glyph = GlyphInfo, GlyphColor = BrushInfo };
            DispatcherQueue.TryEnqueue(() =>
            {
                log.Add(entry);
                scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            });
            AppendLogLine(text);
        }

        void MarkRunning(LogEntry entry, string text) => UpdateEntry(entry, text, GlyphRunning, BrushRunning);
        void MarkSuccess(LogEntry entry, string text) => UpdateEntry(entry, text, GlyphSuccess, BrushSuccess);
        void MarkFailed(LogEntry entry, string text)
        {
            UpdateEntry(entry, text, GlyphFailed, BrushFailed);
            AppendLogLine($"FAILED: {text}");
        }
        void MarkSkipped(LogEntry entry, string text) => UpdateEntry(entry, text, GlyphSkipped, BrushSkipped);

        void AppendFailed(string text)
        {
            var entry = new LogEntry { Text = text, Glyph = GlyphFailed, GlyphColor = BrushFailed };
            DispatcherQueue.TryEnqueue(() =>
            {
                log.Add(entry);
                scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            });
            AppendLogLine($"FAILED: {text}");
        }

        try
        {
            // 0. Winget availability (fresh installs may be missing it)
            if (!SetupAutomationHelper.IsWingetAvailable())
            {
                AddInfo("winget is not available on this system.");
                AppendFailed("winget not found - install the App Installer from the Microsoft Store and retry.");
                progressDialog.Hide();
                await ShowSummaryAsync(installedCount, skippedCount, failedCount, failedApps, logLines);
                return;
            }

            // 1. Windows Updates
            if (profile.RunWindowsUpdate)
            {
                var entry = new LogEntry();
                DispatcherQueue.TryEnqueue(() => log.Add(entry));
                MarkRunning(entry, "Running Windows Update (window may open)...");
                if (await SetupAutomationHelper.RunWindowsUpdateAsync())
                {
                    MarkSuccess(entry, "Windows Update finished. Restart may be required.");
                }
                else
                {
                    MarkFailed(entry, "Windows Update failed or was cancelled.");
                    failedApps.Add("Windows Update");
                }
            }

            // 2. Refresh winget sources
            AddInfo("Refreshing winget package sources...");
            var sourcesOk = await SetupAutomationHelper.UpdateWingetSourcesAsync();
            AddInfo(sourcesOk ? "Winget sources up to date." : "Winget source update failed (continuing anyway).");

            // 3. Upgrade existing apps
            if (profile.UpgradeAllApps)
            {
                AddInfo("Upgrading all existing winget apps...");
                var upgradeOk = await SetupAutomationHelper.UpgradeAllAppsAsync();
                AddInfo(upgradeOk ? "App upgrades finished." : "App upgrade run finished with warnings.");
            }

            // 4. Apps: skip already installed, then install in parallel
            var pending = new List<SetupApp>();
            var checkEntries = new Dictionary<string, LogEntry>();

            foreach (var id in profile.WingetApps)
            {
                var app = SetupCatalog.FindById(id) ?? new SetupApp(id, id, id, "");
                var entry = new LogEntry { Text = $"Checking {app.Name}...", Glyph = GlyphRunning, GlyphColor = BrushRunning };
                DispatcherQueue.TryEnqueue(() => log.Add(entry));
                checkEntries[id] = entry;
                pending.Add(app);
            }

            await Parallel.ForEachAsync(pending, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (app, _) =>
            {
                var entry = checkEntries[app.Id];
                if (await SetupAutomationHelper.IsAppInstalledAsync(app))
                {
                    lock (countersLock) skippedCount++;
                    MarkSkipped(entry, $"{app.Name} - already installed, skipped");
                }
                else
                {
                    MarkRunning(entry, $"Installing {app.Name}...");
                    if (await SetupAutomationHelper.InstallAppAsync(app))
                    {
                        lock (countersLock) installedCount++;
                        MarkSuccess(entry, $"{app.Name} - installed");
                    }
                    else
                    {
                        lock (countersLock)
                        {
                            failedCount++;
                            failedApps.Add(app.Name);
                        }
                        MarkFailed(entry, $"{app.Name} - install failed");
                    }
                }
            });

            // 5. Debloat tweaks
            if (profile.Tweaks.Count > 0)
            {
                AddInfo($"Applying {profile.Tweaks.Count} debloat tweak(s)...");
                var appliedTweaks = 0;
                var failedTweaks = new List<string>();

                foreach (var tweakName in profile.Tweaks)
                {
                    var tweak = TweakCatalog.FindTweak(tweakName);
                    if (tweak == null)
                    {
                        failedTweaks.Add(tweakName);
                        continue;
                    }

                    var entry = new LogEntry { Text = $"Applying {tweak.Name}...", Glyph = GlyphRunning, GlyphColor = BrushRunning };
                    DispatcherQueue.TryEnqueue(() => log.Add(entry));

                    var failedValues = new List<string>();
                    var ok = await Task.Run(() => TweakHelper.ApplyTweak(tweak, failedValues));

                    if (ok)
                    {
                        appliedTweaks++;
                        MarkSuccess(entry, $"{tweak.Name} - done");
                    }
                    else
                    {
                        lock (countersLock) failedTweaks.Add(tweak.Name);
                        MarkFailed(entry, $"{tweak.Name} - failed ({(failedValues.Count > 0 ? string.Join("; ", failedValues) : "unknown error")})");
                    }
                }

                AddInfo($"Tweaks applied: {appliedTweaks}, failed: {failedTweaks.Count}");
                lock (countersLock)
                {
                    failedCount += failedTweaks.Count;
                    failedApps.AddRange(failedTweaks);
                }
            }

            // 6. VC++ Runtimes
            if (profile.InstallVCRuntimes)
            {
                AddInfo("Installing Visual C++ Runtimes...");
                try
                {
                    var extractPath = ResourceHelper.ExtractZipToTemp("Visual-C-Runtimes-All-in-One-Nov-2025.zip", "VisualCDistributables");
                    ProcessHelper.Run(Path.Combine(extractPath, "install_all.bat"));
                    AddInfo("VC++ runtime installer launched.");
                }
                catch (Exception ex)
                {
                    failedApps.Add("VC++ Runtimes");
                    AddInfo($"VC++ runtime install failed: {ex.Message}");
                }
            }

            // 7. External debloat tools (launch externally)
            if (profile.RunChrisTitus)
            {
                AddInfo("Launching Chris Titus WinUtil...");
                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"irm christitus.com/win | iex\"",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(startInfo);
            }

            if (profile.RunRaphi)
            {
                AddInfo("Launching Raphi Debloat...");
                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"iwr -useb https://win11debloat.raphire.com/ | iex\"",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(startInfo);
            }

            AddInfo("Setup automation finished.");
        }
        catch (Exception ex)
        {
            AppendFailed($"Unexpected error: {ex.Message}");
        }

        progressDialog.Hide();
        await ShowSummaryAsync(installedCount, skippedCount, failedCount, failedApps, logLines);
    }

    private async Task ShowSummaryAsync(int installedCount, int skippedCount, int failedCount, List<string> failedApps, List<string> logLines)
    {
        var summary = new System.Text.StringBuilder();
        summary.AppendLine("Setup completed.");
        summary.AppendLine();
        summary.AppendLine($"Installed: {installedCount}");
        summary.AppendLine($"Already installed: {skippedCount}");
        summary.AppendLine($"Failed: {failedCount}");

        if (failedApps.Count > 0)
        {
            summary.AppendLine();
            summary.AppendLine("Failed items:");
            foreach (var app in failedApps)
            {
                summary.AppendLine($"  - {app}");
            }
        }

        summary.AppendLine();
        summary.AppendLine("Note: Some system tweaks or Windows Updates may require a manual restart to take effect.");

        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Moscovium",
            $"setup-log-{DateTime.Now:yyyyMMdd-HHmmss}.txt");
        try
        {
            File.WriteAllLines(logPath, logLines);
            summary.AppendLine();
            summary.AppendLine($"Full log: {logPath}");
        }
        catch { }

        await ShowInfo(summary.ToString());
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
        var picker = new FileOpenPicker();
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

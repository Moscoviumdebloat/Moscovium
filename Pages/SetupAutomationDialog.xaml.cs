using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Helpers;
using MoscoviumThree.Models;
using Windows.Storage.Pickers;

namespace MoscoviumThree.Pages;

public sealed partial class SetupAutomationDialog : UserControl
{
    private readonly List<CheckBox> _appCheckBoxes = new();
    private readonly List<CheckBox> _systemTweakCheckBoxes = new();
    private readonly List<CheckBox> _debloatTweakCheckBoxes = new();

    public SetupAutomationDialog()
    {
        this.InitializeComponent();
        BuildAppList();

        Loaded += (_, _) => ApplyProfile(SetupProfileStore.LoadDefault());
    }

    private void BuildAppList()
    {
        // System tweaks group (kept at the top)
        var systemTweaks = new (string Content, string Key)[]
        {
            ("Run Windows Update (Forces check & install)", nameof(SetupProfile.RunWindowsUpdate)),
            ("Update existing apps (winget upgrade --all)", nameof(SetupProfile.UpgradeAllApps)),
            ("Install All Visual C++ Runtimes", nameof(SetupProfile.InstallVCRuntimes)),
            ("Run Chris Titus WinUtil (Debloat)", nameof(SetupProfile.RunChrisTitus)),
            ("Run Win11 Debloat (Raphi)", nameof(SetupProfile.RunRaphi)),
        };

        foreach (var (content, key) in systemTweaks)
        {
            var checkBox = new CheckBox { Content = content, Tag = key };
            _systemTweakCheckBoxes.Add(checkBox);
            AppsPanel.Children.Add(checkBox);
        }

        // App categories
        foreach (var category in SetupCatalog.Categories)
        {
            var header = new TextBlock
            {
                Text = category,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 8, 0, 0)
            };
            AppsPanel.Children.Add(header);

            foreach (var app in SetupCatalog.Apps.Where(a => a.Category == category))
            {
                var checkBox = new CheckBox { Content = app.Name, Tag = app.Id };
                _appCheckBoxes.Add(checkBox);
                AppsPanel.Children.Add(checkBox);
            }
        }

        // Debloat tweaks group (at the bottom)
        var tweaksHeader = new Grid { Margin = new Thickness(0, 8, 0, 0) };
        tweaksHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        tweaksHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        tweaksHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var tweaksTitle = new TextBlock { Text = "Debloat Tweaks", FontWeight = FontWeights.SemiBold, VerticalAlignment = VerticalAlignment.Center };
        var selectAllButton = new Button { Content = "Select All", Margin = new Thickness(8, 0, 8, 0), Padding = new Thickness(8, 4, 8, 4) };
        selectAllButton.Click += (_, _) =>
        {
            foreach (var cb in _debloatTweakCheckBoxes) cb.IsChecked = true;
        };
        var clearButton = new Button { Content = "Clear", Padding = new Thickness(8, 4, 8, 4) };
        clearButton.Click += (_, _) =>
        {
            foreach (var cb in _debloatTweakCheckBoxes) cb.IsChecked = false;
        };

        Grid.SetColumn(selectAllButton, 1);
        Grid.SetColumn(clearButton, 2);
        tweaksHeader.Children.Add(tweaksTitle);
        tweaksHeader.Children.Add(selectAllButton);
        tweaksHeader.Children.Add(clearButton);
        AppsPanel.Children.Add(tweaksHeader);

        foreach (var tweak in TweakCatalog.Tweaks)
        {
            var checkBox = new CheckBox { Content = tweak.Name, Tag = tweak.Name };
            _debloatTweakCheckBoxes.Add(checkBox);
            AppsPanel.Children.Add(checkBox);
        }
    }

    public SetupProfile GetProfile()
    {
        var profile = new SetupProfile();
        foreach (var cb in _appCheckBoxes)
        {
            if (cb.IsChecked == true && cb.Tag is string appId && appId.Length > 0)
            {
                profile.WingetApps.Add(appId);
            }
        }

        foreach (var cb in _debloatTweakCheckBoxes)
        {
            if (cb.IsChecked == true && cb.Tag is string tweakName)
            {
                profile.Tweaks.Add(tweakName);
            }
        }

        profile.RunWindowsUpdate = GetSystemTweak(nameof(SetupProfile.RunWindowsUpdate));
        profile.UpgradeAllApps = GetSystemTweak(nameof(SetupProfile.UpgradeAllApps));
        profile.InstallVCRuntimes = GetSystemTweak(nameof(SetupProfile.InstallVCRuntimes));
        profile.RunChrisTitus = GetSystemTweak(nameof(SetupProfile.RunChrisTitus));
        profile.RunRaphi = GetSystemTweak(nameof(SetupProfile.RunRaphi));
        return profile;
    }

    public void ApplyProfile(SetupProfile? profile)
    {
        if (profile == null)
        {
            SetProfileStatus("No saved profile.");
            return;
        }

        var selectedApps = new HashSet<string>(profile.WingetApps, System.StringComparer.OrdinalIgnoreCase);
        foreach (var cb in _appCheckBoxes)
        {
            cb.IsChecked = cb.Tag is string id && selectedApps.Contains(id);
        }

        var selectedTweaks = new HashSet<string>(profile.Tweaks, System.StringComparer.OrdinalIgnoreCase);
        foreach (var cb in _debloatTweakCheckBoxes)
        {
            cb.IsChecked = cb.Tag is string name && selectedTweaks.Contains(name);
        }

        SetSystemTweak(nameof(SetupProfile.RunWindowsUpdate), profile.RunWindowsUpdate);
        SetSystemTweak(nameof(SetupProfile.UpgradeAllApps), profile.UpgradeAllApps);
        SetSystemTweak(nameof(SetupProfile.InstallVCRuntimes), profile.InstallVCRuntimes);
        SetSystemTweak(nameof(SetupProfile.RunChrisTitus), profile.RunChrisTitus);
        SetSystemTweak(nameof(SetupProfile.RunRaphi), profile.RunRaphi);

        SetProfileStatus("Loaded saved profile.");
    }

    private bool GetSystemTweak(string key) =>
        _systemTweakCheckBoxes.FirstOrDefault(cb => cb.Tag as string == key)?.IsChecked == true;

    private void SetSystemTweak(string key, bool value)
    {
        var cb = _systemTweakCheckBoxes.FirstOrDefault(c => c.Tag as string == key);
        if (cb != null) cb.IsChecked = value;
    }

    private void SetProfileStatus(string text)
    {
        ProfileStatusText.Text = text;
    }

    private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var cb in _appCheckBoxes)
        {
            cb.IsChecked = true;
        }
        SetProfileStatus($"All {_appCheckBoxes.Count} apps selected.");
    }

    private void BtnClearApps_Click(object sender, RoutedEventArgs e)
    {
        foreach (var cb in _appCheckBoxes)
        {
            cb.IsChecked = false;
        }
        SetProfileStatus("All apps cleared.");
    }

    private async void BtnSaveProfile_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.Downloads,
            SuggestedFileName = "moscovium-setup-profile"
        };
        picker.FileTypeChoices.Add("Moscovium Setup Profile", new List<string> { ".json" });
        InitializePicker(picker);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        SetupProfileStore.Save(file.Path, GetProfile());
        SetProfileStatus($"Profile saved: {System.IO.Path.GetFileName(file.Path)}");
    }

    private async void BtnLoadProfile_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.Downloads };
        picker.FileTypeFilter.Add(".json");
        InitializePicker(picker);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        var profile = SetupProfileStore.Load(file.Path);
        if (profile == null)
        {
            SetProfileStatus("Failed to load profile.");
            return;
        }

        ApplyProfile(profile);
    }

    private static void InitializePicker(object picker)
    {
        var window = App.m_window;
        if (window == null) return;
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
    }
}

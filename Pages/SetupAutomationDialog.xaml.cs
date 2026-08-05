using System;
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
    private readonly List<CheckBox> _systemTweakCheckBoxes = new();
    private readonly Dictionary<string, GroupState> _groups = new();

    public SetupAutomationDialog()
    {
        this.InitializeComponent();
        BuildGroups();
        RefreshCounters();

        Loaded += (_, _) => ApplyProfile(SetupProfileStore.LoadDefault());
    }

    private void BuildGroups()
    {
        AddSystemGroup();

        foreach (var category in SetupCatalog.Categories)
            AddAppGroup(category);

        foreach (var category in TweakCatalog.Categories)
            AddTweakGroup(category);
    }

    private void AddSystemGroup()
    {
        var items = new (string Content, string Description, string Key)[]
        {
            ("Run Windows Update", "Forces a check and installs all pending Windows updates", nameof(SetupProfile.RunWindowsUpdate)),
            ("Update existing apps", "Runs winget upgrade --all", nameof(SetupProfile.UpgradeAllApps)),
            ("Install All Visual C++ Runtimes", "Installs every Microsoft VC++ redistributable", nameof(SetupProfile.InstallVCRuntimes)),
            ("Run Chris Titus WinUtil", "Debloats Windows using Chris Titus's utility", nameof(SetupProfile.RunChrisTitus)),
            ("Run Win11 Debloat (Raphi)", "Debloats Windows using Raphi's script", nameof(SetupProfile.RunRaphi)),
        };

        var state = new GroupState { Category = "System", Counter = MakeCounter() };
        foreach (var (content, description, key) in items)
        {
            var cb = MakeCheckBox(content, description, () => RefreshCounters());
            cb.Tag = key;
            _systemTweakCheckBoxes.Add(cb);
            Place(state, cb);
        }
        FinishGroup(state, "System");
    }

    private void AddAppGroup(string category)
    {
        var state = new GroupState { Category = category, Counter = MakeCounter() };
        foreach (var app in SetupCatalog.Apps.Where(a => a.Category == category))
        {
            var cb = MakeCheckBox(app.Name, app.Description, () => RefreshCounters());
            cb.Tag = app.Id;
            Place(state, cb);
        }
        FinishGroup(state, category);
    }

    private void AddTweakGroup(string category)
    {
        var state = new GroupState { Category = "Tweaks - " + category, Counter = MakeCounter() };
        foreach (var tweak in TweakCatalog.Tweaks.Where(t => t.Category == category))
        {
            var cb = MakeCheckBox(tweak.Name, tweak.Description, () => RefreshCounters());
            cb.Tag = tweak.Name;
            Place(state, cb);
        }
        FinishGroup(state, "Tweaks - " + category);
    }

    private static TextBlock MakeCounter() =>
        new() { FontSize = 12, VerticalAlignment = VerticalAlignment.Center };

    private static CheckBox MakeCheckBox(string title, string? description, Action onToggle)
    {
        var nameText = new TextBlock { Text = title, FontWeight = FontWeights.SemiBold };
        var content = new StackPanel();
        content.Children.Add(nameText);
        if (!string.IsNullOrEmpty(description))
        {
            content.Children.Add(new TextBlock
            {
                Text = description,
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Opacity = 0.7,
                Margin = new Thickness(0, 2, 0, 0)
            });
        }

        var cb = new CheckBox { Content = content, Margin = new Thickness(0, 2, 0, 2) };
        cb.Checked += (_, _) => onToggle();
        cb.Unchecked += (_, _) => onToggle();
        return cb;
    }

    private static void Place(GroupState state, FrameworkElement element)
    {
        if (state.Grid.ColumnDefinitions.Count == 0)
        {
            state.Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            state.Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
        while (state.Grid.RowDefinitions.Count <= state.NextRow)
            state.Grid.RowDefinitions.Add(new RowDefinition());

        int row = state.NextRow;
        int col = state.NextColumn;
        Grid.SetRow(element, row);
        Grid.SetColumn(element, col);
        state.Grid.Children.Add(element);
        state.CheckBoxes.Add((CheckBox)element);
        if (col == 0)
        {
            state.NextColumn = 1;
        }
        else
        {
            state.NextColumn = 0;
            state.NextRow++;
        }
    }

    private void FinishGroup(GroupState state, string header)
    {
        var selectAll = new Button { Content = "Select All", Padding = new Thickness(8, 2, 8, 2) };
        var clear = new Button { Content = "Clear", Padding = new Thickness(8, 2, 8, 2) };
        selectAll.Click += (_, _) =>
        {
            foreach (var cb in state.CheckBoxes) cb.IsChecked = true;
            RefreshCounters();
        };
        clear.Click += (_, _) =>
        {
            foreach (var cb in state.CheckBoxes) cb.IsChecked = false;
            RefreshCounters();
        };

        var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        headerPanel.Children.Add(new TextBlock { Text = header, FontWeight = FontWeights.SemiBold, VerticalAlignment = VerticalAlignment.Center });
        headerPanel.Children.Add(state.Counter);
        headerPanel.Children.Add(selectAll);
        headerPanel.Children.Add(clear);

        var expander = new Expander { Header = headerPanel, Content = state.Grid, IsExpanded = true, Margin = new Thickness(0, 0, 0, 4) };
        state.Expander = expander;
        _groups[state.Category] = state;
        AppsPanel.Children.Add(expander);
    }

    public SetupProfile GetProfile()
    {
        var profile = new SetupProfile();
        foreach (var group in _groups.Values)
        {
            foreach (var cb in group.CheckBoxes)
            {
                if (cb.IsChecked != true || cb.Tag is not string tag) continue;
                if (group.IsTweakGroup)
                    profile.Tweaks.Add(tag);
                else
                    profile.WingetApps.Add(tag);
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

        foreach (var group in _groups.Values)
        {
            foreach (var cb in group.CheckBoxes)
            {
                if (cb.Tag is not string tag) continue;
                var selected = group.IsTweakGroup
                    ? profile.Tweaks.Contains(tag, System.StringComparer.OrdinalIgnoreCase)
                    : profile.WingetApps.Contains(tag, System.StringComparer.OrdinalIgnoreCase);
                cb.IsChecked = selected;
            }
        }

        SetSystemTweak(nameof(SetupProfile.RunWindowsUpdate), profile.RunWindowsUpdate);
        SetSystemTweak(nameof(SetupProfile.UpgradeAllApps), profile.UpgradeAllApps);
        SetSystemTweak(nameof(SetupProfile.InstallVCRuntimes), profile.InstallVCRuntimes);
        SetSystemTweak(nameof(SetupProfile.RunChrisTitus), profile.RunChrisTitus);
        SetSystemTweak(nameof(SetupProfile.RunRaphi), profile.RunRaphi);

        RefreshCounters();
        SetProfileStatus("Loaded saved profile.");
    }

    private bool GetSystemTweak(string key) =>
        _systemTweakCheckBoxes.FirstOrDefault(cb => cb.Tag as string == key)?.IsChecked == true;

    private void SetSystemTweak(string key, bool value)
    {
        var cb = _systemTweakCheckBoxes.FirstOrDefault(c => c.Tag as string == key);
        if (cb != null) cb.IsChecked = value;
    }

    private void RefreshCounters()
    {
        int total = 0;
        foreach (var group in _groups.Values)
        {
            int selected = group.CheckBoxes.Count(cb => cb.IsChecked == true);
            total += selected;
            group.Counter.Text = $"{selected} of {group.CheckBoxes.Count} selected";
        }
        SelectionSummaryText.Text = total == 0
            ? "Nothing selected yet."
            : $"{total} item(s) selected.";
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string query = SearchBox.Text.Trim().ToLowerInvariant();
        foreach (var group in _groups.Values)
        {
            bool any = false;
            foreach (var cb in group.CheckBoxes)
            {
                if (query.Length == 0)
                {
                    cb.Visibility = Visibility.Visible;
                    any = true;
                    continue;
                }
                bool match = (cb.Content as StackPanel)?.Children
                    .OfType<TextBlock>()
                    .Any(t => t.Text.ToLowerInvariant().Contains(query)) == true;
                cb.Visibility = match ? Visibility.Visible : Visibility.Collapsed;
                if (match) any = true;
            }
            group.Expander.Visibility = any ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var group in _groups.Values)
            foreach (var cb in group.CheckBoxes)
                cb.IsChecked = true;
        RefreshCounters();
    }

    private void BtnClearApps_Click(object sender, RoutedEventArgs e)
    {
        foreach (var group in _groups.Values)
            foreach (var cb in group.CheckBoxes)
                cb.IsChecked = false;
        RefreshCounters();
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

    private void SetProfileStatus(string text)
    {
        ProfileStatusText.Text = text;
    }

    private static void InitializePicker(object picker)
    {
        var window = App.m_window;
        if (window == null) return;
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
    }

    private class GroupState
    {
        public string Category { get; set; } = string.Empty;
        public TextBlock Counter { get; set; } = null!;
        public Expander Expander { get; set; } = null!;
        public bool IsTweakGroup => Category.StartsWith("Tweaks - ");
        public int NextRow { get; set; }
        public int NextColumn { get; set; }
        public List<CheckBox> CheckBoxes { get; } = new();
        public Grid Grid { get; } = new() { ColumnSpacing = 12, RowSpacing = 2 };
    }
}

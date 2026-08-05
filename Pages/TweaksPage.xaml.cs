using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MoscoviumThree.Helpers;
using MoscoviumThree.Models;

namespace MoscoviumThree.Pages;

public sealed partial class TweaksPage : Page
{
    private const string GlyphSuccess = "\uE73E";
    private const string GlyphFailed = "\uEA39";
    private const string GlyphRunning = "\uE895";
    private const string GlyphInfo = "\uE946";

    private static readonly SolidColorBrush BrushSuccess = new(Microsoft.UI.Colors.LightGreen);
    private static readonly SolidColorBrush BrushFailed = new(Microsoft.UI.Colors.LightCoral);
    private static readonly SolidColorBrush BrushRunning = new(Microsoft.UI.Colors.Gray);
    private static readonly SolidColorBrush BrushInfo = new(Microsoft.UI.Colors.Silver);

    private sealed class LogEntry
    {
        public string Text { get; set; } = "";
        public string Glyph { get; set; } = GlyphInfo;
        public SolidColorBrush GlyphColor { get; set; } = BrushInfo;
    }

    private readonly List<CheckBox> _tweakCheckBoxes = new();

    public TweaksPage()
    {
        this.InitializeComponent();
        BuildTweakList();
    }

    private void BuildTweakList()
    {
        foreach (var category in TweakCatalog.Categories)
        {
            var header = new TextBlock
            {
                Text = category,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 8, 0, 4)
            };
            TweaksPanel.Children.Add(header);

            foreach (var tweak in TweakCatalog.Tweaks.Where(t => t.Category == category))
            {
                var checkBox = new CheckBox
                {
                    Tag = tweak,
                    Content = new StackPanel
                    {
                        Spacing = 2,
                        Children =
                        {
                            new TextBlock { Text = tweak.Name, Style = (Style)Application.Current.Resources["CardHeaderStyle"] },
                            new TextBlock
                            {
                                Text = tweak.Description,
                                Style = (Style)Application.Current.Resources["CardDescriptionStyle"],
                                TextWrapping = TextWrapping.Wrap,
                                MaxWidth = 640
                            }
                        }
                    }
                };
                _tweakCheckBoxes.Add(checkBox);
                TweaksPanel.Children.Add(checkBox);
            }
        }
    }

    private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var cb in _tweakCheckBoxes) cb.IsChecked = true;
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        foreach (var cb in _tweakCheckBoxes) cb.IsChecked = false;
    }

    private async void BtnApplyTweaks_Click(object sender, RoutedEventArgs e)
    {
        var selected = _tweakCheckBoxes
            .Where(cb => cb.IsChecked == true && cb.Tag is AppTweak)
            .Select(cb => (AppTweak)cb.Tag!)
            .ToList();

        if (selected.Count == 0)
        {
            await ShowInfo("No tweaks selected.", "Nothing to do");
            return;
        }

        var log = new ObservableCollection<LogEntry>();
        var headerText = new TextBlock { Text = "Applying tweaks...", TextWrapping = TextWrapping.Wrap };
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
            Title = "Applying Tweaks...",
            Content = panel,
            XamlRoot = this.XamlRoot
        };
        _ = progressDialog.ShowAsync();

        void AddEntry(string text, string glyph, SolidColorBrush brush)
        {
            var entry = new LogEntry { Text = text, Glyph = glyph, GlyphColor = brush };
            DispatcherQueue.TryEnqueue(() =>
            {
                log.Add(entry);
                scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            });
        }

        var failedTweaks = new List<string>();
        var appliedCount = 0;

        await Task.Run(() =>
        {
            foreach (var tweak in selected)
            {
                DispatcherQueue.TryEnqueue(() => headerText.Text = $"Applying {tweak.Name}...");
                AddEntry($"Applying {tweak.Name}...", GlyphRunning, BrushRunning);

                var failedValues = new List<string>();
                var ok = TweakHelper.ApplyTweak(tweak, failedValues);

                if (ok)
                {
                    appliedCount++;
                    AddEntry($"{tweak.Name} - done", GlyphSuccess, BrushSuccess);
                }
                else
                {
                    failedTweaks.Add(tweak.Name);
                    AddEntry($"{tweak.Name} - failed ({(failedValues.Count > 0 ? string.Join("; ", failedValues) : "unknown error")})", GlyphFailed, BrushFailed);
                }
            }
        });

        progressDialog.Hide();

        var summary = new StringBuilder();
        summary.AppendLine("Tweaks applied.");
        summary.AppendLine();
        summary.AppendLine($"Applied: {appliedCount}");
        summary.AppendLine($"Failed: {failedTweaks.Count}");

        if (failedTweaks.Count > 0)
        {
            summary.AppendLine();
            summary.AppendLine("Failed tweaks:");
            foreach (var name in failedTweaks) summary.AppendLine($"  - {name}");
        }

        summary.AppendLine();
        summary.AppendLine("Restart Explorer or sign out so the changes take effect.");

        await ShowInfo(summary.ToString(), "Tweak Summary");
    }

    private async Task ShowInfo(string message, string title)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }
}

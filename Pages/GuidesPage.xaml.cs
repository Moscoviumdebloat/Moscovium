using System.Linq;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Models;

namespace MoscoviumThree.Pages;

public sealed partial class GuidesPage : Page
{
    public GuidesPage()
    {
        this.InitializeComponent();
        BuildGuides();
    }

    private void BuildGuides()
    {
        foreach (var guide in GuideCatalog.Guides)
        {
            var stepsPanel = new StackPanel { Spacing = 6 };
            for (int i = 0; i < guide.Steps.Count; i++)
            {
                var step = new Grid { ColumnSpacing = 8, Margin = new Thickness(0, 0, 0, 4) };
                step.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
                step.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var number = new TextBlock
                {
                    Text = (i + 1).ToString(),
                    FontWeight = FontWeights.SemiBold,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AccentTextFillColorPrimaryBrush"],
                    VerticalAlignment = VerticalAlignment.Top
                };
                var text = new TextBlock
                {
                    Text = guide.Steps[i],
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Top
                };

                Grid.SetColumn(text, 1);
                step.Children.Add(number);
                step.Children.Add(text);
                stepsPanel.Children.Add(step);
            }

            var header = new StackPanel
            {
                Spacing = 2,
                Children =
                {
                    new TextBlock { Text = guide.Title, FontWeight = FontWeights.SemiBold },
                    new TextBlock
                    {
                        Text = $"{guide.Category} - {guide.Summary}",
                        Style = (Style)Application.Current.Resources["CardDescriptionStyle"],
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
                    }
                }
            };

            var expander = new Expander
            {
                Header = header,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                IsExpanded = false
            };
            expander.Content = stepsPanel;

            GuidesPanel.Children.Add(expander);
        }
    }
}

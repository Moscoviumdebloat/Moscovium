using System.Reflection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace MoscoviumThree.Pages;

public sealed partial class AboutPage : Page
{
    public AboutPage()
    {
        this.InitializeComponent();
        this.Loaded += AboutPage_Loaded;
        LoadAppIcon();
    }

    private void AboutPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TxtVersion.Text = $"Version {Helpers.UpdateService.GetCurrentVersion()}";
    }

    private void LoadAppIcon()
    {
        try
        {
            var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Illustration.gif");
            if (File.Exists(iconPath))
            {
                AppIcon.Source = new BitmapImage(new Uri(iconPath));
            }
        }
        catch { }
    }
}

using System.Reflection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace MoscoviumThree.Pages;

public sealed partial class AboutPage : Page
{
    public AboutPage()
    {
        this.InitializeComponent();
        LoadVersionInfo();
        LoadAppIcon();
    }

    private void LoadVersionInfo()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        TxtVersion.Text = $"Version {version}";
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

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Helpers;

namespace MoscoviumThree.Pages;

public sealed partial class ToolboxPage : Page
{
    public ToolboxPage()
    {
        this.InitializeComponent();
    }

    private void BtnCTT_Click(object sender, RoutedEventArgs e)
    {
        ProcessHelper.RunElevatedPowerShellRaw("iwr -useb https://christitus.com/win | iex");
    }

    private void BtnWin11_Click(object sender, RoutedEventArgs e)
    {
        ProcessHelper.RunElevatedPowerShellRaw("& ([scriptblock]::Create((irm \"https://debloat.raphi.re/\")))");
    }

    private void BtnMAS_Click(object sender, RoutedEventArgs e)
    {
        ProcessHelper.RunElevatedPowerShellRaw("irm https://get.activated.win | iex");
    }

    private void BtnWifi_Click(object sender, RoutedEventArgs e)
    {
        // Navigate to the Network page via the main NavigationView
        var mainWindow = (App.Current as App)?.GetType()
            .GetField("m_window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(App.Current) as MainWindow;

        if (mainWindow != null)
        {
            // Find the network nav item and select it
            var navView = mainWindow.Content as Grid;
            if (navView != null)
            {
                var nav = FindNavigationView(navView);
                if (nav != null)
                {
                    foreach (var item in nav.MenuItems)
                    {
                        if (item is NavigationViewItem navItem && navItem.Tag?.ToString() == "network")
                        {
                            nav.SelectedItem = navItem;
                            break;
                        }
                    }
                }
            }
        }
    }

    private NavigationView? FindNavigationView(DependencyObject parent)
    {
        for (int i = 0; i < Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is NavigationView nav) return nav;
            var result = FindNavigationView(child);
            if (result != null) return result;
        }
        return null;
    }
}

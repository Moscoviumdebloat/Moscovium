using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Helpers;

namespace MoscoviumThree.Pages;

public sealed partial class NetworkPage : Page
{
    public NetworkPage()
    {
        this.InitializeComponent();
    }

    private void BtnBetter_Click(object sender, RoutedEventArgs e)
    {
        ProcessHelper.RunElevatedPowerShellRaw("netsh int tcp set global autotuninglevel=disabled");
    }

    private void BtnDefault_Click(object sender, RoutedEventArgs e)
    {
        ProcessHelper.RunElevatedPowerShellRaw("netsh int tcp set global autotuninglevel=normal");
    }
}

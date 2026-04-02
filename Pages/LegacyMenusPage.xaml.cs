using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace MoscoviumThree.Pages;

public sealed partial class LegacyMenusPage : Page
{
    public LegacyMenusPage()
    {
        this.InitializeComponent();
    }

    private void BtnControlPanel_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("control.exe") { UseShellExecute = true });
    }

    private void BtnServices_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("services.msc") { UseShellExecute = true });
    }

    private void BtnMouseSettings_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("main.cpl") { UseShellExecute = true });
    }

    private void BtnKeyboardSettings_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("control", "keyboard") { UseShellExecute = true });
    }

    private void BtnSoundPanel_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("mmsys.cpl") { UseShellExecute = true });
    }
}

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

    private async void BtnWin11_Click(object sender, RoutedEventArgs e)
    {
        // Define options
        var rbOption1 = new RadioButton 
        { 
            Content = "Install Explorer Patcher, OpenShell, and Nilesoft Shell with the debloat script", 
            IsChecked = true, 
            Tag = "Option1",
            Margin = new Thickness(0, 4, 0, 4)
        };
        var rbOption2 = new RadioButton 
        { 
            Content = "Install StartAllBack with the debloat script", 
            Tag = "Option2",
            Margin = new Thickness(0, 4, 0, 4)
        };
        var rbOption3 = new RadioButton 
        { 
            Content = "Just the debloat script", 
            Tag = "Option3",
            Margin = new Thickness(0, 4, 0, 4)
        };

        var stack = new StackPanel { Spacing = 4 };
        stack.Children.Add(rbOption1);
        stack.Children.Add(rbOption2);
        stack.Children.Add(rbOption3);

        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "Debloat Options",
            Content = stack,
            PrimaryButtonText = "Run",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            if (rbOption1.IsChecked == true)
            {
                RunDebloatWithInstallers();
            }
            else if (rbOption2.IsChecked == true)
            {
                RunDebloatWithStartAllBack();
            }
            else
            {
                RunDebloatOnly();
            }
        }
    }

    private void RunDebloatOnly()
    {
        ProcessHelper.RunElevatedPowerShellRaw("& ([scriptblock]::Create((irm \"https://debloat.raphi.re/\")))");
    }

    private void RunDebloatWithInstallers()
    {
        // Build a PowerShell script to chain installers and then the debloat script
        // Note: Using Start-Process -Wait ensures sequential execution
        
        string assetsPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Assets", "Bundled");
        var scriptBuilder = new System.Text.StringBuilder();
        
        // 1. Explorer Patcher (ep_setup.exe)
        var epPath = System.IO.Path.Combine(assetsPath, "ep_setup.exe");
        if (System.IO.File.Exists(epPath))
        {
             scriptBuilder.Append($"Start-Process -FilePath '{epPath}' -Wait; ");
        }

        // 2. OpenShell (OpenShellSetup*.exe)
        // Find existing file matching pattern
        try
        {
            var openShellFile = System.IO.Directory.GetFiles(assetsPath, "OpenShellSetup*.exe").FirstOrDefault();
            if (openShellFile != null)
            {
                scriptBuilder.Append($"Start-Process -FilePath '{openShellFile}' -Wait; ");
            }
        }
        catch { /* Ignore finding error */ }

        // 3. Nilesoft Shell (setup-x64.msi or similar)
        // Assuming .msi file is Nilesoft
        try
        {
            var msiFile = System.IO.Directory.GetFiles(assetsPath, "*.msi").FirstOrDefault();
            if (msiFile != null)
            {
                scriptBuilder.Append($"Start-Process -FilePath 'msiexec.exe' -ArgumentList '/i \"{msiFile}\"' -Wait; ");
            }
        }
        catch { /* Ignore */ }

        // 4. Run Raphi Debloat
        scriptBuilder.Append("& ([scriptblock]::Create((irm \"https://debloat.raphi.re/\")))");

        ProcessHelper.RunElevatedPowerShellRaw(scriptBuilder.ToString());
    }

    private void RunDebloatWithStartAllBack()
    {
        // Run StartAllBack.ps1 then Raphi
        // StartAllBack.ps1 is interactive and might close the window at the end, so we launch it in a separate waitable process first.
        
        string sabPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "StartAllBack.ps1");
        string script = "";
        
        if (System.IO.File.Exists(sabPath))
        {
             // Start-Process powershell -Wait -File ...
             script += $"Start-Process powershell.exe -ArgumentList '-ExecutionPolicy Bypass -File \"{sabPath}\"' -Wait; ";
        }
        
        script += "& ([scriptblock]::Create((irm \"https://debloat.raphi.re/\")))";

        ProcessHelper.RunElevatedPowerShellRaw(script);
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

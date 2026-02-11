using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoscoviumThree.Pages;
using Windows.Graphics;
using WinRT.Interop;

namespace MoscoviumThree;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        // Set Mica Alt backdrop for modern acrylic look
        this.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };

        // Configure window
        SetupWindow();
    }

    private void SetupWindow()
    {
        // Get AppWindow for title bar and sizing
        var hWnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        // Set window size
        appWindow.Resize(new SizeInt32(960, 640));

        // Set title bar
        appWindow.Title = "Moscovium v3.0";

        // Try to set the icon
        try
        {
            var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Logo.ico");
            if (File.Exists(iconPath))
            {
                appWindow.SetIcon(iconPath);
            }
        }
        catch { }

        // Center window on screen
        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = true;
            presenter.IsMaximizable = true;
        }

        // Request dark theme
        if (this.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = ElementTheme.Dark;
        }
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // Select Home by default
        NavView.SelectedItem = NavView.MenuItems[0];
        ContentFrame.Navigate(typeof(HomePage));
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            switch (tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
                case "toolbox":
                    ContentFrame.Navigate(typeof(ToolboxPage));
                    break;
                case "network":
                    ContentFrame.Navigate(typeof(NetworkPage));
                    break;
                case "about":
                    ContentFrame.Navigate(typeof(AboutPage));
                    break;
            }
        }
    }
}

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

    [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    private void SetupWindow()
    {
        // Get AppWindow for title bar and sizing
        var hWnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        // Force Dark Mode on Title Bar (Standard Win32)
        int useDarkMode = 1;
        if (DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int)) != 0)
        {
            DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref useDarkMode, sizeof(int));
        }

        // Set window size
        appWindow.Resize(new SizeInt32(960, 640));

        // Set title bar
        appWindow.Title = "Moscovium v3.0";

        // Try to set the icon
        try
        {
            // For portable/single-file, BaseDirectory might be temp.
            // Check process main module filename as fallback for "installed" path if needed, 
            // but usually Assets are content.
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
        if (args.IsSettingsSelected)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
        }
        else if (args.SelectedItemContainer is NavigationViewItem selectedItem)
        {
            switch (selectedItem.Tag.ToString())
            {
                case "home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;

                case "toolbox":
                    ContentFrame.Navigate(typeof(ToolboxPage));
                    break;

                case "cs2":
                    ContentFrame.Navigate(typeof(CS2Page));
                    break;

                case "about":
                    ContentFrame.Navigate(typeof(AboutPage));
                    break;

                case "store":
                    ContentFrame.Navigate(typeof(AppStorePage));
                    break;

                // Settings is handled by IsSettingsSelected
            }
        }
    }
}

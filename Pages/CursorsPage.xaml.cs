using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace MoscoviumThree.Pages;

public sealed partial class CursorsPage : Page
{
    // P/Invoke to refresh cursors system-wide without logoff
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

    private const uint SPI_SETCURSORS = 0x0057;
    private const uint SPIF_UPDATEINIFILE = 0x01;
    private const uint SPIF_SENDCHANGE = 0x02;

    // Standard cursor registry names
    private static readonly string[] CursorRegistryNames = new[]
    {
        "Arrow", "Help", "AppStarting", "Wait", "Crosshair",
        "IBeam", "NWPen", "No", "SizeNS", "SizeWE",
        "SizeNWSE", "SizeNESW", "SizeAll", "UpArrow", "Hand",
        "Person", "Pin"
    };

    public CursorsPage()
    {
        this.InitializeComponent();
    }

    // ─── Click Handlers ──────────────────────────────────────────

    private void ApplyCursorConcept1Dark_Click(object sender, RoutedEventArgs e)
    {
        var cursorDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Cursors", "CursorConcept1", "cursor", "dark");
        var mapping = BuildStandardMapping(cursorDir);
        ApplyCursorScheme("Cursor Concept 1 Dark Free", mapping);
    }

    private void ApplyCursorConcept1Light_Click(object sender, RoutedEventArgs e)
    {
        var cursorDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Cursors", "CursorConcept1", "cursor", "light");
        var mapping = BuildStandardMapping(cursorDir);
        ApplyCursorScheme("Cursor Concept 1 Light Free", mapping);
    }

    private void ApplyMaterialDark_Click(object sender, RoutedEventArgs e)
    {
        var cursorDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Cursors", "MaterialDesign", "dark");
        var mapping = BuildStandardMapping(cursorDir);
        ApplyCursorScheme("Material Design Dark Free", mapping);
    }

    private void ApplyMaterialLight_Click(object sender, RoutedEventArgs e)
    {
        var cursorDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Cursors", "MaterialDesign", "light");
        var mapping = BuildStandardMapping(cursorDir);
        ApplyCursorScheme("Material Design Light Free", mapping);
    }

    private void ApplyMacOSNoShadow_Click(object sender, RoutedEventArgs e)
    {
        var cursorDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Cursors", "MacOSCursors",
            "1. Sierra and newer", "1. No Shadow", "1. Normal");
        var mapping = BuildMacOSMapping(cursorDir);
        ApplyCursorScheme("macOS Cursors No Shadow", mapping);
    }

    private void ApplyMacOSWithShadow_Click(object sender, RoutedEventArgs e)
    {
        var cursorDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Cursors", "MacOSCursors",
            "1. Sierra and newer", "2. With Shadow", "1. Normal");
        var mapping = BuildMacOSMapping(cursorDir);
        ApplyCursorScheme("macOS Cursors With Shadow", mapping);
    }

    private void RestoreDefaultCursors_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors", true);
            if (key != null)
            {
                // Set scheme name to empty (Windows Default)
                key.SetValue("", "Windows Default", RegistryValueKind.String);

                // Clear all cursor entries to restore OS defaults
                foreach (var name in CursorRegistryNames)
                {
                    key.SetValue(name, "", RegistryValueKind.ExpandString);
                }
            }

            // Refresh cursors system-wide
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            ShowStatus("Default cursors restored!", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to restore cursors: {ex.Message}");
            ShowStatus($"Failed to restore cursors: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    // ─── Custom Cursor Picker ─────────────────────────────────

    private readonly Dictionary<string, string> _customCursorFiles = new();

    private async void PickCursorFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not string cursorName) return;

        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.FileTypeFilter.Add(".cur");
        picker.FileTypeFilter.Add(".ani");

        var window = App.m_window;
        if (window == null) return;

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        _customCursorFiles[cursorName] = file.Path;
        btn.Content = $"{cursorName}: {file.Name}";
    }

    private void ApplyCustomCursors_Click(object sender, RoutedEventArgs e)
    {
        if (_customCursorFiles.Count == 0)
        {
            ShowStatus("No cursor files selected. Click the buttons above to pick files.", InfoBarSeverity.Warning);
            return;
        }

        ApplyCursorScheme("Moscovium Custom", _customCursorFiles);
    }

    // ─── Cursor Mapping Builders ────────────────────────────────

    /// <summary>
    /// Build mapping for Cursor Concept 1 and Material Design packs (same file naming).
    /// </summary>
    private Dictionary<string, string> BuildStandardMapping(string dir)
    {
        return new Dictionary<string, string>
        {
            ["Arrow"]      = Path.Combine(dir, "arrow.cur"),
            ["Help"]       = Path.Combine(dir, "help.cur"),
            ["AppStarting"]= Path.Combine(dir, "appstarting.ani"),
            ["Wait"]       = Path.Combine(dir, "wait.ani"),
            ["Crosshair"]  = Path.Combine(dir, "crosshair.cur"),
            ["IBeam"]      = Path.Combine(dir, "ibeam.cur"),
            ["NWPen"]      = Path.Combine(dir, "nwpen.cur"),
            ["No"]         = Path.Combine(dir, "no.cur"),
            ["SizeNS"]     = Path.Combine(dir, "sizens.cur"),
            ["SizeWE"]     = Path.Combine(dir, "sizewe.cur"),
            ["SizeNWSE"]   = Path.Combine(dir, "sizenwse.cur"),
            ["SizeNESW"]   = Path.Combine(dir, "sizenesw.cur"),
            ["SizeAll"]    = Path.Combine(dir, "sizeall.cur"),
            ["UpArrow"]    = Path.Combine(dir, "uparrow.cur"),
            ["Hand"]       = Path.Combine(dir, "hand.cur"),
            ["Person"]     = Path.Combine(dir, "person.cur"),
            ["Pin"]        = Path.Combine(dir, "pin.cur"),
        };
    }

    /// <summary>
    /// Build mapping for macOS cursor pack (different file naming convention).
    /// </summary>
    private Dictionary<string, string> BuildMacOSMapping(string dir)
    {
        return new Dictionary<string, string>
        {
            ["Arrow"]      = Path.Combine(dir, "Normal.cur"),
            ["Help"]       = Path.Combine(dir, "Help.cur"),
            ["AppStarting"]= Path.Combine(dir, "Working.ani"),
            ["Wait"]       = Path.Combine(dir, "Busy.ani"),
            ["Crosshair"]  = Path.Combine(dir, "Precision.cur"),
            ["IBeam"]      = Path.Combine(dir, "Text.cur"),
            ["NWPen"]      = Path.Combine(dir, "Handwriting.cur"),
            ["No"]         = Path.Combine(dir, "Unavailable.cur"),
            ["SizeNS"]     = Path.Combine(dir, "Vertical Resize.cur"),
            ["SizeWE"]     = Path.Combine(dir, "Horizontal Resize.cur"),
            ["SizeNWSE"]   = Path.Combine(dir, "Diagonal Resize 1.cur"),
            ["SizeNESW"]   = Path.Combine(dir, "Diagonal Resize 2.cur"),
            ["SizeAll"]    = Path.Combine(dir, "Move.cur"),
            ["UpArrow"]    = Path.Combine(dir, "Alternate.cur"),
            ["Hand"]       = Path.Combine(dir, "Link.cur"),
            ["Person"]     = Path.Combine(dir, "Person.cur"),
            ["Pin"]        = Path.Combine(dir, "Pin.cur"),
        };
    }

    // ─── Core Installation Logic ────────────────────────────────

    private void ApplyCursorScheme(string schemeName, Dictionary<string, string> cursorMapping)
    {
        try
        {
            // 1. Copy cursor files to %LOCALAPPDATA%\Moscovium\Cursors\<schemeName>
            var destDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Moscovium", "Cursors", schemeName);

            Directory.CreateDirectory(destDir);

            var installedMapping = new Dictionary<string, string>();

            foreach (var kvp in cursorMapping)
            {
                if (File.Exists(kvp.Value))
                {
                    var destFile = Path.Combine(destDir, Path.GetFileName(kvp.Value));
                    File.Copy(kvp.Value, destFile, overwrite: true);
                    installedMapping[kvp.Key] = destFile;
                }
                else
                {
                    Debug.WriteLine($"Cursor file not found: {kvp.Value}");
                }
            }

            // 2. Set registry values under HKCU\Control Panel\Cursors
            using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors", true);
            if (key != null)
            {
                // Set the scheme display name
                key.SetValue("", schemeName, RegistryValueKind.String);

                foreach (var kvp in installedMapping)
                {
                    key.SetValue(kvp.Key, kvp.Value, RegistryValueKind.ExpandString);
                }

                // Clear any cursor types that are not in this pack
                foreach (var name in CursorRegistryNames)
                {
                    if (!installedMapping.ContainsKey(name))
                    {
                        key.SetValue(name, "", RegistryValueKind.ExpandString);
                    }
                }
            }

            // 3. Register the scheme
            using var schemesKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors\Schemes", true);
            if (schemesKey != null)
            {
                var schemeValue = string.Join(",", CursorRegistryNames.Select(n =>
                    installedMapping.ContainsKey(n) ? installedMapping[n] : ""));
                schemesKey.SetValue(schemeName, schemeValue, RegistryValueKind.String);
            }

            // 4. Refresh cursors system-wide (instant, no restart needed)
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            ShowStatus($"Applied \"{schemeName}\" cursor pack!", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to apply cursor scheme: {ex.Message}");
            ShowStatus($"Failed to apply cursors: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    private void ShowStatus(string message, InfoBarSeverity severity)
    {
        StatusInfoBar.Message = message;
        StatusInfoBar.Severity = severity;
        StatusInfoBar.IsOpen = true;
    }
}

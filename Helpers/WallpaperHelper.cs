using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MoscoviumThree.Helpers;

/// <summary>
/// Sets the desktop wallpaper with various fit styles using Win32 API.
/// Ported from MoscoviumTwo's Wallpaper class.
/// </summary>
public static class WallpaperHelper
{
    private const int SPI_SETDESKWALLPAPER = 20;
    private const int SPIF_UPDATEINIFILE = 1;
    private const int SPIF_SENDWININICHANGE = 2;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    /// <summary>
    /// Set wallpaper with the specified fit style.
    /// </summary>
    /// <param name="filePath">Path to the image file</param>
    /// <param name="style">Fill, Fit, Stretch, Tile, Center, or Span</param>
    public static void Set(string filePath, string style)
    {
        // Convert to BMP for wallpaper compatibility
        using var stream = new MemoryStream(File.ReadAllBytes(filePath));
        using var image = Image.FromStream(stream);
        var bmpPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
        image.Save(bmpPath, ImageFormat.Bmp);

        // Set registry values for wallpaper style
        using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
        if (key == null)
            throw new InvalidOperationException("Unable to access Desktop registry key.");

        var (wallpaperStyle, tileWallpaper) = style switch
        {
            "Fill" => ("10", "0"),
            "Fit" => ("6", "0"),
            "Stretch" => ("2", "0"),
            "Tile" => ("0", "1"),
            "Center" => ("0", "0"),
            "Span" => ("22", "0"),
            _ => ("10", "0") // Default to Fill
        };

        key.SetValue("WallpaperStyle", wallpaperStyle);
        key.SetValue("TileWallpaper", tileWallpaper);

        // Apply wallpaper
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, bmpPath,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }
}

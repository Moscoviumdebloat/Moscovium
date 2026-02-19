using System.IO.Compression;
using System.Reflection;

namespace MoscoviumThree.Helpers;

/// <summary>
/// Extracts bundled embedded resources to temp directories.
/// </summary>
public static class ResourceHelper
{
    private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();

    /// <summary>
    /// Extract an embedded resource to a temp file.
    /// </summary>
    /// <param name="resourceName">The embedded resource name (filename in Assets/Bundled)</param>
    /// <param name="outputFileName">The desired output filename</param>
    /// <returns>Full path to the extracted file</returns>
    public static string ExtractToTemp(string resourceName, string outputFileName)
    {
        var outputPath = Path.Combine(Path.GetTempPath(), outputFileName);

        // Try to find the resource by matching the end of the name
        var fullResourceName = CurrentAssembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

        if (fullResourceName == null)
        {
            // Fallback: try reading from Assets/Bundled directory next to exe
            var localPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Bundled", resourceName);
            if (File.Exists(localPath))
            {
                File.Copy(localPath, outputPath, true);
                return outputPath;
            }
            throw new FileNotFoundException($"Resource '{resourceName}' not found.");
        }

        using var stream = CurrentAssembly.GetManifestResourceStream(fullResourceName)!;
        using var fileStream = File.Create(outputPath);
        stream.CopyTo(fileStream);

        return outputPath;
    }

    /// <summary>
    /// Extract an embedded ZIP resource and decompress it to a temp directory.
    /// </summary>
    /// <param name="resourceName">The ZIP resource filename</param>
    /// <param name="outputFolderName">Desired output folder name in temp</param>
    /// <returns>Full path to the extracted directory</returns>
    public static string ExtractZipToTemp(string resourceName, string outputFolderName)
    {
        var zipPath = ExtractToTemp(resourceName, resourceName);
        var extractPath = Path.Combine(Path.GetTempPath(), outputFolderName);

        if (Directory.Exists(extractPath))
            Directory.Delete(extractPath, true);

        ZipFile.ExtractToDirectory(zipPath, extractPath);

        return extractPath;
    }

    /// <summary>
    /// Asynchronously extract an embedded ZIP resource and decompress it to a temp directory.
    /// </summary>
    /// <param name="resourceName">The ZIP resource filename</param>
    /// <param name="outputFolderName">Desired output folder name in temp</param>
    /// <returns>Full path to the extracted directory</returns>
    public static async Task<string> ExtractZipToTempAsync(string resourceName, string outputFolderName)
    {
        return await Task.Run(() => ExtractZipToTemp(resourceName, outputFolderName));
    }
}

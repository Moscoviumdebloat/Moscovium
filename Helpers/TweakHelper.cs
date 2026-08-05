using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using MoscoviumThree.Models;

namespace MoscoviumThree.Helpers;

/// <summary>
/// Applies <see cref="AppTweak"/> items: registry values and special actions.
/// </summary>
public static class TweakHelper
{
    /// <summary>
    /// Applies a tweak, returning true on full success. Registry errors are collected in failedValues.
    /// </summary>
    public static bool ApplyTweak(AppTweak tweak, List<string>? failedValues = null)
    {
        switch (tweak.Kind)
        {
            case "RestorePoint":
                return CreateRestorePoint();
            case "CleanDisk":
                return RunDiskCleanup();
            case "CleanTemp":
                return CleanTempFiles();
            case "PowerPlan":
                return SetHighPerformancePowerPlan();
        }

        if (tweak.Registry == null) return true;

        var ok = true;
        foreach (var value in tweak.Registry)
        {
            try
            {
                WriteValue(value.Path, value.Name, value.Value, value.Type);
            }
            catch (Exception ex)
            {
                ok = false;
                failedValues?.Add($"{value.Path}\\{value.Name}: {ex.Message}");
                Debug.WriteLine($"Tweak failed for {value.Path}\\{value.Name}: {ex.Message}");
            }
        }
        return ok;
    }

    private static void WriteValue(string fullPath, string name, object value, string type)
    {
        var root = fullPath.StartsWith("HKEY_LOCAL_MACHINE", StringComparison.OrdinalIgnoreCase)
            ? Registry.LocalMachine
            : Registry.CurrentUser;

        // Strip just the hive prefix, keep the full key subpath
        var keyPath = fullPath[(fullPath.IndexOf('\\') + 1)..];

        using var key = root.CreateSubKey(keyPath, true);
        if (key == null) throw new IOException($"Cannot open registry key {keyPath}");

        var kind = type.ToUpperInvariant() switch
        {
            "DWORD" => RegistryValueKind.DWord,
            "QWORD" => RegistryValueKind.QWord,
            "STRING" => RegistryValueKind.String,
            _ => RegistryValueKind.Unknown
        };

        if (kind == RegistryValueKind.DWord) key.SetValue(name, Convert.ToInt32(value), kind);
        else if (kind == RegistryValueKind.QWord) key.SetValue(name, Convert.ToInt64(value), kind);
        else key.SetValue(name, Convert.ToString(value) ?? string.Empty, kind);
    }

    private static bool SetHighPerformancePowerPlan()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "powercfg",
                Arguments = "-setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            using var process = Process.Start(startInfo);
            if (process == null) return false;
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Power plan switch failed: {ex.Message}");
            return false;
        }
    }

    private static bool CreateRestorePoint()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"Enable-ComputerRestore -Drive 'C:\\'; Checkpoint-Computer -Description 'Moscovium tweaks' -RestorePointType MODIFY_SETTINGS\"",
                UseShellExecute = true,
                CreateNoWindow = false
            };
            using var process = Process.Start(startInfo);
            if (process == null) return false;
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Restore point failed: {ex.Message}");
            return false;
        }
    }

    private static bool RunDiskCleanup()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cleanmgr.exe",
                Arguments = "/verylowdisk",
                UseShellExecute = true,
                CreateNoWindow = false
            };
            using var process = Process.Start(startInfo);
            if (process == null) return false;
            process.WaitForExit();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Disk cleanup failed: {ex.Message}");
            return false;
        }
    }

    private static bool CleanTempFiles()
    {
        var cleaned = false;
        var paths = new[]
        {
            Path.GetTempPath(),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"),
        };

        foreach (var path in paths)
        {
            if (!Directory.Exists(path)) continue;

            foreach (var file in Directory.EnumerateFiles(path))
            {
                try { File.Delete(file); cleaned = true; } catch { }
            }
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                try { Directory.Delete(dir, true); cleaned = true; } catch { }
            }
        }
        return cleaned;
    }
}

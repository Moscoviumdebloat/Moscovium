using System.Diagnostics;

namespace MoscoviumThree.Helpers;

/// <summary>
/// Shared utility for running elevated PowerShell/process commands.
/// </summary>
public static class ProcessHelper
{
    private static readonly string PowerShellPath =
        Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe");

    /// <summary>
    /// Run a PowerShell command elevated (runas).
    /// </summary>
    public static Process? RunElevatedPowerShell(string command, bool noExit = false)
    {
        var encodedCommand = GetEncodedCommand(command);
        var noExitFlag = noExit ? "-NoExit " : "";
        return Process.Start(new ProcessStartInfo
        {
            FileName = PowerShellPath,
            Arguments = $"-NoProfile -ExecutionPolicy Bypass {noExitFlag}-EncodedCommand {encodedCommand}",
            RedirectStandardOutput = false,
            UseShellExecute = true,
            CreateNoWindow = false,
            Verb = "runas"
        });
    }

    /// <summary>
    /// Run a PowerShell command elevated with explicit arguments.
    /// </summary>
    public static Process? RunElevatedPowerShellRaw(string command)
    {
        var encodedCommand = GetEncodedCommand(command);
        return Process.Start(new ProcessStartInfo
        {
            FileName = PowerShellPath,
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -EncodedCommand {encodedCommand}",
            RedirectStandardOutput = false,
            UseShellExecute = true,
            CreateNoWindow = false,
            Verb = "runas"
        });
    }

    /// <summary>
    /// Run an executable elevated.
    /// </summary>
    public static Process? RunElevated(string fileName, string arguments = "")
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = true,
            Verb = "runas"
        });
    }

    /// <summary>
    /// Run an executable normally (no elevation).
    /// </summary>
    public static Process? Run(string fileName, string arguments = "")
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = true
        });
    }

    /// <summary>
    /// Run a PowerShell command with NoProfile, Bypass, NoExit.
    /// </summary>
    public static Process? RunPowerShellCommand(string command)
    {
        var encodedCommand = GetEncodedCommand(command);
        return Process.Start(new ProcessStartInfo
        {
            FileName = PowerShellPath,
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -NoExit -EncodedCommand {encodedCommand}",
            RedirectStandardOutput = false,
            UseShellExecute = true,
            CreateNoWindow = false,
            Verb = "runas"
        });
    }

    private static string GetEncodedCommand(string command)
    {
        return Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(command));
    }
}

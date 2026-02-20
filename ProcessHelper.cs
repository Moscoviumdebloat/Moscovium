using System.Diagnostics;

namespace Moscovium_Lite;

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
    public static void RunElevatedPowerShell(string arguments, bool noExit = false)
    {
        var prefix = noExit ? "-NoProfile -ExecutionPolicy Bypass -NoExit -Command " : "";
        Process.Start(new ProcessStartInfo
        {
            FileName = PowerShellPath,
            Arguments = prefix + arguments,
            RedirectStandardOutput = false,
            UseShellExecute = true,
            CreateNoWindow = false,
            Verb = "runas"
        });
    }

    /// <summary>
    /// Run a PowerShell command elevated with explicit arguments.
    /// </summary>
    public static void RunElevatedPowerShellRaw(string arguments)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = PowerShellPath,
            Arguments = arguments,
            RedirectStandardOutput = false,
            UseShellExecute = true,
            CreateNoWindow = false,
            Verb = "runas"
        });
    }

    /// <summary>
    /// Run an executable elevated.
    /// </summary>
    public static void RunElevated(string fileName, string arguments = "")
    {
        Process.Start(new ProcessStartInfo
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
    public static void Run(string fileName, string arguments = "")
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = true
        });
    }

    /// <summary>
    /// Run a PowerShell command with NoProfile, Bypass, NoExit.
    /// </summary>
    public static void RunPowerShellCommand(string command)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = PowerShellPath,
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -NoExit -Command \"{command}\"",
            RedirectStandardOutput = false,
            UseShellExecute = true,
            CreateNoWindow = false,
            Verb = "runas"
        });
    }
}

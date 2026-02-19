using Microsoft.Win32;

namespace MoscoviumThree.Helpers;

public static class RegistryHelper
{
    private const string PriorityControlKeyPath = @"SYSTEM\ControlSet001\Control\PriorityControl";
    private const string Win32PrioritySeparationValueName = "Win32PrioritySeparation";

    /// <summary>
    /// Sets the Win32PrioritySeparation registry value.
    /// </summary>
    /// <param name="value">The decimal value to set (e.g. 22).</param>
    public static void SetWin32PrioritySeparation(int value)
    {
        try
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(PriorityControlKeyPath, true))
            {
                if (key != null)
                {
                    key.SetValue(Win32PrioritySeparationValueName, value, RegistryValueKind.DWord);
                }
            }
        }
        catch (Exception ex)
        {
            // Ideally log the error, but for now we catch to prevent crash if access is denied etc.
            System.Diagnostics.Debug.WriteLine($"Error setting registry value: {ex.Message}");
        }
    }
}

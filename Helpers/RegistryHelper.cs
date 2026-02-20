using Microsoft.Win32;

namespace MoscoviumThree.Helpers;

public static class RegistryHelper
{
    private const string PriorityControlKeyPath = @"SYSTEM\CurrentControlSet\Control\PriorityControl";
    private const string Win32PrioritySeparationValueName = "Win32PrioritySeparation";

    /// <summary>
    /// Sets the Win32PrioritySeparation registry value.
    /// </summary>
    /// <param name="value">The decimal value to set (e.g. 22).</param>
    public static void SetWin32PrioritySeparation(int value)
    {
        // CreateSubKey opens the key if it exists with write access, or creates it if it doesn't.
        using (RegistryKey key = Registry.LocalMachine.CreateSubKey(PriorityControlKeyPath, true))
        {
            if (key != null)
            {
                key.SetValue(Win32PrioritySeparationValueName, value, RegistryValueKind.DWord);
            }
        }
    }
}

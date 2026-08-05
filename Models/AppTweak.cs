using System.Collections.Generic;

namespace MoscoviumThree.Models;

/// <summary>
/// A single registry value to write.
/// Type is one of: DWord (int), QWord (long), String (string).
/// An empty Name sets the (Default) value.
/// </summary>
public record RegValue(string Path, string Name, string Type, object Value);

/// <summary>
/// One debloat tweak. Registry holds the values to apply; Kind selects a special action.
/// </summary>
public record AppTweak(string Name, string Description, string Category, List<RegValue>? Registry = null, string Kind = "Registry");

public static class TweakCatalog
{
    public static readonly List<AppTweak> Tweaks = new()
    {
        // ===== Privacy & Telemetry =====
        new("Disable Telemetry", "Disables Microsoft telemetry, advertising ID, targeted ads and speech data collection.",
            "Privacy & Telemetry", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Speech_OneCore\Settings\OnlineSpeechPrivacy", "HasAccepted", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Input\TIPC", "Enabled", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\InputPersonalization", "RestrictImplicitInkCollection", "DWord", 1),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\InputPersonalization", "RestrictImplicitTextCollection", "DWord", 1),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\InputPersonalization\TrainedDataStore", "HarvestContacts", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Personalization\Settings", "AcceptedPrivacyPolicy", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Start_TrackProgs", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Siuf\Rules", "NumberOfSIUFInPeriod", "DWord", 0),
            }),
        new("Disable Activity History", "Erases recent docs, clipboard and run history tracking.",
            "Privacy & Telemetry", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System", "EnableActivityFeed", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System", "UploadUserActivities", "DWord", 0),
            }),
        new("Disable Location Tracking", "Denies location access for apps and disables sensor permission.",
            "Privacy & Telemetry", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location", "Value", "String", "Deny"),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", "SensorPermissionState", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\Maps", "AutoUpdateEnabled", "DWord", 0),
            }),
        new("Disable Delivery Optimization", "Stops Windows using your bandwidth to upload updates to other PCs.",
            "Privacy & Telemetry", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DeliveryOptimization", "DODownloadMode", "DWord", 0),
            }),
        new("Disable Consumer Features", "Stops promoted app installs and Store content suggestions.",
            "Privacy & Telemetry", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableWindowsConsumerFeatures", "DWord", 1),
            }),
        new("Disable WPBT", "Prevents your PC vendor from running programs at boot (anti-theft, forced software).",
            "Privacy & Telemetry", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager", "DisableWpbtExecution", "DWord", 1),
            }),
        new("Set Time to UTC", "Fixes clock drift when dual booting with Linux.",
            "Privacy & Telemetry", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\TimeZoneInformation", "RealTimeIsUniversal", "QWord", (long)1),
            }),

        // ===== Explorer & Taskbar =====
        new("Show File Extensions", "Shows .exe, .png etc. in File Explorer.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", "DWord", 0),
            }),
        new("Show Hidden Files", "Reveals hidden files in File Explorer.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", "DWord", 1),
            }),
        new("Dark Theme", "Enables dark mode for the system and apps.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "DWord", 0),
                new(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", "DWord", 0),
            }),
        new("Disable Lock Screen", "Skips the lock screen and goes straight to sign-in.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Personalization", "NoLockScreen", "DWord", 1),
            }),
        new("Hide Start Menu Recommendations", "Removes the recommended section from the Start menu.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PolicyManager\current\device\Start", "HideRecommendedSection", "DWord", 1),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PolicyManager\current\device\Education", "IsEducationEnvironment", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Explorer", "HideRecommendedSection", "DWord", 1),
            }),
        new("Disable Bing in Start Search", "Removes Bing web results from Start menu search.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", "DWord", 0),
            }),
        new("End Task with Right Click", "Adds an End Task option when right-clicking a program on the taskbar.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings", "TaskbarEndTask", "DWord", 1),
            }),
        new("Taskbar Icons Left", "Aligns taskbar icons to the left like Windows 10.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl", "DWord", 0),
            }),
        new("Hide Widgets Button", "Removes the widgets button from the taskbar.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarMn", "DWord", 0),
            }),
        new("Classic Right-Click Menu", "Restores the full classic context menu in File Explorer.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", "", "String", ""),
                new(@"HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", "", "String", ""),
            }),
        new("Enable Long Paths", "Supports file paths longer than 260 characters.",
            "Explorer & Taskbar", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem", "LongPathsEnabled", "DWord", 1),
            }),

        // ===== Gaming & Performance =====
        new("Game Mode", "Lets Windows prioritize system resources for games.",
            "Gaming & Performance", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Microsoft\GameBar", "AllowAutoGameMode", "DWord", 1),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\GameBar", "AutoGameModeEnabled", "DWord", 1),
            }),
        new("Disable Fullscreen Optimizations", "Disables FSO for all apps (can help with exclusive fullscreen games).",
            "Gaming & Performance", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\System\GameConfigStore", "GameDVR_DXGIHonorFSEWindowsCompatible", "DWord", 1),
            }),
        new("Disable Background Apps", "Stops Microsoft Store apps from running in the background.",
            "Gaming & Performance", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled", "DWord", 1),
            }),
        new("Disable Mouse Acceleration", "Removes mouse acceleration for consistent aiming.",
            "Gaming & Performance", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseSpeed", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseThreshold1", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseThreshold2", "DWord", 0),
            }),
        new("Disable Multiplane Overlay", "Can fix stutter caused by overlay composition issues on some GPUs.",
            "Gaming & Performance", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Dwm", "OverlayTestMode", "DWord", 5),
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "DisableOverlays", "DWord", 1),
            }),
        new("Visual Effects: Best Performance", "Turns off animations and eye candy for snappier UI.",
            "Gaming & Performance", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Control Panel\Desktop", "DragFullWindows", "String", "0"),
                new(@"HKEY_CURRENT_USER\Control Panel\Desktop", "MenuShowDelay", "String", "0"),
                new(@"HKEY_CURRENT_USER\Control Panel\Desktop\WindowMetrics", "MinAnimate", "String", "0"),
                new(@"HKEY_CURRENT_USER\Control Panel\Keyboard", "KeyboardDelay", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewAlphaSelect", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewShadow", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAnimations", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", "DWord", 3),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "EnableAeroPeek", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarMn", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowTaskViewButton", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search", "SearchboxTaskbarMode", "DWord", 0),
            }),
        new("Disable Hibernation", "Disables hibernation and removes hiberfil.sys (saves disk space).",
            "Gaming & Performance", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HibernateEnabled", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings", "ShowHibernateOption", "DWord", 0),
            }),
        new("Disable Sticky Keys", "Stops the Shift-pressed-5-times Sticky Keys prompt.",
            "Gaming & Performance", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Control Panel\Accessibility\StickyKeys", "Flags", "DWord", 506),
            }),

        // ===== Hardware & Gaming =====
        new("Enable Hardware Accelerated GPU Scheduling", "Lets the GPU manage its own scheduling, reducing CPU overhead in games. Requires reboot.",
            "Hardware & Gaming", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\DirectX\GraphicsSettings", "HwSchMode", "DWord", 2),
            }),
        new("Disable Game DVR Recording", "Disables Game Bar background recording, a known source of micro-stutter in games.",
            "Hardware & Gaming", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\System\GameConfigStore", "GameDVR_Enabled", "DWord", 0),
                new(@"HKEY_CURRENT_USER\System\GameConfigStore", "GameDVR_FSEBehaviorMode", "DWord", 2),
            }),
        new("High Performance Power Plan", "Switches Windows to the High Performance power plan so the CPU never downclocks.",
            "Hardware & Gaming", Kind: "PowerPlan"),
        new("Disable Memory Integrity (HVCI)", "Disables VBS memory integrity. Can improve FPS and frame pacing on older CPUs. Security tradeoff, requires reboot.",
            "Hardware & Gaming", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", "DWord", 0),
            }),
        new("Disable CPU Mitigations", "Disables Spectre/Meltdown mitigations for a small CPU gain on older CPUs (pre-12th gen). Security tradeoff, requires reboot.",
            "Hardware & Gaming", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "CpuMitigations", "DWord", 0),
            }),

        // ===== Advanced =====
        new("Edge Debloat", "Disables Edge telemetry, shopping assistant, Rewards, first-run experience and more.",
            "Advanced", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\EdgeUpdate", "CreateDesktopShortcutDefault", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "PersonalizationReportingEnabled", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge\ExtensionInstallBlocklist", "1", "String", "ofefcgjbeghpigppfmkologfjadafddi"),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "ShowRecommendationsEnabled", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "HideFirstRunExperience", "DWord", 1),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "UserFeedbackAllowed", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "ConfigureDoNotTrack", "DWord", 1),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "AlternateErrorPagesEnabled", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "EdgeCollectionsEnabled", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "EdgeShoppingAssistantEnabled", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "MicrosoftEdgeInsiderPromotionEnabled", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "ShowMicrosoftRewards", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "WebWidgetAllowed", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "DiagnosticData", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "EdgeAssetDeliveryServiceEnabled", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "WalletDonationEnabled", "DWord", 0),
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "DefaultBrowserSettingsCampaignEnabled", "DWord", 0),
            }),
        new("Disable Storage Sense", "Prevents Windows from auto-deleting temp files.",
            "Advanced", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy", "01", "DWord", 0),
            }),
        new("Disable Reserved Storage", "Frees 7-10 GB held for updates (recommended only on small drives).",
            "Advanced", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\ReserveManager", "ShippedWithReserves", "DWord", 0),
            }),
        new("File Explorer Home & Gallery", "Removes Home and Gallery from Explorer and opens This PC by default.",
            "Advanced", new List<RegValue>
            {
                new(@"HKEY_CURRENT_USER\Software\Classes\CLSID\{f874310e-b6b7-47dc-bc84-b9e6b38f5903}", "System.IsPinnedToNameSpaceTree", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Classes\CLSID\{e88865ea-0e1c-4e20-9aa6-edcd0212c87c}", "System.IsPinnedToNameSpaceTree", "DWord", 0),
                new(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo", "DWord", 1),
            }),
        new("Prefer IPv4 over IPv6", "Can improve latency on networks without IPv6.",
            "Advanced", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters", "DisabledComponents", "DWord", 32),
            }),
        new("Disable Teredo", "Disables Teredo tunneling, which can cause latency in some games.",
            "Advanced", new List<RegValue>
            {
                new(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters", "DisabledComponents", "DWord", 1),
            }),
        new("Create Restore Point", "Creates a system restore point so you can revert tweaks.",
            "Advanced", Kind: "RestorePoint"),
        new("Disk Cleanup", "Runs Disk Cleanup and removes old Windows updates.",
            "Advanced", Kind: "CleanDisk"),
        new("Remove Temp Files", "Erases TEMP folders for all users.",
            "Advanced", Kind: "CleanTemp"),
    };

    public static readonly List<string> Categories = new()
    {
        "Privacy & Telemetry",
        "Explorer & Taskbar",
        "Gaming & Performance",
        "Hardware & Gaming",
        "Advanced",
    };

    public static AppTweak? FindTweak(string name) =>
        Tweaks.Find(t => string.Equals(t.Name, name, System.StringComparison.OrdinalIgnoreCase));
}

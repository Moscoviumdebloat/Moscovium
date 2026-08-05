using System.Collections.Generic;

namespace MoscoviumThree.Models;

/// <summary>
/// Serializable checklist for the one-click PC setup automation.
/// </summary>
public class SetupProfile
{
    public const int CurrentVersion = 1;

    public int Version { get; set; } = CurrentVersion;

    /// <summary>App <see cref="SetupApp.Id"/>s selected by the user.</summary>
    public List<string> WingetApps { get; set; } = new();

    /// <summary><see cref="AppTweak.Name"/>s selected by the user.</summary>
    public List<string> Tweaks { get; set; } = new();

    public bool RunWindowsUpdate { get; set; }
    public bool UpgradeAllApps { get; set; }
    public bool InstallVCRuntimes { get; set; }
    public bool RunChrisTitus { get; set; }
    public bool RunRaphi { get; set; }
}

/// <summary>
/// One installable app in the setup automation.
/// Either a winget package (WingetId, optionally restricted to a Source like "msstore"),
/// a direct installer download (DownloadUrl), a ZIP that gets extracted and has its
/// installer run (ZipUrl), or a PowerShell bootstrap script (ScriptUrl).
/// DownloadUrl may additionally have a ResolvePageUrl + ResolvePattern: the latest direct
/// link is scraped from the vendor page at setup time, falling back to DownloadUrl.
/// </summary>
public record SetupApp(
    string Id,
    string Name,
    string WingetId,
    string Category,
    string? Description = null,
    string? Source = null,
    string? DownloadUrl = null,
    string? ZipUrl = null,
    string? ScriptUrl = null,
    string? ResolvePageUrl = null,
    string? ResolvePattern = null)
{
    public bool IsDownload => !string.IsNullOrEmpty(DownloadUrl);
    public bool IsZip => !string.IsNullOrEmpty(ZipUrl);
    public bool IsScript => !string.IsNullOrEmpty(ScriptUrl);
}

public static class SetupCatalog
{
    public static readonly List<SetupApp> Apps = new()
    {
        // Browsers
        new("Google.Chrome", "Google Chrome", "Google.Chrome", "Browsers", "The classic Google browser"),
        new("Mozilla.Firefox", "Mozilla Firefox", "Mozilla.Firefox", "Browsers", "Privacy-focused browser from Mozilla"),
        new("Brave.Brave", "Brave", "Brave.Brave", "Browsers", "Chromium browser with built-in ad blocking"),
        new("ImputNet.Helium", "Helium", "ImputNet.Helium", "Browsers", "Lightweight, fast Chromium browser"),
        new("Vivaldi.Vivaldi", "Vivaldi", "Vivaldi.Vivaldi", "Browsers", "Power-user Chromium browser, highly customizable"),
        new("Opera.Opera", "Opera", "Opera.Opera", "Browsers", "Feature-packed Chromium browser"),
        new("Ablaze.Floorp", "Floorp", "Ablaze.Floorp", "Browsers", "Customizable Firefox fork"),
        new("LibreWolf.LibreWolf", "LibreWolf", "LibreWolf.LibreWolf", "Browsers", "Privacy-hardened Firefox fork"),
        new("TheBrowserCompany.Arc", "Arc", "TheBrowserCompany.Arc", "Browsers", "Chromium browser with Spaces & tabs"),

        // Gaming & Media
        new("Valve.Steam", "Steam", "Valve.Steam", "Gaming & Media", "PC game store & library"),
        new("EpicGames.EpicGamesLauncher", "Epic Games Launcher", "EpicGames.EpicGamesLauncher", "Gaming & Media", "Epic store & game launcher"),
        new("ElectronicArts.EADesktop", "EA App", "ElectronicArts.EADesktop", "Gaming & Media", "EA games launcher"),
        new("Discord.Discord", "Discord", "Discord.Discord", "Gaming & Media", "Chat & voice for communities"),
        new("Discord.Discord.PTB", "Discord PTB", "Discord.Discord.PTB", "Gaming & Media", "Discord public test build"),
        new("Telegram.TelegramDesktop", "Telegram", "Telegram.TelegramDesktop", "Gaming & Media", "Fast, secure messaging"),
        new("TeamSpeakSystems.TeamSpeakClient.Beta.6", "TeamSpeak 6 (Beta)", "TeamSpeakSystems.TeamSpeakClient.Beta.6", "Gaming & Media", "Low-latency voice chat, beta"),
        new("Spotify.Spotify", "Spotify", "Spotify.Spotify", "Gaming & Media", "Music streaming"),
        new("OBSProject.OBSStudio", "OBS Studio", "OBSProject.OBSStudio", "Gaming & Media", "Streaming & screen recording"),
        new("qBittorrent.qBittorrent", "qBittorrent", "qBittorrent.qBittorrent", "Gaming & Media", "Open-source torrent client"),
        new("Blizzard.BattleNet", "Battle.net", "Blizzard.BattleNet", "Gaming & Media", "Blizzard games launcher"),
        new("Mojang.MinecraftLauncher", "Minecraft Launcher", "Mojang.MinecraftLauncher", "Gaming & Media", "Minecraft game launcher"),
        new("GOG.Galaxy", "GOG Galaxy", "GOG.Galaxy", "Gaming & Media", "DRM-free games launcher"),
        new("Playnite.Playnite", "Playnite", "Playnite.Playnite", "Gaming & Media", "Unified game library manager"),
        new("Ubisoft.Connect", "Ubisoft Connect", "Ubisoft.Connect", "Gaming & Media", "Ubisoft games launcher"),
        new("HeroicGamesLauncher.HeroicGamesLauncher", "Heroic Games Launcher", "HeroicGamesLauncher.HeroicGamesLauncher", "Gaming & Media", "Open-source Epic & GOG launcher"),
        new("ItchIo.Itch", "itch.io", "ItchIo.Itch", "Gaming & Media", "Indie games launcher"),
        new("Daum.PotPlayer", "PotPlayer", "Daum.PotPlayer", "Gaming & Media", "Feature-rich video player"),
        new("PeterPawlowski.foobar2000", "foobar2000", "PeterPawlowski.foobar2000", "Gaming & Media", "Lightweight audio player"),
        new("rocksdanister.LivelyWallpaper", "Lively Wallpaper", "rocksdanister.LivelyWallpaper", "Gaming & Media", "Animated desktop wallpapers"),

        // Creativity & Media
        new("KDE.Krita", "Krita", "KDE.Krita", "Creativity & Media", "Digital painting app"),
        new("GIMP.GIMP", "GIMP", "GIMP.GIMP", "Creativity & Media", "Open-source image editor"),
        new("BlenderFoundation.Blender", "Blender", "BlenderFoundation.Blender", "Creativity & Media", "3D modeling & animation"),
        new("HandBrake.HandBrake", "HandBrake", "HandBrake.HandBrake", "Creativity & Media", "Video transcoder"),
        new("Audacity.Audacity", "Audacity", "Audacity.Audacity", "Creativity & Media", "Audio editor & recorder"),

        // IDEs & Editors
        new("Microsoft.VisualStudioCode", "Visual Studio Code", "Microsoft.VisualStudioCode", "IDEs & Editors", "Microsoft's code editor"),
        new("Anysphere.Cursor", "Cursor", "Anysphere.Cursor", "IDEs & Editors", "AI-powered code editor"),
        new("Google.Antigravity", "Antigravity", "Google.Antigravity", "IDEs & Editors", "Google AI IDE"),
        new("Microsoft.VisualStudio.2022.Community", "Visual Studio 2022 (Community)", "Microsoft.VisualStudio.2022.Community", "IDEs & Editors", "Full-featured .NET IDE"),
        new("Google.AndroidStudio", "Android Studio", "Google.AndroidStudio", "IDEs & Editors", "Android development IDE"),
        new("JetBrains.IntelliJIDEA.Community", "IntelliJ IDEA (Community)", "JetBrains.IntelliJIDEA.Community", "IDEs & Editors", "Java/Kotlin IDE"),
        new("JetBrains.PyCharm.Community", "PyCharm (Community)", "JetBrains.PyCharm.Community", "IDEs & Editors", "Python IDE"),
        new("JetBrains.WebStorm", "WebStorm", "JetBrains.WebStorm", "IDEs & Editors", "JavaScript IDE"),
        new("JetBrains.Rider", "Rider", "JetBrains.Rider", "IDEs & Editors", ".NET IDE"),
        new("JetBrains.CLion", "CLion", "JetBrains.CLion", "IDEs & Editors", "C/C++ IDE"),
        new("JetBrains.GoLand", "GoLand", "JetBrains.GoLand", "IDEs & Editors", "Go IDE"),
        new("JetBrains.DataGrip", "DataGrip", "JetBrains.DataGrip", "IDEs & Editors", "Database IDE"),
        new("Notepad++.Notepad++", "Notepad++", "Notepad++.Notepad++", "IDEs & Editors", "Lightweight text/code editor"),
        new("SublimeHQ.SublimeText.4", "Sublime Text 4", "SublimeHQ.SublimeText.4", "IDEs & Editors", "Fast code editor"),
        new("ZedIndustries.Zed", "Zed", "ZedIndustries.Zed", "IDEs & Editors", "High-performance code editor"),
        new("vim.vim", "Vim", "vim.vim", "IDEs & Editors", "Terminal text editor"),
        new("Neovim.Neovim", "Neovim", "Neovim.Neovim", "IDEs & Editors", "Modern Vim, extensible"),

        // Languages & Runtimes
        new("Python.Python.3.13", "Python 3.13", "Python.Python.3.13", "Languages & Runtimes", "Python language runtime"),
        new("OpenJS.NodeJS.LTS", "Node.js (LTS)", "OpenJS.NodeJS.LTS", "Languages & Runtimes", "JavaScript runtime"),
        new("Git.Git", "Git", "Git.Git", "Languages & Runtimes", "Version control"),
        new("Rustlang.Rustup", "Rust (rustup)", "Rustlang.Rustup", "Languages & Runtimes", "Rust toolchain installer"),
        new("GoLang.Go", "Go", "GoLang.Go", "Languages & Runtimes", "Go language toolchain"),
        new("EclipseAdoptium.Temurin.21.JDK", "OpenJDK 21 (Temurin)", "EclipseAdoptium.Temurin.21.JDK", "Languages & Runtimes", "Java runtime & SDK"),
        new("Microsoft.DotNet.SDK.10", ".NET SDK", "Microsoft.DotNet.SDK.10", "Languages & Runtimes", ".NET development SDK"),

        // Dev Tools & AI
        new("Docker.DockerDesktop", "Docker Desktop", "Docker.DockerDesktop", "Dev Tools & AI", "Containers & dev environments"),
        new("Microsoft.WindowsTerminal", "Windows Terminal", "Microsoft.WindowsTerminal", "Dev Tools & AI", "Modern terminal"),
        new("GitHub.GitHubDesktop", "GitHub Desktop", "GitHub.GitHubDesktop", "Dev Tools & AI", "Git GUI"),
        new("JetBrains.Toolbox", "JetBrains Toolbox", "JetBrains.Toolbox", "Dev Tools & AI", "JetBrains IDE manager"),
        new("Eugeny.Tabby", "Tabby (Terminus SSH)", "Eugeny.Tabby", "Dev Tools & AI", "Modern SSH & terminal client"),
        new("Termius.Termius", "Termius (SSH client)", "", "Dev Tools & AI", "Cross-platform SSH client",
            DownloadUrl: "https://download.termius.com/windows/Install%20Termius.exe"),
        new("ElementLabs.LMStudio", "LM Studio", "ElementLabs.LMStudio", "Dev Tools & AI", "Local LLM chat & models"),
        new("Anthropic.ClaudeCode", "Claude Code (CLI)", "Anthropic.ClaudeCode", "Dev Tools & AI", "Anthropic AI coding agent"),
        new("OpenAI.Codex", "OpenAI Codex (CLI)", "OpenAI.Codex", "Dev Tools & AI", "OpenAI coding agent"),
        new("T3Tools.T3Code", "T3 Code", "T3Tools.T3Code", "Dev Tools & AI", "Multi-provider coding agent"),
        new("Massgrave.MAS", "MAS - Windows & Office Activation", "", "Dev Tools & AI", "Activation scripts for Windows/Office (Massgrave)",
            ScriptUrl: "https://get.activated.win"),
        new("Postman.Postman", "Postman (API)", "Postman.Postman", "Dev Tools & AI", "API testing client"),
        new("Insomnia.Insomnia", "Insomnia (API)", "Insomnia.Insomnia", "Dev Tools & AI", "API design & testing"),
        new("DevToys-app.DevToys", "DevToys", "DevToys-app.DevToys", "Dev Tools & AI", "Developer utility toolbox"),
        new("Ollama.Ollama", "Ollama (Local LLMs)", "Ollama.Ollama", "Dev Tools & AI", "Run LLMs locally"),
        new("WinSCP.WinSCP", "WinSCP", "WinSCP.WinSCP", "Dev Tools & AI", "SFTP/FTP file client"),
        new("Microsoft.Sysinternals.Suite", "Sysinternals Suite", "Microsoft.Sysinternals.Suite", "Dev Tools & AI", "Advanced Windows tools"),

        // Utilities
        new("7zip.7zip", "7-Zip", "7zip.7zip", "Utilities", "Archive manager"),
        new("VideoLAN.VLC", "VLC Media Player", "VideoLAN.VLC", "Utilities", "Universal media player"),
        new("Microsoft.PowerToys", "PowerToys", "Microsoft.PowerToys", "Utilities", "Windows power utilities"),
        new("voidtools.Everything", "Everything Search", "voidtools.Everything", "Utilities", "Instant file search"),
        new("ShareX.ShareX", "ShareX", "ShareX.ShareX", "Utilities", "Screenshots & screen capture"),
        new("dotPDN.PaintDotNet", "Paint.NET", "dotPDN.PaintDotNet", "Utilities", "Image editor"),
        new("CodecGuide.K-LiteCodecPack.Standard", "K-Lite Codec Pack", "CodecGuide.K-LiteCodecPack.Standard", "Utilities", "Video/audio codecs"),
        new("Rufus.Rufus", "Rufus (USB Imager)", "Rufus.Rufus", "Utilities", "USB boot drive creator"),
        new("CharlesMilette.TranslucentTB", "TranslucentTB", "CharlesMilette.TranslucentTB", "Utilities", "Transparent taskbar"),
        new("Microsoft.Sysinternals.Autoruns", "Autoruns", "Microsoft.Sysinternals.Autoruns", "Utilities", "Startup programs manager"),
        new("WinsiderSS.SystemInformer", "System Informer", "WinsiderSS.SystemInformer", "Utilities", "Advanced task manager"),
        new("rcmaehl.MSEdgeRedirect", "MSEdgeRedirect", "rcmaehl.MSEdgeRedirect", "Utilities", "Redirect Edge links to your browser"),
        new("RevoUninstaller.RevoUninstaller", "Revo Uninstaller", "RevoUninstaller.RevoUninstaller", "Utilities", "Thorough app uninstaller"),
        new("TechNobo.TcNoAccountSwitcher", "TcNo Account Switcher", "TechNobo.TcNoAccountSwitcher", "Utilities", "Switch accounts for games/apps"),
        new("CipherMachines.EnigmaSim", "Enigma Machine Simulator", "", "Utilities", "Classic Enigma machine sim",
            ZipUrl: "https://www.ciphermachinesandcryptology.com/files/EnigmaSim.zip"),
        new("AntibodySoftware.WizTree", "WizTree", "AntibodySoftware.WizTree", "Utilities", "Fast disk usage analyzer"),
        new("WinDirStat.WinDirStat", "WinDirStat", "WinDirStat.WinDirStat", "Utilities", "Disk usage visualizer"),
        new("RamenSoftware.Windhawk", "Windhawk", "RamenSoftware.Windhawk", "Utilities", "Windows customization mods"),
        new("valinet.ExplorerPatcher", "ExplorerPatcher", "valinet.ExplorerPatcher", "Utilities", "Windows 11 taskbar/Explorer tweaks"),
        new("StartIsBack.StartAllBack", "StartAllBack", "StartIsBack.StartAllBack", "Utilities", "Classic Windows 11 Start menu"),
        new("Rainmeter.Rainmeter", "Rainmeter", "Rainmeter.Rainmeter", "Utilities", "Desktop widgets"),
        new("File-New-Project.EarTrumpet", "EarTrumpet", "File-New-Project.EarTrumpet", "Utilities", "Per-app volume control"),
        new("AntoineAflalo.SoundSwitch", "SoundSwitch", "AntoineAflalo.SoundSwitch", "Utilities", "Quick audio device switching"),
        new("QL-Win.QuickLook", "QuickLook", "QL-Win.QuickLook", "Utilities", "Space-preview files"),
        new("Klocman.BulkCrapUninstaller", "Bulk Crap Uninstaller", "Klocman.BulkCrapUninstaller", "Utilities", "Bulk app uninstaller"),
        new("LocalSend.LocalSend", "LocalSend", "LocalSend.LocalSend", "Utilities", "Local file sharing"),
        new("Henry++.simplewall", "simplewall", "Henry++.simplewall", "Utilities", "Simple firewall manager"),
        new("GlassWire.GlassWire", "GlassWire", "GlassWire.GlassWire", "Utilities", "Network usage monitor"),
        new("Malwarebytes.Malwarebytes", "Malwarebytes", "Malwarebytes.Malwarebytes", "Utilities", "Antimalware scanner"),
        new("RustDesk.RustDesk", "RustDesk (Remote Desktop)", "", "Utilities", "Open-source remote desktop",
            DownloadUrl: "https://github.com/rustdesk/rustdesk/releases/download/1.4.9/rustdesk-1.4.9-x86_64.exe",
            ResolvePageUrl: "https://github.com/rustdesk/rustdesk/releases/latest",
            ResolvePattern: "https://github\\.com/rustdesk/rustdesk/releases/download/[^\"' ]+/rustdesk-[^\"' ]+-x86_64\\.exe"),
        new("JAMSoftware.TreeSize.Free", "TreeSize Free", "JAMSoftware.TreeSize.Free", "Utilities", "Disk space explorer"),
        new("Obsidian.Obsidian", "Obsidian (Notes)", "Obsidian.Obsidian", "Utilities", "Local markdown notes"),
        new("Bitwarden.Bitwarden", "Bitwarden", "Bitwarden.Bitwarden", "Utilities", "Password manager"),
        new("KeePassXCTeam.KeePassXC", "KeePassXC", "KeePassXCTeam.KeePassXC", "Utilities", "Offline password manager"),
        new("Dropbox.Dropbox", "Dropbox", "Dropbox.Dropbox", "Utilities", "Cloud file sync"),
        new("Mega.MEGASync", "MEGA Sync", "Mega.MEGASync", "Utilities", "Encrypted cloud storage"),
        new("TheDocumentFoundation.LibreOffice", "LibreOffice", "TheDocumentFoundation.LibreOffice", "Utilities", "Free office suite"),
        new("ONLYOFFICE.DesktopEditors", "OnlyOffice", "ONLYOFFICE.DesktopEditors", "Utilities", "Office suite with doc editors"),

        // Drivers & Hardware
        new("Wagnardsoft.DisplayDriverUninstaller", "Display Driver Uninstaller", "Wagnardsoft.DisplayDriverUninstaller", "Drivers & Hardware", "Clean GPU driver removal"),
        new("CrystalDewWorld.CrystalDiskInfo", "CrystalDiskInfo", "CrystalDewWorld.CrystalDiskInfo", "Drivers & Hardware", "SSD/HDD health monitor"),
        new("REALiX.HWiNFO", "HWiNFO", "REALiX.HWiNFO", "Drivers & Hardware", "System monitoring & sensors"),
        new("CPUID.CPU-Z", "CPU-Z", "CPUID.CPU-Z", "Drivers & Hardware", "CPU & system info"),
        new("Guru3D.Afterburner", "MSI Afterburner", "Guru3D.Afterburner", "Drivers & Hardware", "GPU overclocking & overlay"),
        new("Nvidia.NvidiaApp", "NVIDIA App", "", "Drivers & Hardware", "NVIDIA drivers & settings",
            DownloadUrl: "https://us.download.nvidia.com/nvapp/client/11.0.8.299/NVIDIA_app_v11.0.8.299.exe",
            ResolvePageUrl: "https://www.nvidia.com/en-us/software/nvidia-app/",
            ResolvePattern: "https?://us\\.download\\.nvidia\\.com/nvapp/[^\"' ]+\\.exe"),
        new("AMD.Adrenalin", "AMD Adrenalin Edition", "", "Drivers & Hardware", "AMD drivers & settings",
            DownloadUrl: "https://drivers.amd.com/drivers/installer/26.10/whql/amd-software-adrenalin-edition-26.7.1-minimalsetup-260724_web.exe",
            ResolvePageUrl: "https://www.amd.com/en/support/download/drivers.html",
            ResolvePattern: "https?://drivers\\.amd\\.com/drivers/installer/[^\"' ]+\\.exe"),
        new("AMD.RyzenMaster", "AMD Ryzen Master", "", "Drivers & Hardware", "AMD CPU overclocking",
            DownloadUrl: "https://drivers.amd.com/drivers/amd_ryzen_master_3.1.1.5502.exe",
            ResolvePageUrl: "https://www.amd.com/en/products/software/ryzen-master.html",
            ResolvePattern: "https?://drivers\\.amd\\.com/drivers/amd_ryzen_master_[^\"' ]+\\.exe"),
        new("TechPowerUp.GPU-Z", "GPU-Z", "TechPowerUp.GPU-Z", "Drivers & Hardware", "GPU information"),
        new("Rem0o.FanControl", "FanControl", "Rem0o.FanControl", "Drivers & Hardware", "Custom fan curves"),
        new("OpenRGB.OpenRGB", "OpenRGB", "OpenRGB.OpenRGB", "Drivers & Hardware", "Unified RGB control"),
        new("Resplendence.LatencyMon", "LatencyMon", "Resplendence.LatencyMon", "Drivers & Hardware", "DPC latency analyzer"),
        new("Logitech.OptionsPlus", "Logi Options+", "Logitech.OptionsPlus", "Drivers & Hardware", "Logitech device settings"),
        new("RazerInc.RazerInstaller.Synapse4", "Razer Synapse 4", "RazerInc.RazerInstaller.Synapse4", "Drivers & Hardware", "Razer device settings"),
        new("SteelSeries.GG", "SteelSeries GG", "SteelSeries.GG", "Drivers & Hardware", "SteelSeries engine & Moments"),
    };

    public static readonly List<string> Categories = new()
    {
        "Browsers",
        "Gaming & Media",
        "Creativity & Media",
        "IDEs & Editors",
        "Languages & Runtimes",
        "Dev Tools & AI",
        "Utilities",
        "Drivers & Hardware",
    };

    public static SetupApp? FindById(string id) =>
        Apps.Find(a => string.Equals(a.Id, id, System.StringComparison.OrdinalIgnoreCase));

    public static string? GetAppName(string id) => FindById(id)?.Name;
}

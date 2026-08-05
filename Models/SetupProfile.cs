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
/// a direct installer download (DownloadUrl), or a ZIP that gets extracted and has its
/// installer run (ZipUrl).
/// DownloadUrl may additionally have a ResolvePageUrl + ResolvePattern: the latest direct
/// link is scraped from the vendor page at setup time, falling back to DownloadUrl.
/// </summary>
public record SetupApp(
    string Id,
    string Name,
    string WingetId,
    string Category,
    string? Source = null,
    string? DownloadUrl = null,
    string? ZipUrl = null,
    string? ResolvePageUrl = null,
    string? ResolvePattern = null)
{
    public bool IsDownload => !string.IsNullOrEmpty(DownloadUrl);
    public bool IsZip => !string.IsNullOrEmpty(ZipUrl);
}

public static class SetupCatalog
{
    public static readonly List<SetupApp> Apps = new()
    {
        // Browsers
        new("Google.Chrome", "Google Chrome", "Google.Chrome", "Browsers"),
        new("Mozilla.Firefox", "Mozilla Firefox", "Mozilla.Firefox", "Browsers"),

        // Gaming & Media
        new("Valve.Steam", "Steam", "Valve.Steam", "Gaming & Media"),
        new("EpicGames.EpicGamesLauncher", "Epic Games Launcher", "EpicGames.EpicGamesLauncher", "Gaming & Media"),
        new("ElectronicArts.EADesktop", "EA App", "ElectronicArts.EADesktop", "Gaming & Media"),
        new("Discord.Discord", "Discord", "Discord.Discord", "Gaming & Media"),
        new("Discord.Discord.PTB", "Discord PTB", "Discord.Discord.PTB", "Gaming & Media"),
        new("Telegram.TelegramDesktop", "Telegram", "Telegram.TelegramDesktop", "Gaming & Media"),
        new("TeamSpeakSystems.TeamSpeakClient.Beta.6", "TeamSpeak 6 (Beta)", "TeamSpeakSystems.TeamSpeakClient.Beta.6", "Gaming & Media"),
        new("Spotify.Spotify", "Spotify", "Spotify.Spotify", "Gaming & Media"),
        new("OBSProject.OBSStudio", "OBS Studio", "OBSProject.OBSStudio", "Gaming & Media"),
        new("qBittorrent.qBittorrent", "qBittorrent", "qBittorrent.qBittorrent", "Gaming & Media"),

        // IDEs & Editors
        new("Microsoft.VisualStudioCode", "Visual Studio Code", "Microsoft.VisualStudioCode", "IDEs & Editors"),
        new("Anysphere.Cursor", "Cursor", "Anysphere.Cursor", "IDEs & Editors"),
        new("Microsoft.VisualStudio.2022.Community", "Visual Studio 2022 (Community)", "Microsoft.VisualStudio.2022.Community", "IDEs & Editors"),
        new("Google.AndroidStudio", "Android Studio", "Google.AndroidStudio", "IDEs & Editors"),
        new("JetBrains.IntelliJIDEA.Community", "IntelliJ IDEA (Community)", "JetBrains.IntelliJIDEA.Community", "IDEs & Editors"),
        new("JetBrains.PyCharm.Community", "PyCharm (Community)", "JetBrains.PyCharm.Community", "IDEs & Editors"),
        new("JetBrains.WebStorm", "WebStorm", "JetBrains.WebStorm", "IDEs & Editors"),
        new("JetBrains.Rider", "Rider", "JetBrains.Rider", "IDEs & Editors"),
        new("JetBrains.CLion", "CLion", "JetBrains.CLion", "IDEs & Editors"),
        new("JetBrains.GoLand", "GoLand", "JetBrains.GoLand", "IDEs & Editors"),
        new("JetBrains.DataGrip", "DataGrip", "JetBrains.DataGrip", "IDEs & Editors"),
        new("Notepad++.Notepad++", "Notepad++", "Notepad++.Notepad++", "IDEs & Editors"),
        new("SublimeHQ.SublimeText.4", "Sublime Text 4", "SublimeHQ.SublimeText.4", "IDEs & Editors"),
        new("ZedIndustries.Zed", "Zed", "ZedIndustries.Zed", "IDEs & Editors"),
        new("vim.vim", "Vim", "vim.vim", "IDEs & Editors"),
        new("Neovim.Neovim", "Neovim", "Neovim.Neovim", "IDEs & Editors"),

        // Languages & Runtimes
        new("Python.Python.3.13", "Python 3.13", "Python.Python.3.13", "Languages & Runtimes"),
        new("OpenJS.NodeJS.LTS", "Node.js (LTS)", "OpenJS.NodeJS.LTS", "Languages & Runtimes"),
        new("Git.Git", "Git", "Git.Git", "Languages & Runtimes"),
        new("Rustlang.Rustup", "Rust (rustup)", "Rustlang.Rustup", "Languages & Runtimes"),
        new("GoLang.Go", "Go", "GoLang.Go", "Languages & Runtimes"),
        new("EclipseAdoptium.Temurin.21.JDK", "OpenJDK 21 (Temurin)", "EclipseAdoptium.Temurin.21.JDK", "Languages & Runtimes"),
        new("Microsoft.DotNet.SDK.10", ".NET SDK", "Microsoft.DotNet.SDK.10", "Languages & Runtimes"),

        // Dev Tools & AI
        new("Docker.DockerDesktop", "Docker Desktop", "Docker.DockerDesktop", "Dev Tools & AI"),
        new("Microsoft.WindowsTerminal", "Windows Terminal", "Microsoft.WindowsTerminal", "Dev Tools & AI"),
        new("GitHub.GitHubDesktop", "GitHub Desktop", "GitHub.GitHubDesktop", "Dev Tools & AI"),
        new("JetBrains.Toolbox", "JetBrains Toolbox", "JetBrains.Toolbox", "Dev Tools & AI"),
        new("Eugeny.Tabby", "Tabby (Terminus SSH)", "Eugeny.Tabby", "Dev Tools & AI"),
        new("Termius.Termius", "Termius (SSH client)", "", "Dev Tools & AI",
            DownloadUrl: "https://download.termius.com/windows/Install%20Termius.exe"),
        new("ElementLabs.LMStudio", "LM Studio", "ElementLabs.LMStudio", "Dev Tools & AI"),

        // Utilities
        new("7zip.7zip", "7-Zip", "7zip.7zip", "Utilities"),
        new("VideoLAN.VLC", "VLC Media Player", "VideoLAN.VLC", "Utilities"),
        new("Microsoft.PowerToys", "PowerToys", "Microsoft.PowerToys", "Utilities"),
        new("voidtools.Everything", "Everything Search", "voidtools.Everything", "Utilities"),
        new("ShareX.ShareX", "ShareX (Screenshots)", "ShareX.ShareX", "Utilities"),
        new("dotPDN.PaintDotNet", "Paint.NET", "dotPDN.PaintDotNet", "Utilities"),
        new("CodecGuide.K-LiteCodecPack.Standard", "K-Lite Codec Pack", "CodecGuide.K-LiteCodecPack.Standard", "Utilities"),
        new("Rufus.Rufus", "Rufus (USB Imager)", "Rufus.Rufus", "Utilities"),
        new("CharlesMilette.TranslucentTB", "TranslucentTB", "CharlesMilette.TranslucentTB", "Utilities"),
        new("Microsoft.Sysinternals.Autoruns", "Autoruns", "Microsoft.Sysinternals.Autoruns", "Utilities"),
        new("WinsiderSS.SystemInformer", "System Informer", "WinsiderSS.SystemInformer", "Utilities"),
        new("rcmaehl.MSEdgeRedirect", "MSEdgeRedirect", "rcmaehl.MSEdgeRedirect", "Utilities"),
        new("RevoUninstaller.RevoUninstaller", "Revo Uninstaller", "RevoUninstaller.RevoUninstaller", "Utilities"),
        new("TechNobo.TcNoAccountSwitcher", "TcNo Account Switcher", "TechNobo.TcNoAccountSwitcher", "Utilities"),
        new("CipherMachines.EnigmaSim", "Enigma Machine Simulator", "", "Utilities",
            ZipUrl: "https://www.ciphermachinesandcryptology.com/files/EnigmaSim.zip"),

        // Drivers & Hardware
        new("Wagnardsoft.DisplayDriverUninstaller", "Display Driver Uninstaller", "Wagnardsoft.DisplayDriverUninstaller", "Drivers & Hardware"),
        new("CrystalDewWorld.CrystalDiskInfo", "CrystalDiskInfo", "CrystalDewWorld.CrystalDiskInfo", "Drivers & Hardware"),
        new("REALiX.HWiNFO", "HWiNFO", "REALiX.HWiNFO", "Drivers & Hardware"),
        new("CPUID.CPU-Z", "CPU-Z", "CPUID.CPU-Z", "Drivers & Hardware"),
        new("Guru3D.Afterburner", "MSI Afterburner", "Guru3D.Afterburner", "Drivers & Hardware"),
        new("Nvidia.NvidiaApp", "NVIDIA App", "", "Drivers & Hardware",
            DownloadUrl: "https://us.download.nvidia.com/nvapp/client/11.0.8.299/NVIDIA_app_v11.0.8.299.exe",
            ResolvePageUrl: "https://www.nvidia.com/en-us/software/nvidia-app/",
            ResolvePattern: "https?://us\\.download\\.nvidia\\.com/nvapp/[^\"' ]+\\.exe"),
        new("AMD.Adrenalin", "AMD Adrenalin Edition", "", "Drivers & Hardware",
            DownloadUrl: "https://drivers.amd.com/drivers/whql-amd-software-adrenalin-edition-26.7.1-win11-c.exe",
            ResolvePageUrl: "https://www.amd.com/en/support/download/drivers.html",
            ResolvePattern: "https?://drivers\\.amd\\.com/drivers/installer/[^\"' ]+\\.exe"),
        new("AMD.RyzenMaster", "AMD Ryzen Master", "", "Drivers & Hardware",
            DownloadUrl: "https://drivers.amd.com/drivers/amd_ryzen_master_3.1.1.5502.exe",
            ResolvePageUrl: "https://www.amd.com/en/products/software/ryzen-master.html",
            ResolvePattern: "https?://drivers\\.amd\\.com/drivers/amd_ryzen_master_[^\"' ]+\\.exe"),
        new("TechPowerUp.GPU-Z", "GPU-Z", "TechPowerUp.GPU-Z", "Drivers & Hardware"),
    };

    public static readonly List<string> Categories = new()
    {
        "Browsers",
        "Gaming & Media",
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

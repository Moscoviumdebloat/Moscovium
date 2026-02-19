using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;

namespace MoscoviumThree.Models;

public class AppStoreItem : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _author = string.Empty;
    private string _version = string.Empty;
    private string _downloadUrl = string.Empty;
    private string _iconUrl = string.Empty;
    private bool _isDownloading;
    private double _downloadProgress;
    private string _status = string.Empty;
    private bool _isInstalled;

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetField(ref _description, value);
    }

    public string Author
    {
        get => _author;
        set => SetField(ref _author, value);
    }

    public string Version
    {
        get => _version;
        set => SetField(ref _version, value);
    }

    public string DownloadUrl
    {
        get => _downloadUrl;
        set => SetField(ref _downloadUrl, value);
    }

    public string IconUrl
    {
        get => _iconUrl;
        set => SetField(ref _iconUrl, value);
    }

    public bool IsDownloading
    {
        get => _isDownloading;
        set
        {
            if (SetField(ref _isDownloading, value))
            {
                OnPropertyChanged(nameof(ShowInstallButton));
                OnPropertyChanged(nameof(ShowProgressBar));
            }
        }
    }

    public double DownloadProgress
    {
        get => _downloadProgress;
        set
        {
            if (SetField(ref _downloadProgress, value))
            {
                OnPropertyChanged(nameof(DownloadProgressText));
            }
        }
    }

    public string DownloadProgressText => $"{DownloadProgress:F0}%";

    public string Status
    {
        get => _status;
        set => SetField(ref _status, value);
    }
    
    public bool IsInstalled
    {
        get => _isInstalled;
        set
        {
            if (SetField(ref _isInstalled, value))
            {
                OnPropertyChanged(nameof(ShowInstallButton));
                OnPropertyChanged(nameof(ShowUninstallButton));
            }
        }
    }

    // Computed properties for UI visibility
    public Visibility ShowInstallButton => IsDownloading || IsInstalled ? Visibility.Collapsed : Visibility.Visible;
    public Visibility ShowUninstallButton => IsInstalled && !IsDownloading ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ShowProgressBar => IsDownloading ? Visibility.Visible : Visibility.Collapsed;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

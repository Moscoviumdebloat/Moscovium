using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using MoscoviumThree.Models;

namespace MoscoviumThree.Helpers;

public class GitHubService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://api.github.com";

    public GitHubService()
    {
        _httpClient = new HttpClient();
        // GitHub API requires a User-Agent header
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Moscovium-AppStore");
    }

    public async Task<List<AppStoreItem>> GetAppsFromOrgAsync(string orgName)
    {
        // Refresh headers with token if available
        // Refresh headers with token if available
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        if (!string.IsNullOrEmpty(SettingsHelper.GitHubToken))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {SettingsHelper.GitHubToken}");
        }

        var apps = new List<AppStoreItem>();
        try
        {
            var url = $"{ApiBaseUrl}/orgs/{orgName}/repos?type=public&per_page=100";
            var repos = await _httpClient.GetFromJsonAsync<List<GitHubRepo>>(url);

            if (repos != null)
            {
                foreach (var repo in repos)
                {
                    // Skip archived or empty repos, and internal .github repo
                    if (repo.Archived || repo.Name.Equals(".github", StringComparison.OrdinalIgnoreCase)) 
                    {
                        Debug.WriteLine($"Skipping repo {repo.Name}: Archived={repo.Archived}");
                        continue;
                    }

                    // Try to get latest release for download link
                    GitHubRelease? release = null;
                    try
                    {
                         release = await GetLatestReleaseAsync(repo.Owner.Login, repo.Name);
                    }
                    catch (Exception ex)
                    {
                         Debug.WriteLine($"Failed to get release for {repo.Name}: {ex.Message}");
                    }
                    
                    if (release == null)
                    {
                        Debug.WriteLine($"No release found for {repo.Name}");
                    }
                    
                    // Construct the item
                    var item = new AppStoreItem
                    {
                        Name = repo.Name,
                        Description = repo.Description ?? "No description available.",
                        Author = repo.Owner.Login,
                        Version = release?.TagName ?? "Unknown",
                        DownloadUrl = release?.Assets?.FirstOrDefault(a => a.Name.EndsWith(".exe") || a.Name.EndsWith(".zip") || a.Name.EndsWith(".msi"))?.BrowserDownloadUrl ?? "",
                        Status = "Available",
                        IconUrl = repo.Owner.AvatarUrl
                    };

                    apps.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching repos for {orgName}: {ex.Message}");
            // Log to file for user to see
            try 
            {
                string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MoscoviumGitHubError.txt");
                File.AppendAllText(logPath,($"Error fetching {orgName}: {ex}\n"));
            } catch {}
            
            // Re-throw so the UI knows it failed
            throw;
        }

        return apps;
    }

    private async Task<GitHubRelease?> GetLatestReleaseAsync(string owner, string repo)
    {
        try
        {
            var url = $"{ApiBaseUrl}/repos/{owner}/{repo}/releases/latest";
            return await _httpClient.GetFromJsonAsync<GitHubRelease>(url);
        }
        catch (HttpRequestException ex)
        {
             // 404 means no release usually
             Debug.WriteLine($"GetRelease failed {repo}: {ex.StatusCode}");
             return null;
        }
        catch
        {
            return null;
        }
    }

    // Helper classes for JSON deserialization
    private class GitHubRepo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("owner")]
        public GitHubOwner Owner { get; set; } = new();

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }
    }

    private class GitHubOwner
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = "";
        
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = "";
    }

    private class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = "";

        [JsonPropertyName("assets")]
        public List<GitHubAsset>? Assets { get; set; }
    }

    private class GitHubAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = "";
    }
}

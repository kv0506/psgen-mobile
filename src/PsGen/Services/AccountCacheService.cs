using System.Text.Json;
using PsGen.Mobile.Models;

namespace PsGen.Mobile.Services;

/// <summary>
/// Provides local JSON-file caching for accounts so the UI can load instantly
/// while a background refresh fetches fresh data once per app session.
/// </summary>
public class AccountCacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly string _cacheFilePath =
        Path.Combine(FileSystem.AppDataDirectory, "accounts_cache.json");

    private List<AccountDto>? _memoryCache;
    private bool _refreshedThisSession;

    /// <summary>
    /// Returns cached accounts from memory or disk. Returns an empty list when no cache exists.
    /// </summary>
    public async Task<List<AccountDto>> GetCachedAccountsAsync()
    {
        if (_memoryCache is not null)
            return _memoryCache;

        try
        {
            if (File.Exists(_cacheFilePath))
            {
                var json = await File.ReadAllTextAsync(_cacheFilePath);
                _memoryCache = JsonSerializer.Deserialize<List<AccountDto>>(json, JsonOptions) ?? [];
                return _memoryCache;
            }
        }
        catch
        {
            // Cache is corrupt — treat as empty.
        }

        _memoryCache = [];
        return _memoryCache;
    }

    /// <summary>
    /// Persists accounts to local cache and updates the in-memory copy.
    /// </summary>
    public async Task SaveCacheAsync(List<AccountDto> accounts)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        _memoryCache = accounts;

        try
        {
            var json = JsonSerializer.Serialize(accounts, JsonOptions);
            await File.WriteAllTextAsync(_cacheFilePath, json);
        }
        catch
        {
            // Best-effort; the in-memory copy is still valid.
        }
    }

    /// <summary>
    /// Indicates whether the remote data has already been fetched during this app session.
    /// </summary>
    public bool HasRefreshedThisSession => _refreshedThisSession;

    /// <summary>
    /// Marks the session as having completed a remote refresh.
    /// </summary>
    public void MarkRefreshed() => _refreshedThisSession = true;

    /// <summary>
    /// Clears all cached data (e.g. on logout).
    /// </summary>
    public void ClearCache()
    {
        _memoryCache = null;
        _refreshedThisSession = false;

        try
        {
            if (File.Exists(_cacheFilePath))
                File.Delete(_cacheFilePath);
        }
        catch
        {
            // Best-effort cleanup.
        }
    }
}

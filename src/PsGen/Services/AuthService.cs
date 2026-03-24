using System.Text.Json;
using PsGen.Mobile.Models;

namespace PsGen.Mobile.Services;

public class AuthService
{
    private const string SessionKey = "current_session";
    private readonly ApiService _apiService;
    private UserSessionDto? _currentSession;

    public AuthService(ApiService apiService)
    {
        _apiService = apiService;
        LoadSession();
    }

    public bool IsLoggedIn
    {
        get
        {
            if (_currentSession == null) return false;
            return _currentSession.ExpiresAt > DateTimeOffset.UtcNow;
        }
    }

    public string? AuthToken => _currentSession?.Id;

    public string? UserId => _currentSession?.UserId;

    public async Task<RecordResponse<UserSessionDto>?> LoginAsync(string username, string password)
    {
        var request = new LoginRequest { Username = username, Password = password };
        var response = await _apiService.PostAsync<RecordResponse<UserSessionDto>>("login", request);

        if (response is { IsSuccess: true })
        {
            _currentSession = response.Result;
            SaveSession();
        }

        return response;
    }

    public void Logout()
    {
        _currentSession = null;
        SecureStorage.Remove(SessionKey);
    }

    private void SaveSession()
    {
        if (_currentSession == null) return;
        var json = JsonSerializer.Serialize(_currentSession);
        SecureStorage.SetAsync(SessionKey, json).ConfigureAwait(false);
    }

    private void LoadSession()
    {
        try
        {
            var json = SecureStorage.GetAsync(SessionKey).GetAwaiter().GetResult();
            if (!string.IsNullOrWhiteSpace(json))
                _currentSession = JsonSerializer.Deserialize<UserSessionDto>(json);
        }
        catch
        {
            _currentSession = null;
        }
    }
}

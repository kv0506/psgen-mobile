using PsGen.Mobile.Models;

namespace PsGen.Mobile.Services;

public class AccountService
{
    private readonly ApiService _apiService;
    private readonly AuthService _authService;

    public AccountService(ApiService apiService, AuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
    }

    public async Task<RecordsResponse<AccountDto>?> GetAllAsync()
    {
        if (!_authService.IsLoggedIn)
            return new RecordsResponse<AccountDto> { IsSuccess = false, Message = "Session expired. Please log in again." };

        return await _apiService.GetAsync<RecordsResponse<AccountDto>>("accounts", _authService.AuthToken);
    }

    public async Task<RecordResponse<AccountDto>?> GetAsync(string accountId)
    {
        return await _apiService.GetAsync<RecordResponse<AccountDto>>(
            $"accounts?accountId={accountId}", _authService.AuthToken);
    }

    public async Task<RecordResponse<AccountDto>?> CreateAsync(CreateAccountRequest request)
    {
        return await _apiService.PutAsync<RecordResponse<AccountDto>>("accounts", request, _authService.AuthToken);
    }

    public async Task<RecordResponse<AccountDto>?> UpdateAsync(UpdateAccountRequest request)
    {
        return await _apiService.PostAsync<RecordResponse<AccountDto>>("accounts", request, _authService.AuthToken);
    }

    public async Task<DeletedResponse?> DeleteAsync(string accountId)
    {
        var request = new DeleteAccountRequest { Id = accountId };
        return await _apiService.DeleteAsync<DeletedResponse>("accounts", request, _authService.AuthToken);
    }
}

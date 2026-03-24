using PsGen.Mobile.Models;
using PsGen.Mobile.Services;

namespace PsGen.Mobile.Views;

public partial class AccountsPage : ContentPage
{
    private readonly AccountService _accountService;
    private readonly AccountCacheService _cacheService;
    private readonly AuthService _authService;
    private List<AccountDto> _allAccounts = [];

    public AccountsPage(AccountService accountService, AccountCacheService cacheService, AuthService authService)
    {
        InitializeComponent();
        _accountService = accountService;
        _cacheService = cacheService;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Show cached data immediately so the UI is not blank.
        _allAccounts = await _cacheService.GetCachedAccountsAsync();
        ApplyFilter();
        UpdateFavorites();

        // Refresh from the API once per app session.
        if (!_cacheService.HasRefreshedThisSession)
            await RefreshFromApiAsync();
    }

    private async Task RefreshFromApiAsync()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        try
        {
            var response = await _accountService.GetAllAsync();
            if (response is { IsSuccess: true })
            {
                _allAccounts = response.Result ?? [];
                await _cacheService.SaveCacheAsync(_allAccounts);
                _cacheService.MarkRefreshed();
                ApplyFilter();
                UpdateFavorites();
            }
            else
            {
                await HandlePossibleAuthErrorAsync(response?.Message);
            }
        }
        catch
        {
            await DisplayAlertAsync("Error", "Connection error. Please try again.", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    /// <summary>
    /// Forces a remote refresh regardless of session state (used after mutations).
    /// </summary>
    private async Task ForceRefreshAsync()
    {
        _cacheService.MarkRefreshed();
        await RefreshFromApiAsync();
    }

    private void ApplyFilter()
    {
        var query = SearchBar.Text?.Trim() ?? string.Empty;
        var filtered = string.IsNullOrWhiteSpace(query)
            ? _allAccounts
            : _allAccounts.Where(a => a.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        AccountsCollection.ItemsSource = filtered;
    }

    private void UpdateFavorites()
    {
        var favorites = _allAccounts.Where(a => a.IsFavorite).ToList();
        FavoritesSection.IsVisible = favorites.Count > 0;
        FavoritesCollection.ItemsSource = favorites;
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e) => ApplyFilter();

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await ForceRefreshAsync();
        RefreshContainer.IsRefreshing = false;
    }

    private async void OnAccountTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string accountId)
        {
            var account = _allAccounts.FirstOrDefault(a => a.Id == accountId);
            if (account != null)
                await Shell.Current.GoToAsync(nameof(ManageAccountPage),
                    new Dictionary<string, object> { ["Account"] = account });
        }
    }

    private async void OnFavoriteAccountTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string accountId)
        {
            var account = _allAccounts.FirstOrDefault(a => a.Id == accountId);
            if (account != null)
                await Shell.Current.GoToAsync(nameof(GeneratePasswordPage),
                    new Dictionary<string, object> { ["Account"] = account });
        }
    }

    private async void OnGeneratePassword(object? sender, EventArgs e)
    {
        if (sender is ImageButton btn && btn.CommandParameter is string accountId)
        {
            var account = _allAccounts.FirstOrDefault(a => a.Id == accountId);
            if (account != null)
                await Shell.Current.GoToAsync(nameof(GeneratePasswordPage),
                    new Dictionary<string, object> { ["Account"] = account });
        }
    }

    private async void OnToggleFavorite(object? sender, EventArgs e)
    {
        if (sender is ImageButton btn && btn.CommandParameter is string accountId)
        {
            var account = _allAccounts.FirstOrDefault(a => a.Id == accountId);
            if (account != null)
            {
                account.IsFavorite = !account.IsFavorite;
                var request = new UpdateAccountRequest
                {
                    Id = account.Id,
                    Name = account.Name,
                    Category = account.Category,
                    Username = account.Username,
                    Pattern = account.Pattern,
                    Length = account.Length,
                    IncludeSpecialCharacter = account.IncludeSpecialCharacter,
                    UseCustomSpecialCharacter = account.UseCustomSpecialCharacter,
                    CustomSpecialCharacter = account.CustomSpecialCharacter,
                    Notes = account.Notes,
                    IsFavorite = account.IsFavorite
                };

                try
                {
                    await _accountService.UpdateAsync(request);
                    await ForceRefreshAsync();
                }
                catch
                {
                    await DisplayAlertAsync("Error", "Failed to update favorite", "OK");
                }
            }
        }
    }

    private async void OnDeleteAccount(object? sender, EventArgs e)
    {
        if (sender is SwipeItem item && item.CommandParameter is string accountId)
        {
            var confirm = await DisplayAlertAsync("Delete", "Are you sure you want to delete this account?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    var response = await _accountService.DeleteAsync(accountId);
                    if (response is { IsSuccess: true })
                        await ForceRefreshAsync();
                    else
                        await DisplayAlertAsync("Error", response?.Message ?? "Failed to delete account", "OK");
                }
                catch
                {
                    await DisplayAlertAsync("Error", "Connection error", "OK");
                }
            }
        }
    }

    private async Task HandlePossibleAuthErrorAsync(string? message)
    {
        if (!_authService.IsLoggedIn
            || (message is not null && message.Contains("session expired", StringComparison.OrdinalIgnoreCase)))
        {
            RedirectToLogin();
            return;
        }

        await DisplayAlertAsync("Error", message ?? "Failed to load accounts", "OK");
    }

    private static void RedirectToLogin()
    {
        var services = Application.Current!.Handler!.MauiContext!.Services;
        services.GetRequiredService<AuthService>().Logout();
        services.GetRequiredService<AccountCacheService>().ClearCache();

        Application.Current.Windows[0].Page = new NavigationPage(
            services.GetRequiredService<LoginPage>());
    }
}

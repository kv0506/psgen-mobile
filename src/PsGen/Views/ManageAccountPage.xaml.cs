using PsGen.Mobile.Models;
using PsGen.Mobile.Services;

namespace PsGen.Mobile.Views;

[QueryProperty(nameof(Account), "Account")]
public partial class ManageAccountPage : ContentPage
{
    private readonly AccountService _accountService;
    private AccountDto? _account;

    public AccountDto? Account
    {
        get => _account;
        set
        {
            _account = value;
            LoadAccountData();
        }
    }

    public ManageAccountPage(AccountService accountService)
    {
        InitializeComponent();
        _accountService = accountService;
    }

    private void LoadAccountData()
    {
        if (_account == null) return;

        AccountNameEntry.Text = _account.Name;
        CategoryEntry.Text = _account.Category;
        UsernameEntry.Text = _account.Username;
        PatternEntry.Text = _account.Pattern;
        LengthSlider.Value = _account.Length;
        LengthLabel.Text = _account.Length.ToString();
        IncludeSpecialSwitch.IsToggled = _account.IncludeSpecialCharacter;
        UseCustomSpecialSwitch.IsToggled = _account.UseCustomSpecialCharacter;
        CustomSpecialCharEntry.Text = _account.CustomSpecialCharacter;
        NotesEditor.Text = _account.Notes;
        FavoriteSwitch.IsToggled = _account.IsFavorite;

        UpdateSpecialCharVisibility();
        Title = "Edit Account";
    }

    private void OnLengthChanged(object? sender, ValueChangedEventArgs e)
    {
        var length = (int)Math.Round(e.NewValue);
        LengthSlider.Value = length;
        LengthLabel.Text = length.ToString();
    }

    private void OnIncludeSpecialToggled(object? sender, ToggledEventArgs e) => UpdateSpecialCharVisibility();

    private void OnUseCustomSpecialToggled(object? sender, ToggledEventArgs e) => UpdateSpecialCharVisibility();

    private void UpdateSpecialCharVisibility()
    {
        CustomSpecialSection.IsVisible = IncludeSpecialSwitch.IsToggled;
        CustomSpecialCharEntry.IsVisible = IncludeSpecialSwitch.IsToggled && UseCustomSpecialSwitch.IsToggled;

        if (!IncludeSpecialSwitch.IsToggled)
        {
            UseCustomSpecialSwitch.IsToggled = false;
            CustomSpecialCharEntry.Text = string.Empty;
        }

        if (!UseCustomSpecialSwitch.IsToggled)
            CustomSpecialCharEntry.Text = string.Empty;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        var name = AccountNameEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            ErrorLabel.Text = "Account name is required";
            ErrorLabel.IsVisible = true;
            return;
        }

        var pattern = PatternEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(pattern))
        {
            ErrorLabel.Text = "Pattern is required";
            ErrorLabel.IsVisible = true;
            return;
        }

        SaveButton.IsEnabled = false;
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        try
        {
            var length = (int)Math.Round(LengthSlider.Value);

            if (_account != null && !string.IsNullOrEmpty(_account.Id))
            {
                var request = new UpdateAccountRequest
                {
                    Id = _account.Id,
                    Name = name,
                    Category = CategoryEntry.Text?.Trim() ?? string.Empty,
                    Username = UsernameEntry.Text?.Trim() ?? string.Empty,
                    Pattern = pattern,
                    Length = length,
                    IncludeSpecialCharacter = IncludeSpecialSwitch.IsToggled,
                    UseCustomSpecialCharacter = UseCustomSpecialSwitch.IsToggled,
                    CustomSpecialCharacter = CustomSpecialCharEntry.Text?.Trim() ?? string.Empty,
                    Notes = NotesEditor.Text?.Trim() ?? string.Empty,
                    IsFavorite = FavoriteSwitch.IsToggled
                };

                var response = await _accountService.UpdateAsync(request);
                if (response is { IsSuccess: true })
                {
                    await DisplayAlertAsync("Success", $"Account '{name}' updated", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorLabel.Text = response?.Message ?? "Failed to update account";
                    ErrorLabel.IsVisible = true;
                }
            }
            else
            {
                var request = new CreateAccountRequest
                {
                    Name = name,
                    Category = CategoryEntry.Text?.Trim() ?? string.Empty,
                    Username = UsernameEntry.Text?.Trim() ?? string.Empty,
                    Pattern = pattern,
                    Length = length,
                    IncludeSpecialCharacter = IncludeSpecialSwitch.IsToggled,
                    UseCustomSpecialCharacter = UseCustomSpecialSwitch.IsToggled,
                    CustomSpecialCharacter = CustomSpecialCharEntry.Text?.Trim() ?? string.Empty,
                    Notes = NotesEditor.Text?.Trim() ?? string.Empty,
                    IsFavorite = FavoriteSwitch.IsToggled
                };

                var response = await _accountService.CreateAsync(request);
                if (response is { IsSuccess: true })
                {
                    await DisplayAlertAsync("Success", $"Account '{name}' created", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorLabel.Text = response?.Message ?? "Failed to create account";
                    ErrorLabel.IsVisible = true;
                }
            }
        }
        catch
        {
            ErrorLabel.Text = "Connection error. Please try again.";
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            SaveButton.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}

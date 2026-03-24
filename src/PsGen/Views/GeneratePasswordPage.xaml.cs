using PsGen.Mobile.Models;
using PsGen.Mobile.Services;

namespace PsGen.Mobile.Views;

[QueryProperty(nameof(Account), "Account")]
public partial class GeneratePasswordPage : ContentPage
{
    private readonly AuthService _authService;
    private AccountDto? _account;
    private string? _resultPassword;
    private bool _displayPassword;

    public AccountDto? Account
    {
        get => _account;
        set
        {
            _account = value;
            LoadAccountData();
        }
    }

    public GeneratePasswordPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private void LoadAccountData()
    {
        if (_account == null) return;

        AccountLabel.Text = _account.Name;
        AccountLabel.IsVisible = true;
        PatternEntry.Text = _account.Pattern;
        LengthSlider.Value = _account.Length;
        LengthLabel.Text = _account.Length.ToString();
        IncludeSpecialSwitch.IsToggled = _account.IncludeSpecialCharacter;
        UseCustomSpecialSwitch.IsToggled = _account.UseCustomSpecialCharacter;
        CustomSpecialCharEntry.Text = _account.CustomSpecialCharacter;

        UpdateSpecialCharVisibility();
        SaveAccountButton.IsVisible = _authService.IsLoggedIn;
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

    private void OnTogglePatternVisibility(object? sender, EventArgs e)
    {
        PatternEntry.IsPassword = !PatternEntry.IsPassword;
    }

    private void OnTogglePinVisibility(object? sender, EventArgs e)
    {
        PinEntry.IsPassword = !PinEntry.IsPassword;
    }

    private void OnTogglePasswordVisibility(object? sender, EventArgs e)
    {
        _displayPassword = !_displayPassword;
        UpdatePasswordDisplay();
    }

    private void OnGenerateClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        var pattern = PatternEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(pattern))
        {
            ErrorLabel.Text = "Pattern is required";
            ErrorLabel.IsVisible = true;
            return;
        }

        var pin = PinEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4)
        {
            ErrorLabel.Text = "PIN must be at least 4 digits";
            ErrorLabel.IsVisible = true;
            return;
        }

        var actualPattern = pattern + pin;
        var length = (int)Math.Round(LengthSlider.Value);
        var includeSpecial = IncludeSpecialSwitch.IsToggled;
        var useCustomSpecial = UseCustomSpecialSwitch.IsToggled;
        var customSpecial = CustomSpecialCharEntry.Text?.Trim() ?? string.Empty;

        _resultPassword = PasswordHashService.CreatePassword(
            actualPattern, length, includeSpecial, useCustomSpecial, customSpecial);

        _displayPassword = false;
        ResultFrame.IsVisible = true;
        UpdatePasswordDisplay();

        if (_authService.IsLoggedIn)
            SaveAccountButton.IsVisible = true;
    }

    private void UpdatePasswordDisplay()
    {
        if (_resultPassword == null) return;
        PasswordLabel.Text = _displayPassword ? _resultPassword : new string('*', _resultPassword.Length);
    }

    private async void OnCopyPassword(object? sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_resultPassword))
        {
            await Clipboard.SetTextAsync(_resultPassword);
            await DisplayAlertAsync("Copied", "Password copied to clipboard", "OK");
        }
    }

    private async void OnSaveAccountClicked(object? sender, EventArgs e)
    {
        if (_resultPassword == null) return;

        var account = _account ?? new AccountDto();
        account.Pattern = PatternEntry.Text?.Trim() ?? string.Empty;
        account.Length = (int)Math.Round(LengthSlider.Value);
        account.IncludeSpecialCharacter = IncludeSpecialSwitch.IsToggled;
        account.UseCustomSpecialCharacter = UseCustomSpecialSwitch.IsToggled;
        account.CustomSpecialCharacter = CustomSpecialCharEntry.Text?.Trim() ?? string.Empty;

        await Shell.Current.GoToAsync(nameof(ManageAccountPage),
            new Dictionary<string, object> { ["Account"] = account });
    }
}

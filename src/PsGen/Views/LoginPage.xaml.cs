using PsGen.Mobile.Services;

namespace PsGen.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        var username = UsernameEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Please enter username and password";
            ErrorLabel.IsVisible = true;
            return;
        }

        LoginButton.IsEnabled = false;
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        try
        {
            var response = await _authService.LoginAsync(username, password);
            if (response is { IsSuccess: true })
            {
                Application.Current!.Windows[0].Page = new AppShell();
            }
            else
            {
                ErrorLabel.Text = response?.Message ?? "Invalid username or password";
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Connection error. Please try again.";
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            LoginButton.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}

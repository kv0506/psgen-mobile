using PsGen.Mobile.Services;
using PsGen.Mobile.Views;

namespace PsGen.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(ManageAccountPage), typeof(ManageAccountPage));
        Routing.RegisterRoute(nameof(GeneratePasswordPage), typeof(GeneratePasswordPage));
    }

    private void OnLogoutClicked(object? sender, EventArgs e)
    {
        var services = Application.Current!.Handler!.MauiContext!.Services;
        services.GetRequiredService<AuthService>().Logout();
        services.GetRequiredService<AccountCacheService>().ClearCache();

        Application.Current.Windows[0].Page = new NavigationPage(
            services.GetRequiredService<LoginPage>());
    }
}

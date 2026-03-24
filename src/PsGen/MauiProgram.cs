using Microsoft.Extensions.Logging;
using PsGen.Mobile.Services;
using PsGen.Mobile.Views;

namespace PsGen.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<AccountService>();
        builder.Services.AddSingleton<AccountCacheService>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<AccountsPage>();
        builder.Services.AddTransient<ManageAccountPage>();
        builder.Services.AddTransient<GeneratePasswordPage>();

        return builder.Build();
    }
}

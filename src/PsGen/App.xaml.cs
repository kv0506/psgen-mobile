using PsGen.Mobile.Services;
using PsGen.Mobile.Views;

namespace PsGen.Mobile;

public partial class App : Application
{
	private readonly IServiceProvider _services;

	public App(IServiceProvider services)
	{
		InitializeComponent();
		_services = services;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var authService = _services.GetRequiredService<AuthService>();

		Page startPage = authService.IsLoggedIn
			? new AppShell()
			: new NavigationPage(_services.GetRequiredService<LoginPage>());

		return new Window(startPage);
	}
}
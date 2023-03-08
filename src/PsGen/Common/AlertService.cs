using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace PsGen.Common;

public class AlertService
{
	public static async Task ShortToast(string message)
	{
		var cancellationTokenSource = new CancellationTokenSource();
		var toast = Toast.Make(message, ToastDuration.Short, 15);
		await toast.Show(cancellationTokenSource.Token);

	}
}
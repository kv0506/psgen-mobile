using CSharpExtensions;
using PsGen.Common;

namespace PsGen;

public class MainViewModel : ViewModelBase
{
	private string _actualPassword;

	public MainViewModel()
	{
		ClearPassword();

		GeneratePasswordCommand = new Command(GeneratePassword);
		ClearPasswordCommand = new Command(ClearPassword);
		CopyPasswordCommand = new Command(CopyPassword);
	}

	private string _password;

	public string Password
	{
		get => _password;
		set => SetProperty(ref _password, value);
	}

	private string _pattern;

	public string Pattern
	{
		get => _pattern;
		set => SetProperty(ref _pattern, value);
	}

	private string _pin;

	public string Pin
	{
		get => _pin;
		set => SetProperty(ref _pin, value);
	}

	private int _length;

	public int Length
	{
		get => _length;
		set => SetProperty(ref _length, value);
	}

	private bool _includeSpecialCharacter;

	public bool IncludeSpecialCharacter
	{
		get => _includeSpecialCharacter;
		set => SetProperty(ref _includeSpecialCharacter, value);
	}

	private bool _useCustomSpecialCharacter;

	public bool UseCustomSpecialCharacter
	{
		get => _useCustomSpecialCharacter;
		set => SetProperty(ref _useCustomSpecialCharacter, value);
	}

	private string _customSpecialCharacter;

	public string CustomSpecialCharacter
	{
		get => _customSpecialCharacter;
		set => SetProperty(ref _customSpecialCharacter, value);
	}

	private bool _showPassword;

	public bool ShowPassword
	{
		get => _showPassword;
		set
		{
			if (SetProperty(ref _showPassword, value))
			{
				ShowHidePassword();
			}
		}
	}

	public Command GeneratePasswordCommand { get; set; }

	public Command CopyPasswordCommand { get; set; }

	public Command ClearPasswordCommand { get; set; }

	private async void GeneratePassword()
	{
		if (Pattern.IsNullOrWhiteSpace())
		{
			await AlertService.ShortToast("Please enter the password pattern");
			return;
		}

		if (Pin.IsNullOrWhiteSpace() || !int.TryParse(Pin, out var pin))
		{
			await AlertService.ShortToast("Please enter valid pin");
			return;
		}

		if (Length < 8 || Length > 44)
		{
			await AlertService.ShortToast("Please enter valid password length. Length must be between 8 to 44.");
			return;
		}

		if (UseCustomSpecialCharacter && CustomSpecialCharacter.IsNullOrWhiteSpace())
		{
			await AlertService.ShortToast("Please enter the custom special character");
			return;
		}

		_actualPassword = PasswordHash.CreatePassword($"{Pattern}{Pin}", Length, IncludeSpecialCharacter,
			UseCustomSpecialCharacter,
			CustomSpecialCharacter);

		ShowHidePassword();
	}

	private async void CopyPassword()
	{
		if (_actualPassword.IsNotNullOrWhiteSpace())
		{
			await Clipboard.SetTextAsync(_actualPassword);
			await AlertService.ShortToast("Password copied");
		}
	}

	private void ClearPassword()
	{
		Password = string.Empty;
		Pattern = string.Empty;
		Pin = string.Empty;
		Length = 20;
		CustomSpecialCharacter = string.Empty;
		IncludeSpecialCharacter = true;
		UseCustomSpecialCharacter = false;
		_actualPassword = string.Empty;
	}

	private void ShowHidePassword()
	{
		Password = _actualPassword.IsNotNullOrWhiteSpace()
			? (ShowPassword ? _actualPassword : new string('*', Length))
			: string.Empty;
	}
}
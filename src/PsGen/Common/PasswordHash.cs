using System.Security.Cryptography;
using System.Text;

namespace PsGen.Common;

public static class PasswordHash
{
	private static readonly string Characters = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	public static string CreatePassword(string pattern, int length, bool includeSpecialCharacter,
		bool useCustomSpecialCharacter, string customSpecialCharacter)
	{
		var randomPassword = CreateHash(pattern);
		var password = new StringBuilder();

		if (includeSpecialCharacter)
		{
			var hasSpecialCharacter = false;

			for (var i = 0; i < length; i++)
			{
				if (Characters.Contains(randomPassword[i]))
				{
					password.Append(randomPassword[i]);
				}
				else
				{
					hasSpecialCharacter = true;
					password.Append(useCustomSpecialCharacter ? customSpecialCharacter : randomPassword[i]);
				}
			}

			if (!hasSpecialCharacter)
			{
				password.Remove(length - 1, 1);
				password.Append(useCustomSpecialCharacter ? customSpecialCharacter : "#");
			}
		}
		else
		{
			for (var i = 0; i < length; i++)
			{
				password.Append(Characters.Contains(randomPassword[i]) ? randomPassword[i] : Characters[i]);
			}
		}

		return password.ToString();
	}

	private static string CreateHash(string pattern)
	{
		var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(pattern));
		return Convert.ToBase64String(hash);
	}
}
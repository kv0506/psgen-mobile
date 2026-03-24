using System.Security.Cryptography;
using System.Text;

namespace PsGen.Mobile.Services;

public static class PasswordHashService
{
    private const string Characters = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string CreatePassword(
        string pattern,
        int length,
        bool includeSpecialCharacter,
        bool useCustomSpecialCharacter,
        string customSpecialCharacter)
    {
        var randomPassword = CreateHash(pattern);
        var password = new StringBuilder();

        if (!includeSpecialCharacter)
        {
            for (var i = 0; i < length; i++)
            {
                if (i < randomPassword.Length && Characters.Contains(randomPassword[i]))
                    password.Append(randomPassword[i]);
                else
                    password.Append(Characters[i % Characters.Length]);
            }

            return password.ToString();
        }

        var hasSpecialCharacter = false;
        for (var i = 0; i < length; i++)
        {
            if (i < randomPassword.Length && Characters.Contains(randomPassword[i]))
            {
                password.Append(randomPassword[i]);
            }
            else
            {
                hasSpecialCharacter = true;
                if (useCustomSpecialCharacter && !string.IsNullOrEmpty(customSpecialCharacter))
                    password.Append(customSpecialCharacter[0]);
                else if (i < randomPassword.Length)
                    password.Append(randomPassword[i]);
                else
                    password.Append('#');
            }
        }

        if (!hasSpecialCharacter)
        {
            var passwordPart = password.ToString()[..(length - 1)];
            return useCustomSpecialCharacter && !string.IsNullOrEmpty(customSpecialCharacter)
                ? passwordPart + customSpecialCharacter[0]
                : passwordPart + '#';
        }

        return password.ToString();
    }

    private static string CreateHash(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}

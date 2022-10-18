using System.Security.Cryptography;
using System.Text;

namespace quizz.Utils;

public static class HashExtension
{
    public static string Sha256(this string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(inputBytes);

        return Encoding.UTF8.GetString(hashBytes);
    }
}
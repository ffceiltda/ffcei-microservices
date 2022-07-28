using System.Security.Cryptography;
using System.Text;

namespace FFCEI.Microservices.Generators;

/// <summary>
/// Random token generator
/// </summary>
public static class RandomTokenGenerator
{
    /// <summary>
    /// Generate a randon token
    /// </summary>
    /// <param name="length">Token length</param>
    /// <param name="useNumbers">Use numbers</param>
    /// <param name="useLetters">Use letters</param>
    /// <param name="letterCapitalization">Letter capitalization style</param>
    /// <param name="useSymbols">Use symbols</param>
    /// <returns></returns>
    public static string GenerateRandomToken(int length, bool useNumbers = true, bool useLetters = true, LetterCapitalization letterCapitalization = LetterCapitalization.UpperCase, bool useSymbols = false)
    {
        var chars = new List<char>();

        if (useNumbers)
        {
            chars.AddRange("0123456789".ToArray());
        }

        if (useLetters)
        {
            if ((letterCapitalization == LetterCapitalization.LowerCase) ||
                (letterCapitalization == LetterCapitalization.AnyCase))
            {
                chars.AddRange("abcdefghijklmnopqrstuwvxyz".ToArray());
            }

            if ((letterCapitalization == LetterCapitalization.UpperCase) ||
                (letterCapitalization == LetterCapitalization.AnyCase))
            {
                chars.AddRange("ABCDEFGHIJKLMNOPQRSTUWVXYZ".ToArray());
            }
        }

        if (useSymbols)
        {
            chars.AddRange("+-*/=_!@#$%&´`~^?\\|".ToArray());
        }

        var result = new StringBuilder();

        for (int current = 0; current < length; current++)
        {
            var index = RandomNumberGenerator.GetInt32(chars.Count);

#pragma warning disable IDE0058 // Expression value is never used
            result.Append(chars[index]);
#pragma warning restore IDE0058 // Expression value is never used
        }

        return result.ToString();
    }
}

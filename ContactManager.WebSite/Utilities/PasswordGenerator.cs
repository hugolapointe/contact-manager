using System.Text;

namespace ContactManager.WebSite.Utilities;

/// <summary>
/// Generates random passwords that meet security requirements.
/// </summary>
public class PasswordGenerator {
    private static Random RANDOM = new();

    private const string LOWERCASES = "abcdefghijklmnopqrstuvwxyz";
    private const string UPPERCASES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string DIGITS = "0123456789";
    private const string SPECIALS = "!@#$%&*";

    private const int LENGTH_MIN = 10;
    private const int LOWERCASE_MIN = 1;
    private const int UPPERCASE_MIN = 1;
    private const int DIGITS_MIN = 1;
    private const int SPECIAL_MIN = 1;

    /// <summary>
    /// Generates a random password with at least one lowercase letter, one uppercase letter, 
    /// one digit, and one special character.
    /// </summary>
    /// <returns>A randomly generated password of at least 10 characters.</returns>
    public static string Generate() {
        var password = new StringBuilder();

        for (int i = 0; i < LOWERCASE_MIN; i++) {
            password.Append(LOWERCASES[RANDOM.Next(LOWERCASES.Length)]);
        }

        for (int i = 0; i < UPPERCASE_MIN; i++) {
            password.Append(UPPERCASES[RANDOM.Next(UPPERCASES.Length)]);
        }

        for (int i = 0; i < DIGITS_MIN; i++) {
            password.Append(DIGITS[RANDOM.Next(DIGITS.Length)]);
        }

        for (int i = 0; i < SPECIAL_MIN; i++) {
            password.Append(SPECIALS[RANDOM.Next(SPECIALS.Length)]);
        }

        while (password.Length < LENGTH_MIN) {
            password.Append(LOWERCASES[RANDOM.Next(LOWERCASES.Length)]);
        }

        return new string(password.ToString().ToCharArray()
            .OrderBy(x => RANDOM.Next()).ToArray()
        );
    }
}

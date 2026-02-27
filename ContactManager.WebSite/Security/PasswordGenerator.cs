using System.Text;
using System.Security.Cryptography;

namespace ContactManager.WebSite.Security;

public class PasswordGenerator {
    private const string LOWERCASES = "abcdefghijklmnopqrstuvwxyz";
    private const string UPPERCASES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string DIGITS = "0123456789";
    private const string SPECIALS = "!@#$%&*";
    private const int LENGTH_MIN = 10;
    private const int LOWERCASE_MIN = 1;
    private const int UPPERCASE_MIN = 1;
    private const int DIGITS_MIN = 1;
    private const int SPECIAL_MIN = 1;
    public static string Generate() {
        var password = new StringBuilder();
        for (int i = 0; i < LOWERCASE_MIN; i++) {
            password.Append(LOWERCASES[RandomNumberGenerator.GetInt32(LOWERCASES.Length)]);
        }
        for (int i = 0; i < UPPERCASE_MIN; i++) {
            password.Append(UPPERCASES[RandomNumberGenerator.GetInt32(UPPERCASES.Length)]);
        }
        for (int i = 0; i < DIGITS_MIN; i++) {
            password.Append(DIGITS[RandomNumberGenerator.GetInt32(DIGITS.Length)]);
        }
        for (int i = 0; i < SPECIAL_MIN; i++) {
            password.Append(SPECIALS[RandomNumberGenerator.GetInt32(SPECIALS.Length)]);
        }
        while (password.Length < LENGTH_MIN) {
            password.Append(LOWERCASES[RandomNumberGenerator.GetInt32(LOWERCASES.Length)]);
        }
        return password.ToString();
    }
}

using System.Security.Cryptography;

namespace webapi.Generators
{
    public class PasswordGenerator
    {
        private readonly char[] _lowercaseChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private readonly char[] _uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private readonly char[] _numericChars = "0123456789".ToCharArray();
        private readonly char[] _specialChars = "!@#$%^&*()_+-=[]{}|;:',.<>?/~".ToCharArray();

        public string GeneratePassword(int length = 12, bool includeSpecialChars = true)
        {
            if (length < 8)
            {
                throw new ArgumentException("Password length should be at least 8 characters.");
            }

            var random = new RNGCryptoServiceProvider();
            var charSet = _lowercaseChars.Concat(_uppercaseChars).Concat(_numericChars).ToArray();

            if (includeSpecialChars)
            {
                charSet = charSet.Concat(_specialChars).ToArray();
            }

            var passwordChars = new char[length];
            var data = new byte[length];

            random.GetBytes(data);

            for (int i = 0; i < length; i++)
            {
                passwordChars[i] = charSet[data[i] % charSet.Length];
            }

            return new string(passwordChars);
        }
    }
}

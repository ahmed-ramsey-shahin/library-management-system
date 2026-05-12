using System.Security.Cryptography;
using Lms.Application.Common.Interfaces;

namespace Lms.Infrastructure.Identity
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string Specials = "@$!%*?&";
        private const string AllChars = Lowercase + Uppercase + Digits + Specials;

        private static char GetRandomChar(string chars)
        {
            int index = RandomNumberGenerator.GetInt32(chars.Length);
            return chars[index];
        }

        private static void Shuffle(List<char> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public string Generate(int length=8)
        {
            if (length < 8)
            {
                throw new ArgumentException("Password length must be at least 8.");
            }

            var password = new List<char>
            {
                GetRandomChar(Lowercase),
                GetRandomChar(Uppercase),
                GetRandomChar(Digits),
                GetRandomChar(Specials)
            };

            for (int i = password.Count; i < length; i++)
            {
                password.Add(GetRandomChar(AllChars));
            }

            Shuffle(password);
            return new string([.. password]);
        }
    }
}

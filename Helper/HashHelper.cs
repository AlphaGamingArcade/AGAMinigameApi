using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AGAMinigameApi.Helpers
{
    public class HashHelper
    {
        public static string Base64UrlEncode(byte[] bytes)
        {
            var s = Convert.ToBase64String(bytes);
            return s.Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        public static string ComputeSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
using System.Security.Cryptography;
using System.Text;

namespace FinalProject.Middleware
{
    public class MD5_password
    {
        //private static string storedHashedPassword = "dshyfwy7fegdcgeferyfgreruigherugjhurtngv";
        public static string GetSha256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            string inputHashedPassword = GetSha256Hash(inputPassword);
            return StringComparer.OrdinalIgnoreCase.Compare(inputHashedPassword, storedHashedPassword) == 0;
        }
    }
}

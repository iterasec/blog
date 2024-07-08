using System.Security.Cryptography;
using System.Text;

namespace RedirectTest.Services
{
    public class StringHelper
    {
        public string GenerateRandomImageName(string inputName)
        {
            string[] arr = inputName.Split('.');
            if (arr.Length != 2)
            {
                throw new ArgumentException();
            }

            if (arr[1] != "png" && arr[1] != "jpg")
            {
                throw new ArgumentException();
            }

            Random rand = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 32; i++)
            {
                sb.Append((char)rand.Next(97, 123));
            }
            return sb.ToString() + "." + arr[1];
        }

        public string GetPasswordHash(string Password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
            SHA1 sha1 = SHA1.Create();
            byte[] hashPasswordBytes = sha1.ComputeHash(passwordBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashPasswordBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", hashPasswordBytes[i]);
            }

            return sb.ToString();
        }

        public bool CheckIfItIsBaseUrl(string url)
        {
            if (url.Contains('#') || url.Contains('\\') || url.Contains('?') || url.Contains('/') || url.Contains('&') || url.Contains("http") || url.Contains("https"))
                return false;
            else
                return true;
        }
    }
}

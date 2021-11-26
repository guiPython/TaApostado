using System.Security.Cryptography;
using System.Text;

namespace TaApostado.Utils
{
    public class MD5Utils
    {
        public static string Generate(string input)
        {
            MD5 hash = MD5.Create();
            var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("X2"));
            }

            return builder.ToString();
        }
    }
}

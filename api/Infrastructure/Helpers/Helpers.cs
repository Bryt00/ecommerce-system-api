using System.Security.Cryptography;
using System.Text;


namespace ecommapi.Infrastructure.Helpers
{
   static public class Helpers
    {
        public static string HashToken(  string token)
        {
            using var encryption = SHA256.Create();
            var tokenHash = encryption.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(tokenHash);
             
        }
    } 
}
using System.Security.Cryptography;
using System.Text;

namespace CableManager.Common.Helpers
{
   public static class CryptoHelper
   {
      public static byte[] HashPassword(string password)
      {
         var provider = new SHA1CryptoServiceProvider();
         var encoding = new UnicodeEncoding();

         return provider.ComputeHash(encoding.GetBytes(password));
      }
   }
}
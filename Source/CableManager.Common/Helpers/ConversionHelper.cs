using System.Text;

namespace CableManager.Common.Helpers
{
   public static class ConversionHelper
   {
      public static string ToString(byte[] bytes)
      {
         var builder = new StringBuilder();

         foreach (var byteItem in bytes)
         {
            builder.Append(byteItem.ToString("d"));
         }

         return builder.ToString();
      }
   }
}
using System.IO;
using System.Reflection;

namespace CableManager.Common.Helpers
{
   public static class FileHelper
   {
      public static Stream GetResourceStream(string name)
      {
         return Assembly.GetCallingAssembly().GetManifestResourceStream(name);
      }
   }
}
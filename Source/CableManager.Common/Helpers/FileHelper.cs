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

      public static void SaveToDisk(MemoryStream memoryStream, string fileName)
      {
         FileStream fileStream;

         using (fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
         {
            memoryStream.CopyTo(fileStream);
         }
      }
   }
}
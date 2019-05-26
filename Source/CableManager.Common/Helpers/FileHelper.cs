using System.IO;

namespace CableManager.Common.Helpers
{
   public static class FileHelper
   {
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
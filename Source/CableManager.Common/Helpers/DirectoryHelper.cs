using System;
using System.IO;
using System.Reflection;

namespace CableManager.Common.Helpers
{
   public static class DirectoryHelper
   {
      public static void CreateDirectory(string directoryName)
      {
         if (!string.IsNullOrEmpty(directoryName))
         {
            var directoryInfo = new DirectoryInfo(directoryName);

            if (!Directory.Exists(directoryInfo.FullName))
            {

               Directory.CreateDirectory(directoryInfo.FullName);
            }
         }
      }

      public static string GetApplicationStoragePath()
      {
         string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Cable manager";

         return userProfilePath;
      }

      public static string GetApplicationPath()
      {
         string codeBase = Assembly.GetExecutingAssembly().CodeBase;
         UriBuilder uri = new UriBuilder(codeBase);
         string path = Uri.UnescapeDataString(uri.Path);

         return Path.GetDirectoryName(path);
      }
   }
}
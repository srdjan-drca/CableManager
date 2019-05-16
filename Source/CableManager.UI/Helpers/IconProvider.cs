namespace CableManager.UI.Helpers
{
   public static class IconProvider
   {
      public static string GetImagePath(string imageName)
      {
         string resourcePath = "../Resources/Images/" + imageName;

         return resourcePath;
      }
   }
}
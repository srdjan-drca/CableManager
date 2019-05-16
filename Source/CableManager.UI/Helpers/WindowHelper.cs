using System.Linq;
using System.Windows;

namespace CableManager.UI.Helpers
{
   public static class WindowHelper
   {
      public static Window GetWindowRef(string windowName)
      {
         Window retVal = null;
         foreach (Window window in Application.Current.Windows)
         {
            // The window's Name is set in XAML. See comment below
            if (window.Name.Trim().ToLower() == windowName.Trim().ToLower())
            {
               retVal = window;
               break;
            }
         }

         return retVal;
      }

      public static bool IsWindowOpen<T>(string name = "") where T : Window
      {
         return string.IsNullOrEmpty(name)
            ? Application.Current.Windows.OfType<T>().Any()
            : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
      }
   }
}
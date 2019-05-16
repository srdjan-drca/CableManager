using System;
using System.Globalization;
using System.Windows.Data;

namespace CableManager.UI.Converters
{
   public class ToUpperValueConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var inputString = value as string;

         return string.IsNullOrEmpty(inputString) ? string.Empty : inputString.ToUpper();
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}

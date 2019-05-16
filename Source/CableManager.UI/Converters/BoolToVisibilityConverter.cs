using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CableManager.UI.Converters
{
   internal class BoolToVisibilityConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is bool == false)
            return DependencyProperty.UnsetValue;

         bool bValue = (bool)value;

         bool invert = !string.IsNullOrEmpty(parameter as string) && ((string)parameter).ToLowerInvariant().Contains("invert");
         if (invert)
            return bValue ? Visibility.Collapsed : Visibility.Visible;

         return bValue ? Visibility.Visible : Visibility.Collapsed;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is Visibility == false)
            return DependencyProperty.UnsetValue;

         Visibility vValue = (Visibility)value;

         bool invert = !string.IsNullOrEmpty(parameter as string) && ((string)parameter).ToLowerInvariant().Contains("invert");

         return invert ? vValue != Visibility.Visible : vValue == Visibility.Visible;
      }
   }
}
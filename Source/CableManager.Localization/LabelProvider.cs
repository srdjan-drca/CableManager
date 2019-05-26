using System.Resources;
using System.Globalization;
using System.ComponentModel;
using CableManager.Localization.Resources;

namespace CableManager.Localization
{
   public class LabelProvider : ILabelProvider, INotifyPropertyChanged
   {
      private readonly ResourceManager _resourceManager = Labels.ResourceManager;

      private CultureInfo _currentCulture = null;

      public string this[string key]
      {
         get
         {
            string label = _resourceManager.GetString(key, _currentCulture);

            return string.IsNullOrEmpty(label)
               ? key
               : label;
         }
      }

      public void SetCulture(string cultureCode)
      {
         CurrentCulture = new CultureInfo(cultureCode);
      }

      public string GetCulture()
      {
         return CurrentCulture.Name;
      }

      #region Private methods

      private CultureInfo CurrentCulture
      {
         get { return _currentCulture; }
         set
         {
            if (_currentCulture != value)
            {
               _currentCulture = value;
               PropertyChangedEventHandler @event = PropertyChanged;

               @event?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
         }
      }

      #endregion

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
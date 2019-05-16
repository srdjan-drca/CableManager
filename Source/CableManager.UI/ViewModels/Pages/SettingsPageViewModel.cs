using System;
using System.Collections.Generic;
using CableManager.Localization;

namespace CableManager.UI.ViewModels.Pages
{
   public class SettingsPageViewModel : RootViewModel
   {
      private string _selectedLanguage;

      public SettingsPageViewModel(LabelProvider labelProvider) : base(labelProvider)
      {
         SelectedLanguage = string.IsNullOrEmpty(Properties.Settings.Default.SelectedLanguage)
            ? CultureCode.Croatian
            : Properties.Settings.Default.SelectedLanguage;
      }

      public Dictionary<string, string> LanguageDropDown
      {
         get
         {
            var languageDropDown = CreateLanguageDropDown();

            return languageDropDown;
         }
      }

      public string SelectedLanguage
      {
         get
         {
            return _selectedLanguage;
         }
         set
         {
            try
            {
               LabelProvider.SetCulture(value);

               _selectedLanguage = value;
            }
            catch (Exception exception)
            {
               Console.WriteLine(exception.Message);
               _selectedLanguage = CultureCode.Croatian;
            }

            Properties.Settings.Default.SelectedLanguage = _selectedLanguage;
            Properties.Settings.Default.Save();
         }
      }

      private Dictionary<string, string> CreateLanguageDropDown()
      {
         var languageDropDown = new Dictionary<string, string>
         {
            { CultureCode.English, LabelProvider["UI_English"] },
            { CultureCode.Croatian, LabelProvider["UI_Croatian"] }
         };

         return languageDropDown;
      }
   }
}
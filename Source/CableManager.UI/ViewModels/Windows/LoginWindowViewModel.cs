using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using CableManager.Services.User;
using CableManager.UI.Helpers;
using CableManager.UI.Navigation;
using CableManager.UI.Views.Windows;
using CableManager.UI.Notification;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Services.License;

namespace CableManager.UI.ViewModels.Windows
{
   public class LoginWindowViewModel : RootViewModel
   {
      private readonly IFrameNavigationService _frameNavigationService;

      private readonly IUserService _userService;

      private bool _shouldClearPasswordBox;

      private string _userName;

      private bool _isSubmitAllowed;

      private bool _isEnterLicenseVisible;

      public LoginWindowViewModel(LabelProvider labelProvider, IFrameNavigationService frameNavigationService,
         IUserService userService, ILicenseService licenseService)
         : base(labelProvider)
      {
         _frameNavigationService = frameNavigationService;
         _userService = userService;

         SubmitCredentialsCommand = new RelayCommand<object>(SubmitCredentials);
         ResetCredentialsCommand = new RelayCommand<object>(ResetCredentials);
         ShowLicenseWindowCommand = new RelayCommand<object>(ShowLicenseWindow);
         SelectLanguageCommand = new RelayCommand<object>(SelectLanguage);

         ReturnResult result = licenseService.CheckLicense();

         IsSubmitAllowed = result.IsSuccess;
         IsEnterLicenseVisible = !result.IsSuccess;

         MessengerInstance.Register<Message>(this, NotificationHandler);

         TranslateApp();
      }

      public bool ShouldClearPasswordBox
      {
         get { return _shouldClearPasswordBox; }
         set
         {
            _shouldClearPasswordBox = value;
            RaisePropertyChanged(nameof(ShouldClearPasswordBox));
         }
      }

      public string UserName
      {
         get { return _userName; }
         set
         {
            _userName = value;
            RaisePropertyChanged(nameof(UserName));
         }
      }

      public bool IsSubmitAllowed
      {
         get
         {
            return _isSubmitAllowed;
         }
         set
         {
            _isSubmitAllowed = value;
            RaisePropertyChanged(nameof(IsSubmitAllowed));
         }
      }

      public bool IsEnterLicenseVisible
      {
         get
         {
            return _isEnterLicenseVisible;
         }
         set
         {
            _isEnterLicenseVisible = value;
            RaisePropertyChanged(nameof(IsEnterLicenseVisible));
         }
      }

      public RelayCommand<object> SubmitCredentialsCommand { get; }

      public RelayCommand<object> ResetCredentialsCommand { get; }

      public RelayCommand<object> ShowLicenseWindowCommand { get; }

      public RelayCommand<object> SelectLanguageCommand { get; }

      #region Private methods

      private void SubmitCredentials(object parameter)
      {
         ReturnResult result;
         var uploadFileParameters = (object[]) parameter;
         var isSignUp = uploadFileParameters[0] as bool?;
         var userName = uploadFileParameters[1] as string;
         var passwordBox = uploadFileParameters[2] as PasswordBox;
         var passwordBoxConfirm = uploadFileParameters[3] as PasswordBox;

         if (isSignUp != null && isSignUp.Value)
         {
            result = _userService.SignUp(userName, passwordBox?.Password, passwordBoxConfirm?.Password);
         }
         else
         {
            result = _userService.LogIn(userName, passwordBox?.Password);

            if (result.IsSuccess)
            {
               var mainWindow = new MainWindow();
               mainWindow.Show();

               Window loginWindow = WindowHelper.GetWindowRef("LoginWindowId");
               loginWindow.Close();

               Application.Current.MainWindow = mainWindow;

               _frameNavigationService.NavigateTo(PageName.OfferCreatePage);
            }
         }

         StatusMessage = result.Message;
      }

      private void ResetCredentials(object parameter)
      {
         var passwordBox = parameter as PasswordBox;

         StatusMessage = string.Empty;
         UserName = string.Empty;
         passwordBox?.Clear();
      }

      private void ShowLicenseWindow(object obj)
      {
         if (!WindowHelper.IsWindowOpen<LicenseWindow>())
         {
            var licenseWindow = new LicenseWindow();

            licenseWindow.Show();
         }
      }

      private void SelectLanguage(object parameter)
      {
         var selectedCulture = parameter as string;

         LabelProvider.SetCulture(selectedCulture);
      }

      private void TranslateApp()
      {
         var selectedCulture = string.IsNullOrEmpty(Properties.Settings.Default.SelectedLanguage)
            ? CultureCode.Croatian
            : Properties.Settings.Default.SelectedLanguage;

         LabelProvider.SetCulture(selectedCulture);
      }

      private void NotificationHandler(Message message)
      {
         if (message.Type == MessageType.LicenseActivation)
         {
            bool isLicenseValid;
            bool.TryParse(message.RecordId, out isLicenseValid);

            if (isLicenseValid)
            {
               IsSubmitAllowed = true;
               IsEnterLicenseVisible = false;
            }
         }
      }

      #endregion Private methods
   }
}
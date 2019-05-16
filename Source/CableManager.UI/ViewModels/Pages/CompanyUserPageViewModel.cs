using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.User;
using CableManager.Services.Result;
using CableManager.Services.User;

namespace CableManager.UI.ViewModels.Pages
{
   public class CompanyUserPageViewModel : RootViewModel
   {
      private readonly IUserRepository _userRepository;

      private readonly IUserService _userService;

      public CompanyUserPageViewModel(LabelProvider labelProvider, IUserRepository userRepository, IUserService userService) : base(labelProvider)
      {
         _userRepository = userRepository;
         _userService = userService;

         SaveCompanyUserCommand = new RelayCommand<object>(SaveCompanyUser);
      }

      public string CurrentUserNumber
      {
         get
         {
            CurrentUser = _userService.GetCurrentlyLoggedInUser();

            return CurrentUser.Number;
         }
      }

      public string CurrentUserName
      {
         get
         {
            CurrentUser = _userService.GetCurrentlyLoggedInUser();

            return CurrentUser.Name;
         }
      }

      public RelayCommand<object> SaveCompanyUserCommand { get; set; }

      #region Private methods

      private void SaveCompanyUser(object parameter)
      {
         var userParameters = (object[])parameter;
         var isChangingPassword = userParameters[0] as bool?;
         ReturnResult result = new SuccessResult();

         if (isChangingPassword.HasValue && isChangingPassword.Value)
         {
            var currentPasswordBox = userParameters[1] as PasswordBox;
            var newPasswordBox = userParameters[2] as PasswordBox;
            var newPasswordBoxRepeated = userParameters[3] as PasswordBox;

            result = _userService.ChangePassword(CurrentUser.Name,
               currentPasswordBox?.Password, newPasswordBox?.Password, newPasswordBoxRepeated?.Password);

            if (result.IsSuccess)
            {
               var userResult = result as UserResult;

               if (userResult != null)
               {
                  CurrentUser.Password = userResult.Password;
               }
            }
         }

         if (result.IsSuccess)
         {
            ReturnResult saveResult = _userRepository.Save(CurrentUser);

            if (isChangingPassword.HasValue && !isChangingPassword.Value)
            {
               result = saveResult;
            }
         }

         StatusMessage = result.Message;
      }

      #endregion
   }
}

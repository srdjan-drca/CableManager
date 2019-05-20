using System;
using System.Linq;
using CableManager.Common.Helpers;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Models;
using CableManager.Repository.User;
using CableManager.Services.Result;

namespace CableManager.Services.User
{
   public class UserService : IUserService
   {
      private readonly LabelProvider _labelProvider;

      private readonly IUserRepository _userRepository;

      private UserModel _currentLoggedInUser;

      public UserService(LabelProvider labelProvider, IUserRepository userRepository)
      {
         _labelProvider = labelProvider;
         _userRepository = userRepository;
      }

      public ReturnResult SignUp(string userName, string password, string passwordRepeated)
      {
         if (string.IsNullOrEmpty(userName))
         {
            return new FailResult(_labelProvider["UI_UserNameIsEmpty"]);
         }

         if (string.IsNullOrEmpty(password))
         {
            return new FailResult(_labelProvider["UI_PasswordIsEmpty"]);
         }

         if (password != passwordRepeated)
         {
            return new FailResult(_labelProvider["UI_PasswordsMismatch"]);
         }

         if (_userRepository == null)
         {
            return new FailResult(_labelProvider["UI_UserDatabaseIsNotCreated"]);
         }

         if (IsExists(userName))
         {
            return new FailResult(_labelProvider["UI_UserNameAlreadySigned"]);
         }

         try
         {
            ReturnResult result = _userRepository.Save(new UserModel(userName, password));

            if (result.IsSuccess)
            {
               result.Message = _labelProvider["UI_UserSuccessfullySignedUp"];
            }

            return result;
         }
         catch (Exception exception)
         {
            return new FailResult(exception.Message);
         }
      }

      public ReturnResult LogIn(string userName, string password)
      {
         if (string.IsNullOrEmpty(userName))
         {
            return new FailResult(_labelProvider["UI_UserNameIsEmpty"]);
         }

         if (string.IsNullOrEmpty(password))
         {
            return new FailResult(_labelProvider["UI_PasswordIsEmpty"]);
         }

         if (_userRepository == null)
         {
            return new FailResult(_labelProvider["UI_UserDatabaseIsNotCreated"]);
         }

         ReturnResult result;

         try
         {
            UserModel user = _userRepository.GetByNameAndPassword(userName, password);

            if (user == null)
            {
               result = new FailResult(_labelProvider["UI_UserOrPasswordNotCorrect"]);
            }
            else
            {
               _currentLoggedInUser = user;
               result = new SuccessResult();
            }
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      public ReturnResult LogOut()
      {
         _currentLoggedInUser = null;

         return new SuccessResult();
      }

      public ReturnResult ChangePassword(string userName, string currentPassword, string newPassword, string newPasswordRepeated)
      {
         if (string.IsNullOrEmpty(currentPassword))
         {
            return new FailResult(_labelProvider["UI_CurrentPasswordIsEmpty"]);
         }

         UserModel user = _userRepository.GetByNameAndPassword(userName, currentPassword);

         if (user == null)
         {
            return new FailResult(_labelProvider["UI_UserOrPasswordNotCorrect"]);
         }

         if (string.IsNullOrEmpty(newPassword))
         {
            return new FailResult(_labelProvider["UI_NewPasswordIsEmpty"]);
         }

         if (newPassword != newPasswordRepeated)
         {
            return new FailResult(_labelProvider["UI_PasswordsMismatch"]);
         }

         string password = ConversionHelper.ToString(CryptoHelper.HashPassword(newPassword));

         var result = new UserResult(true, _labelProvider["UI_PasswordSuccessfullyChanged"])
         {
            Password = password
         };

         return result;
      }

      public UserModel GetCurrentlyLoggedInUser()
      {
         UserModel user = _userRepository.Get(_currentLoggedInUser.Id);

         _currentLoggedInUser = user;

         return _currentLoggedInUser;
      }

      public void UpdateLastOfferNumber(string userId, string lastOfferNumber)
      {
         _userRepository.UpdateLastOfferNumber(userId, lastOfferNumber);
      }

      private bool IsExists(string userName)
      {
         bool isExists = _userRepository.GetAll().Any(x => x.Name == userName);

         return isExists;
      }
   }
}
using CableManager.Common.Result;
using CableManager.Repository.Models;

namespace CableManager.Services.User
{
   public interface IUserService
   {
      ReturnResult SignUp(string userName, string password, string passwordRepeated);

      ReturnResult LogIn(string userName, string password);

      ReturnResult LogOut();

      ReturnResult ChangePassword(string userName, string currentPassword, string newPassword, string newPasswordRepeated);

      UserModel GetCurrentlyLoggedInUser();

      void UpdateLastOfferNumber(string userId, string lastOfferNumber);
   }
}
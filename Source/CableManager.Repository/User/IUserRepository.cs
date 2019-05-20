using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Repository.Models;

namespace CableManager.Repository.User
{
   public interface IUserRepository
   {
      ReturnResult Save(UserModel userModel);

      UserModel Get(string userId);

      UserModel GetByNameAndPassword(string userName, string password);

      List<UserModel> GetAll();

      void UpdateLastOfferNumber(string userId, string lastOfferNumber);
   }
}
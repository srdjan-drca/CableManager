using CableManager.Common.Result;

namespace CableManager.Services.Result
{
   public class UserResult : ReturnResult
   {
      public string Password { get; set; }

      public UserResult(bool isSuccess, string message) : base(isSuccess, message)
      {
      }
   }
}

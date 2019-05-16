namespace CableManager.Common.Result
{
   public abstract class ReturnResult
   {
      public bool IsSuccess { get; set; }

      public string Message { get; set; }

      protected ReturnResult(bool isSuccess, string message)
      {
         IsSuccess = isSuccess;
         Message = message;
      }
   }
}
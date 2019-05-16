namespace CableManager.Common.Result
{
   public class SuccessResult : ReturnResult
   {
      public SuccessResult() : base(true, string.Empty)
      {
      }

      public SuccessResult(string message) : base(true, message)
      {
      }
   }
}
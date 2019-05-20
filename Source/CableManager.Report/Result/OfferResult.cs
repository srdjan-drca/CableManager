using CableManager.Common.Result;

namespace CableManager.Report.Result
{
   public class OfferResult : ReturnResult
   {
      public string Date { get; set; }

      public string FileName { get; set; }

      public string FileFullName { get; set; }

      public OfferResult(bool isSuccess, string message) : base(isSuccess, message)
      {
      }
   }
}

using CableManager.Localization;

namespace CableManager.Report.Models
{
   public class BaseReportModel
   {
      public string TimeDate { get; set; }

      public ILabelProvider LabelProvider { get; set; }
   }
}
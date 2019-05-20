using CableManager.Report.Models;
using CableManager.Report.Generators.Excel.Worksheets;

namespace CableManager.Report.Generators.Excel.Workbooks
{
   public class CableOfferWorkbook : BaseWorkbook
   {
      public CableOfferWorkbook(BaseReportModel baseReportModel) : base(baseReportModel)
      {
      }

      protected override void CreateWorksheets()
      {
         Worksheets.Add(new CableWorksheet(BaseReportModel));
      }
   }
}
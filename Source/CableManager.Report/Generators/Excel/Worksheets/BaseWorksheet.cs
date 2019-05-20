using CableManager.Localization;
using CableManager.Report.Models;
using OfficeOpenXml;

namespace CableManager.Report.Generators.Excel.Worksheets
{
   public abstract class BaseWorksheet
   {
      protected ILabelProvider LabelProvider;

      protected BaseWorksheet(BaseReportModel baseReportModel)
      {
         LabelProvider = baseReportModel.LabelProvider;
      }

      public abstract string Name { get; set; }

      public abstract void AddContent(ExcelWorksheet worksheet);
   }
}
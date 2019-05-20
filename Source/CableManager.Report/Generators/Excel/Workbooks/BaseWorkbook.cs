using System.Collections.Generic;
using CableManager.Report.Models;
using CableManager.Report.Generators.Excel.Worksheets;

namespace CableManager.Report.Generators.Excel.Workbooks
{
   public abstract class BaseWorkbook
   {
      private readonly List<BaseWorksheet> _worksheets = new List<BaseWorksheet>();

      protected BaseReportModel BaseReportModel { get; set; }

      public List<BaseWorksheet> Worksheets => _worksheets;

      protected BaseWorkbook(BaseReportModel baseReportModel)
      {
         BaseReportModel = baseReportModel;

         CreateWorksheets();
      }

      protected abstract void CreateWorksheets();
   }
}
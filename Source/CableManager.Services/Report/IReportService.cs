using System.Collections.Generic;
using CableManager.Common.Result;

namespace CableManager.Services.Report
{
   public interface IReportService
   {
      ReturnResult GeneratePdfReport(string fileName, string language);

      ReturnResult GenerateExcelReport(string fileName, string language);

      ReturnResult DeleteReport(List<string> fullFileNames);
   }
}
using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Report.Result;

namespace CableManager.Services.Report
{
   public interface IReportService
   {
      OfferResult GeneratePdfReport(string filePrefix, string customerId, string note, string language);

      OfferResult GenerateExcelReport(string filePrefix, string customerId, string note, string language);

      ReturnResult DeleteReport(List<string> fullFileNames);
   }
}
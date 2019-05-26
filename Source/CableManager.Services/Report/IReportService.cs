using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Services.Calculation.Models;

namespace CableManager.Services.Report
{
   public interface IReportService
   {
      ReturnResult GeneratePdfReport(Offer offer);

      ReturnResult GenerateExcelReport(Offer offer);

      ReturnResult DeleteReport(List<string> fullFileNames);
   }
}
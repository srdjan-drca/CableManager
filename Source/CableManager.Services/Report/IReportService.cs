using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Services.Offer.Models;

namespace CableManager.Services.Report
{
   public interface IReportService
   {
      ReturnResult GeneratePdfReport(OfferModel offer);

      ReturnResult GenerateExcelReport(OfferModel offer);

      ReturnResult DeleteReport(List<string> fullFileNames);
   }
}
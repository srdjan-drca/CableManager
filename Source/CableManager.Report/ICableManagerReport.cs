using CableManager.Common.Result;
using CableManager.Report.Models;
using Spire.Pdf;

namespace CableManager.Report
{
   public interface ICableManagerReport
   {
      PdfDocumentBase GenerateOfferPdf(BaseReportModel baseReportModel);

      ReturnResult GenerateOfferExcel(string fileName, BaseReportModel baseReportModel);
   }
}
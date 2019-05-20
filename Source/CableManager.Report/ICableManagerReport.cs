using System.IO;
using Spire.Pdf;
using CableManager.Report.Models;

namespace CableManager.Report
{
   public interface ICableManagerReport
   {
      PdfDocumentBase GenerateOfferPdf(BaseReportModel baseReportModel);

      MemoryStream GenerateOfferExcel(BaseReportModel baseReportModel);
   }
}
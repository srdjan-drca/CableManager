using System;
using System.IO;
using CableManager.Report.Common;
using CableManager.Report.StyleManager;
using OfficeOpenXml;
using Spire.Pdf;
using CableManager.Common.Result;
using CableManager.Report.Generators.Pdf.Documents;
using CableManager.Report.Models;

namespace CableManager.Report
{
   public class CableManagerReport : ICableManagerReport
   {
      public CableManagerReport()
      {
         ReportUtils.RegisterLicence();
         ReportStyleManager.Instance.Initialize();
      }

      public PdfDocumentBase GenerateOfferPdf(BaseReportModel baseReportModel)
      {
         var cableOfferPdf = new CableOfferPdf(baseReportModel);
         PdfDocumentBase pdfDocument = cableOfferPdf.Generate();

         return pdfDocument;
      }

      public ReturnResult GenerateOfferExcel(string fileName, BaseReportModel baseReportModel)
      {
         ReturnResult result;

         try
         {
            var newFile = new FileInfo(fileName);
            using (var excelPackage = new ExcelPackage(newFile))
            {
               ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Test");

               excelPackage.Save();
            }

            result = new SuccessResult();
         }
         catch (Exception exception)
         {
            Console.WriteLine(exception);
            throw;
         }

         return result;
      }
   }
}
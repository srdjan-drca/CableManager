using System.IO;
using System.Linq;
using System.Collections.Generic;
using Spire.Pdf;
using OfficeOpenXml;
using CableManager.Report.Models;
using CableManager.Report.Helpers;
using CableManager.Report.StyleManager;
using CableManager.Report.Generators.Excel.Workbooks;
using CableManager.Report.Generators.Excel.Worksheets;
using CableManager.Report.Generators.Pdf.Documents;
using CableManager.Report.Generators.Pdf.Sections;

namespace CableManager.Report
{
   public class CableManagerReport : ICableManagerReport
   {
      public CableManagerReport()
      {
         ReportUtils.RegisterLicense();
         ReportStyleManager.Instance.Initialize();
      }

      public PdfDocumentBase GenerateOfferPdf(BaseReportModel baseReportModel)
      {
         var cableOfferPdf = new CableOfferPdfDocument(baseReportModel);
         PdfDocumentBase pdfDocument = GeneratePdfFile(cableOfferPdf.Sections);

         return pdfDocument;
      }

      public MemoryStream GenerateOfferExcel(BaseReportModel baseReportModel)
      {
         var cableOfferWorkbook = new CableOfferWorkbook(baseReportModel);
         MemoryStream excelFile = GenerateExcelFile(cableOfferWorkbook.Worksheets);

         return excelFile;
      }

      #region Private methods

      private PdfDocumentBase GeneratePdfFile(List<BasePdfSection> sections)
      {
         MemoryStream[] reportSections = sections.Select(section => section.GenerateContent()).ToArray();
         PdfDocumentBase mergedDocument = PdfDocument.MergeFiles(reportSections);

         return mergedDocument;
      }

      private MemoryStream GenerateExcelFile(List<BaseWorksheet> worksheets)
      {
         var excelFile = new MemoryStream();

         using (var package = new ExcelPackage(excelFile))
         {
            foreach (var worksheet in worksheets)
            {
               var excelWorksheet = package.Workbook.Worksheets.Add(worksheet.Name);

               worksheet.AddContent(excelWorksheet);
            }

            package.Save();
         }

         excelFile.Position = 0;

         return excelFile;
      }

      #endregion
   }
}
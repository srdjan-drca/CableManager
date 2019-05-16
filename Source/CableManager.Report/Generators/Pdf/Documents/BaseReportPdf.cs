using System.Collections.Generic;
using System.IO;
using System.Linq;
using CableManager.Report.Generators.Pdf.Sections;
using CableManager.Report.Models;
using Spire.Pdf;

namespace CableManager.Report.Generators.Pdf.Documents
{
   public abstract class BaseReportPdf
   {
      protected BaseReportModel BaseReportModel { get; set; }

      protected List<BaseSection> Sections { get; }

      protected BaseReportPdf(BaseReportModel baseReportModel)
      {
         Sections = new List<BaseSection>();
         BaseReportModel = baseReportModel;

         CreateSections();
      }

      public PdfDocumentBase Generate()
      {
         MemoryStream[] reportSections = Sections.Select(section => section.GenerateContent()).ToArray();
         PdfDocumentBase mergedDocument = PdfDocument.MergeFiles(reportSections);

         return mergedDocument;
      }

      protected abstract void CreateSections();
   }
}
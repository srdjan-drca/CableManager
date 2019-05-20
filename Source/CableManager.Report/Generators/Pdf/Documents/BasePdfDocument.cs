using System.Collections.Generic;
using CableManager.Report.Models;
using CableManager.Report.Generators.Pdf.Sections;

namespace CableManager.Report.Generators.Pdf.Documents
{
   public abstract class BasePdfDocument
   {
      protected BaseReportModel BaseReportModel { get; set; }

      public List<BasePdfSection> Sections { get; }

      protected BasePdfDocument(BaseReportModel baseReportModel)
      {
         Sections = new List<BasePdfSection>();
         BaseReportModel = baseReportModel;

         CreateSections();
      }

      protected abstract void CreateSections();
   }
}
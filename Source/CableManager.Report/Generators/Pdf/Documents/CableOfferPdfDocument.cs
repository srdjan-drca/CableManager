using CableManager.Report.Models;
using CableManager.Report.Generators.Pdf.Sections;

namespace CableManager.Report.Generators.Pdf.Documents
{
   public class CableOfferPdfDocument : BasePdfDocument
   {
      public CableOfferPdfDocument(BaseReportModel baseReportModel) : base(baseReportModel)
      {
      }

      protected override void CreateSections()
      {
         Sections.Add(new CablePdfSection(BaseReportModel));
      }
   }
}
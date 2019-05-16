using CableManager.Report.Generators.Pdf.Sections;
using CableManager.Report.Models;

namespace CableManager.Report.Generators.Pdf.Documents
{
   public class CableOfferPdf : BaseReportPdf
   {
      public CableOfferPdf(BaseReportModel baseReportModel) : base(baseReportModel)
      {
      }

      protected override void CreateSections()
      {
         Sections.Add(new CableSection(BaseReportModel));
      }
   }
}
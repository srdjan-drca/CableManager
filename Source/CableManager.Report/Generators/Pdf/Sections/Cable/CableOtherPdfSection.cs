using System.IO;
using CableManager.Report.Extensions;
using CableManager.Report.Models;
using Spire.Pdf;
using Spire.Pdf.Grid;

namespace CableManager.Report.Generators.Pdf.Sections.Cable
{
   public class CableOtherPdfSection : BaseCablePdfSection
   {
      public CableOtherPdfSection(BaseReportModel baseReportModel) : base(baseReportModel)
      {
      }

      public override MemoryStream GenerateContent()
      {
         var pdfDocument = new PdfDocument();
         int mainTableHeight;

         pdfDocument.PageSettings.Margins.Top = 20;
         pdfDocument.PageSettings.Margins.Left = 7;
         pdfDocument.PageSettings.Margins.Right = 7;
         pdfDocument.PageSettings.Margins.Bottom = 20;

         PdfPageBase page = pdfDocument.Pages.Add(PdfPageSize.A4);
         PdfGrid companyDetailsGrid = CreateCompanyDetailsContent();
         PdfGrid offerDetailsGrid = CreateOfferDetailsGrid();
         PdfGrid cableOfferGrid = CreateCableOfferGrid(out mainTableHeight);
         PdfGrid cableOfferTotalsGrid = CreateCableOfferTotalsGrid();

         page.Add(companyDetailsGrid, 0, 0);
         page.Add(offerDetailsGrid, 0, 70);
         page.Add(cableOfferGrid, 0, 100);
         page.Add(cableOfferTotalsGrid, 0, 105 + mainTableHeight);
         AddBottomLine(page, 155 + mainTableHeight);
         page.Add(ReportFooter);

         return pdfDocument.ToMemoryStream();
      }

      private PdfGrid CreateOfferDetailsGrid()
      {
         PdfGrid grid = CreateGrid(80, 100);
         PdfGridRow row1 = grid.AddRow();

         row1.AddCell(Arial12BlackLeftBold, 0, LabelProvider["DOC_OfferNumber"]);
         row1.AddCell(Arial12BlackLeftBold, 1, OfferReportModel.Id);

         return grid;
      }
   }
}

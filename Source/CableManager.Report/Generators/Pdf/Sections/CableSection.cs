using System.Drawing;
using System.IO;
using CableManager.Report.Common;
using CableManager.Report.Models;
using CableManager.Report.StyleManager;
using CableManager.Report.StyleManager.Cell;
using Spire.Pdf;
using Spire.Pdf.Grid;

namespace CableManager.Report.Generators.Pdf.Sections
{
   public class CableSection : BaseSection
   {
      private CellStyle _arial12BlackLeft;
      private CellStyle _arial10BlackLeft;
      private CellStyle _arial9BlackLeft;
      private CellStyle _arial8BlackLeft;

      private CellStyle _arial12BlackLeftBold;
      private CellStyle _arial10BlackLeftBold;
      private CellStyle _arial9BlackLeftBold;

      private CellStyle _arial8BlackLeftGrayTopBorder;
      private CellStyle _arial8BlackLeftWhiteTopBorder;
      private CellStyle _arial8BlackLeftWhiteBottomBorder;
      private CellStyle _arial8BlackLeftWhiteLeftBorder;
      private CellStyle _arial8BlackLeftWhiteRightBorder;

      private readonly OfferReportModel _offerReportModel;

      public CableSection(BaseReportModel baseReportModel) : base(baseReportModel)
      {
         CreateCellStyles();

         _offerReportModel = (OfferReportModel)baseReportModel;
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
         PdfGrid customerDetailsGrid = CreateCustomerDetailsContent();
         PdfGrid offerDetailsGrid = CreateOfferDetailsGrid();
         PdfGrid cableOfferGrid = CreateCableOfferGrid(out mainTableHeight);
         PdfGrid cableOfferTotalsGrid = CreateCableOfferTotalsGrid();

         page.Add(companyDetailsGrid, 0, 0);
         page.Add(customerDetailsGrid, 35, 130);
         page.Add(offerDetailsGrid, 300, 130);
         AddUserDetails(page);
         page.Add(cableOfferGrid, 0, 360);
         page.Add(cableOfferTotalsGrid, 0, 360 + mainTableHeight);
         AddBottomLine(page, 410 + mainTableHeight);

         page.Add(ReportFooter);

         return pdfDocument.ToMemoryStream();
      }

      private void AddBottomLine(PdfPageBase page, float yPosition)
      {
         page.AddText("Kontrolirao:    ********", _arial8BlackLeft, 0, yPosition);
         page.AddText("Odobrio:    ********", _arial8BlackLeft, 230, yPosition);

         page.AddText("Ponuda služi samo kao poziv na plaćanje", _arial9BlackLeft, 0, yPosition + 20);
         page.AddText("Iskazani porez na dodanu vrijednost (PDV 25%) ne može se priznati kao predporez temeljem ove ponude.", _arial9BlackLeft, 0, yPosition + 32);
         page.AddText("IZDAO:", _arial9BlackLeft, 0, yPosition + 44);
      }

      private PdfGrid CreateCableOfferTotalsGrid()
      {
         PdfGrid grid = CreateGrid(90, 100, 100, 80, 90, 50);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();

         row1.RemoveAllBorders();
         row2.RemoveAllBorders();
         row3.RemoveAllBorders();

         row1.AddCell(_arial9BlackLeft, 1, "Ukupno rabat:");
         row1.AddCell(_arial9BlackLeft, 2, "5.284,14");
         row1.AddCell(_arial9BlackLeft.Clone().SetRightAlignment(), 3, "Ukupno bez PDV:");
         row1.AddCell(_arial9BlackLeft.Clone().SetRightAlignment(), 5, "15.852,36");

         row2.AddCell(_arial9BlackLeft.Clone().SetRightAlignment(), 3, "Ukupno PDV:");
         row2.AddCell(_arial9BlackLeft.Clone().SetRightAlignment(), 5, "3.963,09");

         row3.AddCell(_arial9BlackLeftBold.Clone().SetRightAlignment(), 3, "Ukupno:");
         row3.AddCell(_arial9BlackLeftBold.Clone().SetRightAlignment(), 5, "19.815,45");

         return grid;
      }

      private PdfGrid CreateCompanyDetailsContent()
      {
         PdfGrid grid = CreateGrid(250);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();
         PdfGridRow row4 = grid.AddRow();
         string taxNumber = "OIB: " + _offerReportModel.CompanyDetails?.TaxNumber;

         row1.AddCell(_arial12BlackLeftBold, 0, _offerReportModel.CompanyDetails?.Name);
         row2.AddCell(_arial10BlackLeftBold, 0, _offerReportModel.CompanyDetails?.Street);
         row3.AddCell(_arial10BlackLeftBold, 0, _offerReportModel.CompanyDetails?.City);
         row4.AddCell(_arial10BlackLeftBold, 0, taxNumber);

         return grid;
      }

      private PdfGrid CreateCustomerDetailsContent()
      {
         PdfGrid grid = CreateGrid(150);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();
         PdfGridRow row4 = grid.AddRow();

         row1.AddCell(_arial10BlackLeftBold, 0, _offerReportModel.CustomerDetails.Name);
         row2.AddCell(_arial10BlackLeft, 0, _offerReportModel.CustomerDetails.Street);
         row3.AddCell(_arial10BlackLeft, 0, _offerReportModel.CustomerDetails.PostCodeAndCity);
         row4.AddCell(_arial10BlackLeft, 0, _offerReportModel.CustomerDetails.Country);

         return grid;
      }

      private PdfGrid CreateOfferDetailsGrid()
      {
         PdfGrid grid = CreateGrid(80, 100);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();
         PdfGridRow row4 = grid.AddRow();
         PdfGridRow row5 = grid.AddRow();

         row1.AddCell(_arial12BlackLeftBold, 0, "PONUDA br:");
         row1.AddCell(_arial12BlackLeftBold, 1, _offerReportModel.Id);

         row2.AddCell(_arial9BlackLeft, 0, "Datum:");
         row2.AddCell(_arial9BlackLeft, 1, _offerReportModel.TimeDate);

         row3.AddCell(_arial9BlackLeft, 0, "OIB kupca:");
         row3.AddCell(_arial9BlackLeft, 1, _offerReportModel.CustomerDetails.TaxNumber);

         row4.AddCell(_arial9BlackLeft, 0, "Vrijedi do:");
         row4.AddCell(_arial9BlackLeft, 1, string.Empty);

         row5.AddCell(_arial9BlackLeft, 0, "Datum isporuke:");
         row5.AddCell(_arial9BlackLeft, 1, string.Empty);

         return grid;
      }

      private void AddUserDetails(PdfPageBase page)
      {
         string userDetails = "Dokument sastavio: " + _offerReportModel.UserNumberAndName;
         string customer = "Kupac:0258, email: " + _offerReportModel.CustomerDetails.Email;
         string contactInfo = CreateContactInfo();

         page.AddText(userDetails, _arial9BlackLeft, 0, 240);
         page.AddText(customer, _arial9BlackLeft, 0, 250);
         page.AddText(_offerReportModel.Note, _arial9BlackLeft, 0, 260);
         page.AddText(contactInfo, _arial9BlackLeft, 0, 290);
         page.AddText(_offerReportModel.CompanyDetails?.BankAccount1, _arial9BlackLeft, 0, 310);
         page.AddText(_offerReportModel.CompanyDetails?.BankAccount2, _arial9BlackLeft, 0, 320);
      }

      private string CreateContactInfo()
      {
         string phoneInfo = "tel: " + _offerReportModel.CompanyDetails?.Phone1 + ", " + _offerReportModel.CompanyDetails?.Phone2;
         string faxInfo = "fax: " + _offerReportModel.CompanyDetails?.Fax;
         string mobileInfo = "mob: " + _offerReportModel.CompanyDetails?.MobilePhone;
         string emailInfo = "email: " + _offerReportModel.CompanyDetails?.Email;
         string contactInfo = string.Join(", ", phoneInfo, faxInfo, mobileInfo, emailInfo);

         return contactInfo;
      }

      private PdfGrid CreateCableOfferGrid(out int offset)
      {
         PdfGrid grid = CreateGrid(20, 70, 140, 30, 30, 40, 40, 40, 60, 60);

         AddHeaderRow(grid);
         AddEmptyRow(grid, true);

         offset = 60;

         foreach (CableDetails cableDetails in _offerReportModel.Cables)
         {
            PdfGridRow contentRow = grid.AddRow();

            contentRow.AddCell(_arial8BlackLeftWhiteLeftBorder, 0, cableDetails.SerialNumber);
            contentRow.AddCell(_arial8BlackLeft, 1, cableDetails.ItemCode);
            contentRow.AddCell(_arial8BlackLeft, 2, cableDetails.Name);
            contentRow.AddCell(_arial8BlackLeft, 3, cableDetails.Quantity);
            contentRow.AddCell(_arial8BlackLeft, 4, cableDetails.Unit);
            contentRow.AddCell(_arial8BlackLeft, 5, cableDetails.PricePerUnit);
            contentRow.AddCell(_arial8BlackLeft, 6, cableDetails.Rebate);
            contentRow.AddCell(_arial8BlackLeft, 7, cableDetails.Vat);
            contentRow.AddCell(_arial8BlackLeft, 8, cableDetails.TotalPrice);
            contentRow.AddCell(_arial8BlackLeftWhiteRightBorder, 9, cableDetails.TotalPriceWithVat);

            offset += 10;
         }

         AddEmptyRow(grid, false);

         return grid;
      }

      private void AddHeaderRow(PdfGrid grid)
      {
         PdfGridRow headerRow = grid.AddRow();

         headerRow.AddCell(_arial8BlackLeftGrayTopBorder.Clone().SetLeftBorder(), 0, "Rb");
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 1, "Šifra Dat.ispor.");
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 2, "Naziv");
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 3, "Količina");
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 4, "Jmj");
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 5, "Cijena");
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 6, "Rabat\n %");
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 7, "PDV\n %");
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 8, "Ukupno bez PDV:");
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder.Clone().SetRightBorder(), 9, "Iznos");
      }

      private void AddEmptyRow(PdfGrid grid, bool isTop)
      {
         PdfGridRow emptyRow = grid.AddRow(15);
         CellStyle cellStyle1 = isTop
            ? _arial8BlackLeftWhiteLeftBorder.Clone().SetTopBorder()
            : _arial8BlackLeftWhiteLeftBorder.Clone().SetBottomBorder();
         CellStyle cellStyle2 = isTop
            ? _arial8BlackLeftWhiteTopBorder
            : _arial8BlackLeftWhiteBottomBorder;
         CellStyle cellStyle3 = isTop
            ? _arial8BlackLeftWhiteRightBorder.Clone().SetTopBorder()
            : _arial8BlackLeftWhiteRightBorder.Clone().SetBottomBorder();

         emptyRow.AddCell(cellStyle1, 0, string.Empty);

         for (int j = 1; j < 9; j++)
         {
            emptyRow.AddCell(cellStyle2, j, string.Empty);
         }

         emptyRow.AddCell(cellStyle3, 9, string.Empty);
      }

      private void CreateCellStyles()
      {
         _arial12BlackLeft = ReportStyleManager.Instance.Get(CellStyleId.Arial12BlackLeft);
         _arial10BlackLeft = ReportStyleManager.Instance.Get(CellStyleId.Arial10BlackLeft);
         _arial9BlackLeft = ReportStyleManager.Instance.Get(CellStyleId.Arial9BlackLeft);
         _arial8BlackLeft = ReportStyleManager.Instance.Get(CellStyleId.Arial8BlackLeft);
         var arial8BlackLeftGray = ReportStyleManager.Instance.Get(CellStyleId.Arial8BlackLeftGray);

         _arial12BlackLeftBold = _arial12BlackLeft.Clone().SetBold();
         _arial10BlackLeftBold = _arial10BlackLeft.Clone().SetBold();
         _arial9BlackLeftBold = _arial9BlackLeft.Clone().SetBold();

         _arial8BlackLeftGrayTopBorder = arial8BlackLeftGray.Clone().RemoveAllBorders().SetTopBorder().SetBorderBrush(Color.Black, 0.1f);
         _arial8BlackLeftWhiteTopBorder = _arial8BlackLeft.Clone().RemoveAllBorders().SetTopBorder().SetBorderBrush(Color.Black, 0.1f);
         _arial8BlackLeftWhiteBottomBorder = _arial8BlackLeft.Clone().RemoveAllBorders().SetBottomBorder().SetBorderBrush(Color.Black, 0.1f);
         _arial8BlackLeftWhiteLeftBorder = _arial8BlackLeft.Clone().RemoveAllBorders().SetLeftBorder().SetBorderBrush(Color.Black, 0.1f);
         _arial8BlackLeftWhiteRightBorder = _arial8BlackLeft.Clone().RemoveAllBorders().SetRightBorder().SetBorderBrush(Color.Black, 0.1f);
      }
   }
}
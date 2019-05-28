using System.IO;
using System.Drawing;
using System.Globalization;
using Spire.Pdf;
using Spire.Pdf.Grid;
using Spire.Pdf.Graphics;
using CableManager.Report.Extensions;
using CableManager.Report.Helpers;
using CableManager.Report.Models;
using CableManager.Report.StyleManager;
using CableManager.Report.StyleManager.Cell;

namespace CableManager.Report.Generators.Pdf.Sections
{
   public class CablePdfSection : BasePdfSection
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

      private readonly CultureInfo _cultureInfo;

      public CablePdfSection(BaseReportModel baseReportModel) : base(baseReportModel)
      {
         CreateCellStyles();

         _offerReportModel = (OfferReportModel)baseReportModel;

         _cultureInfo = new CultureInfo(_offerReportModel.LabelProvider.GetCulture());
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
         PdfImage logo = CreateLogoImage();
         PdfGrid offerDetailsGrid = CreateOfferDetailsGrid();
         PdfGrid cableOfferGrid = CreateCableOfferGrid(out mainTableHeight);
         PdfGrid cableOfferTotalsGrid = CreateCableOfferTotalsGrid();

         page.Add(companyDetailsGrid, 0, 0);
         page.Add(customerDetailsGrid, 35, 130);
         page.Add(logo, 400, 20, 120, 50);
         page.Add(offerDetailsGrid, 300, 130);
         AddUserDetails(page);
         page.Add(cableOfferGrid, 0, 360);
         page.Add(cableOfferTotalsGrid, 0, 360 + mainTableHeight);
         AddBottomLine(page, 410 + mainTableHeight);

         page.Add(ReportFooter);

         return pdfDocument.ToMemoryStream();
      }

      private PdfGrid CreateCompanyDetailsContent()
      {
         PdfGrid grid = CreateGrid(250);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();
         PdfGridRow row4 = grid.AddRow();
         string taxNumber = string.Join(" ", LabelProvider["DOC_TaxNumber"] + _offerReportModel.CompanyModelPdf?.TaxNumber);

         row1.AddCell(_arial12BlackLeftBold, 0, _offerReportModel.CompanyModelPdf?.Name);
         row2.AddCell(_arial10BlackLeftBold, 0, _offerReportModel.CompanyModelPdf?.Street);
         row3.AddCell(_arial10BlackLeftBold, 0, _offerReportModel.CompanyModelPdf?.City);
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

         row1.AddCell(_arial10BlackLeftBold, 0, _offerReportModel.CustomerModelPdf.Name);
         row2.AddCell(_arial10BlackLeft, 0, _offerReportModel.CustomerModelPdf.Street);
         row3.AddCell(_arial10BlackLeft, 0, _offerReportModel.CustomerModelPdf.PostCodeAndCity);
         row4.AddCell(_arial10BlackLeft, 0, _offerReportModel.CustomerModelPdf.Country);

         return grid;
      }

      private PdfImage CreateLogoImage()
      {
         PdfImage logo = null;

         if (_offerReportModel.CompanyModelPdf.LogoPath != null)
         {
            string logoPath = new FileInfo(_offerReportModel.CompanyModelPdf.LogoPath).FullName;
            Image image = Image.FromFile(logoPath);

            logo = PdfImage.FromImage(image);
         }

         return logo;
      }

      private PdfGrid CreateOfferDetailsGrid()
      {
         PdfGrid grid = CreateGrid(80, 100);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();
         PdfGridRow row4 = grid.AddRow();
         PdfGridRow row5 = grid.AddRow();

         row1.AddCell(_arial12BlackLeftBold, 0, LabelProvider["DOC_OfferNumber"]);
         row1.AddCell(_arial12BlackLeftBold, 1, _offerReportModel.Id);

         row2.AddCell(_arial9BlackLeft, 0, LabelProvider["DOC_Date"]);
         row2.AddCell(_arial9BlackLeft, 1, _offerReportModel.TimeDate);

         row3.AddCell(_arial9BlackLeft, 0, LabelProvider["DOC_CustomerTaxNumber"]);
         row3.AddCell(_arial9BlackLeft, 1, _offerReportModel.CustomerModelPdf.TaxNumber);

         row4.AddCell(_arial9BlackLeft, 0, LabelProvider["DOC_WorthUntil"]);
         row4.AddCell(_arial9BlackLeft, 1, string.Empty);

         row5.AddCell(_arial9BlackLeft, 0, LabelProvider["DOC_DeliveryDate"]);
         row5.AddCell(_arial9BlackLeft, 1, string.Empty);

         return grid;
      }

      private void AddUserDetails(PdfPageBase page)
      {
         string userDetails = string.Join(" ",LabelProvider["DOC_DocumentComposedBy"], _offerReportModel.UserNumberAndName);
         string userEmail = string.Join(" ", LabelProvider["DOC_Email"], _offerReportModel.CustomerModelPdf.Email);
         string contactInfo = CreateContactInfo();

         page.AddText(userDetails, _arial9BlackLeft, 0, 240);
         page.AddText(userEmail, _arial9BlackLeft, 0, 250);
         page.AddText(_offerReportModel.Note, _arial9BlackLeft, 0, 260);
         page.AddText(contactInfo, _arial9BlackLeft, 0, 290);
         page.AddText(_offerReportModel.CompanyModelPdf?.BankAccounts[0] , _arial9BlackLeft, 0, 310);
         page.AddText(_offerReportModel.CompanyModelPdf?.BankAccounts[1], _arial9BlackLeft, 0, 320);
      }

      private string CreateContactInfo()
      {
         string phones = string.Join(", ", _offerReportModel.CompanyModelPdf?.Phone1, _offerReportModel.CompanyModelPdf?.Phone2);
         string phoneInfo = string.Join(" ", LabelProvider["DOC_Phone"], phones);
         string faxInfo = string.Join(" ", LabelProvider["DOC_Fax"], _offerReportModel.CompanyModelPdf?.Fax);
         string mobileInfo = string.Join(" ", LabelProvider["DOC_Mobile"], _offerReportModel.CompanyModelPdf?.MobilePhone);
         string emailInfo = string.Join(" ", LabelProvider["DOC_Email"], _offerReportModel.CompanyModelPdf?.Email);
         string contactInfo = string.Join(", ", phoneInfo, faxInfo, mobileInfo, emailInfo);

         return contactInfo;
      }

      private PdfGrid CreateCableOfferGrid(out int offset)
      {
         PdfGrid grid = CreateGrid(10, 20, 160, 40, 40, 40, 40, 40, 60, 60);

         AddHeaderRow(grid);
         AddEmptyRow(grid, true);

         offset = 60;

         foreach (OfferItem offerItem in _offerReportModel.OfferItems)
         {
            PdfGridRow contentRow = grid.AddRow();

            contentRow.AddCell(_arial8BlackLeftWhiteLeftBorder, 0, string.Empty);
            contentRow.AddCell(_arial8BlackLeft, 1, offerItem.SerialNumber);
            contentRow.AddCell(_arial8BlackLeft, 2, offerItem.Name);
            contentRow.AddCell(_arial8BlackLeft, 3, offerItem.Quantity);
            contentRow.AddCell(_arial8BlackLeft, 4, offerItem.Unit);
            contentRow.AddCell(_arial8BlackLeft, 5, string.Format(_cultureInfo, "{0:#,#.00}", offerItem.PricePerItem));
            contentRow.AddCell(_arial8BlackLeft, 6, string.Format(_cultureInfo, "{0:#,#.00}", offerItem.Rebate));
            contentRow.AddCell(_arial8BlackLeft, 7, string.Format(_cultureInfo, "{0:#,#.00}", offerItem.ValueAddedTax));
            contentRow.AddCell(_arial8BlackLeft, 8, string.Format(_cultureInfo, "{0:#,#.00}", offerItem.TotalPriceWithRebate));
            contentRow.AddCell(_arial8BlackLeftWhiteRightBorder, 9, string.Format(_cultureInfo, "{0:#,#.00}", offerItem.TotalPriceWithVat));

            offset += 10;
         }

         AddEmptyRow(grid, false);

         return grid;
      }

      private void AddHeaderRow(PdfGrid grid)
      {
         PdfGridRow headerRow = grid.AddRow();

         headerRow.AddCell(_arial8BlackLeftGrayTopBorder.Clone().SetLeftBorder(), 0, string.Empty);
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 1, LabelProvider["DOC_OrdinalNumber"]);
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 2, LabelProvider["DOC_Name"]);
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 3, LabelProvider["DOC_Quantity"]);
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 4, LabelProvider["DOC_Unit"]);
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 5, LabelProvider["DOC_Price"]);
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 6, LabelProvider["DOC_Rebate"]);
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 7, LabelProvider["DOC_VAT"]);
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder, 8, LabelProvider["DOC_TotalWithoutVAT"].Replace(":", ""));
         headerRow.AddCell(_arial8BlackLeftGrayTopBorder.Clone().SetRightBorder(), 9, LabelProvider["DOC_Amount"]);
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

      private PdfGrid CreateCableOfferTotalsGrid()
      {
         PdfGrid grid = CreateGrid(90, 100, 100, 80, 70, 70);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();

         row1.RemoveAllBorders();
         row2.RemoveAllBorders();
         row3.RemoveAllBorders();

         string totalRebate = string.Format(_cultureInfo, "{0:#,#.00}", _offerReportModel.OfferTotal.TotalRebate);
         string totalPrice = string.Format(_cultureInfo, "{0:#,#.00}", _offerReportModel.OfferTotal.TotalPrice);
         string totalValueAddedTax = string.Format(_cultureInfo, "{0:#,#.00}", _offerReportModel.OfferTotal.TotalValueAddedTax);
         string grandTotal = string.Format(_cultureInfo, "{0:#,#.00}", _offerReportModel.OfferTotal.GrandTotal);

         row1.AddCell(_arial9BlackLeft, 1, LabelProvider["DOC_RebateTotal"]);
         row1.AddCell(_arial9BlackLeft, 2, totalRebate);

         row1.AddCell(_arial9BlackLeft.Clone().SetRightAlignment(), 3, LabelProvider["DOC_TotalWithoutVAT"]);
         row1.AddCell(_arial9BlackLeft.Clone().SetRightAlignment(), 5, totalPrice);

         row2.AddCell(_arial9BlackLeft.Clone().SetRightAlignment(), 3, LabelProvider["DOC_TotalVAT"]);
         row2.AddCell(_arial9BlackLeft.Clone().SetRightAlignment(), 5, totalValueAddedTax);

         row3.AddCell(_arial9BlackLeftBold.Clone().SetRightAlignment(), 3, LabelProvider["DOC_Total"]);
         row3.AddCell(_arial9BlackLeftBold.Clone().SetRightAlignment(), 5, grandTotal);

         return grid;
      }

      private void AddBottomLine(PdfPageBase page, float yPosition)
      {
         string controlledBy = LabelProvider["DOC_ControlledBy"] + "    ********";
         string approvedBy = LabelProvider["DOC_ApprovedBy"] + "    ********";

         page.AddText(controlledBy, _arial8BlackLeft, 0, yPosition);
         page.AddText(approvedBy, _arial8BlackLeft, 230, yPosition);

         page.AddText(LabelProvider["DOC_Bottom1"], _arial9BlackLeft, 0, yPosition + 20);
         page.AddText(LabelProvider["DOC_Bottom2"], _arial9BlackLeft, 0, yPosition + 32);
         page.AddText(LabelProvider["DOC_IssuedBy"], _arial9BlackLeft, 0, yPosition + 44);
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
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using CableManager.Report.Extensions;
using CableManager.Report.Helpers;
using CableManager.Report.Models;
using CableManager.Report.StyleManager;
using CableManager.Report.StyleManager.Cell;
using Spire.Pdf;
using Spire.Pdf.Grid;

namespace CableManager.Report.Generators.Pdf.Sections.Cable
{
   public class BaseCablePdfSection : BasePdfSection
   {
      protected readonly OfferReportModel OfferReportModel;

      protected readonly CultureInfo CultureInfo;

      protected CellStyle Arial12BlackLeft;
      protected CellStyle Arial10BlackLeft;
      protected CellStyle Arial9BlackLeft;
      protected CellStyle Arial8BlackLeft;

      protected CellStyle Arial12BlackLeftBold;
      protected CellStyle Arial10BlackLeftBold;
      protected CellStyle Arial9BlackLeftBold;

      protected CellStyle Arial8BlackLeftGrayTopBorder;
      protected CellStyle Arial8BlackLeftWhiteTopBorder;
      protected CellStyle Arial8BlackLeftWhiteBottomBorder;
      protected CellStyle Arial8BlackLeftWhiteLeftBorder;
      protected CellStyle Arial8BlackLeftWhiteRightBorder;

      public BaseCablePdfSection(BaseReportModel baseReportModel) : base(baseReportModel)
      {
         CreateCellStyles();

         OfferReportModel = (OfferReportModel)baseReportModel;
         CultureInfo = new CultureInfo(OfferReportModel.LabelProvider.GetCulture());
      }

      public override MemoryStream GenerateContent()
      {
         throw new NotImplementedException();
      }

      protected PdfGrid CreateCompanyDetailsContent()
      {
         PdfGrid grid = CreateGrid(250);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();
         PdfGridRow row4 = grid.AddRow();
         string taxNumber = string.Join(" ", LabelProvider["DOC_TaxNumber"] + OfferReportModel.CompanyModelPdf?.TaxNumber);

         row1.AddCell(Arial12BlackLeftBold, 0, OfferReportModel.CompanyModelPdf?.Name);
         row2.AddCell(Arial10BlackLeftBold, 0, OfferReportModel.CompanyModelPdf?.Street);
         row3.AddCell(Arial10BlackLeftBold, 0, OfferReportModel.CompanyModelPdf?.City);
         row4.AddCell(Arial10BlackLeftBold, 0, taxNumber);

         return grid;
      }

      protected PdfGrid CreateCableOfferGrid(out int offset)
      {
         PdfGrid grid = CreateGrid(10, 20, 160, 40, 40, 40, 40, 40, 60, 60);

         AddHeaderRow(grid);
         AddEmptyRow(grid, true);

         offset = 60;

         foreach (OfferItem offerItem in OfferReportModel.OfferItems)
         {
            PdfGridRow contentRow = grid.AddRow();

            contentRow.AddCell(Arial8BlackLeftWhiteLeftBorder, 0, string.Empty);
            contentRow.AddCell(Arial8BlackLeft, 1, offerItem.SerialNumber);
            contentRow.AddCell(Arial8BlackLeft, 2, offerItem.Name);
            contentRow.AddCell(Arial8BlackLeft, 3, offerItem.Quantity);
            contentRow.AddCell(Arial8BlackLeft, 4, offerItem.Unit);
            contentRow.AddCell(Arial8BlackLeft, 5, string.Format(CultureInfo, "{0:#,0.00}", offerItem.PricePerItem));
            contentRow.AddCell(Arial8BlackLeft, 6, string.Format(CultureInfo, "{0:#,0.00}", offerItem.Rebate));
            contentRow.AddCell(Arial8BlackLeft, 7, string.Format(CultureInfo, "{0:#,0.00}", offerItem.ValueAddedTax));
            contentRow.AddCell(Arial8BlackLeft, 8, string.Format(CultureInfo, "{0:#,0.00}", offerItem.TotalPriceWithRebate));
            contentRow.AddCell(Arial8BlackLeftWhiteRightBorder, 9, string.Format(CultureInfo, "{0:#,0.00}", offerItem.TotalPriceWithVat));

            offset += 10;
         }

         AddEmptyRow(grid, false);

         return grid;
      }

      protected PdfGrid CreateCableOfferTotalsGrid()
      {
         PdfGrid grid = CreateGrid(90, 100, 100, 80, 70, 70);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();

         row1.RemoveAllBorders();
         row2.RemoveAllBorders();
         row3.RemoveAllBorders();

         string totalRebate = string.Format(CultureInfo, "{0:#,0.00}", OfferReportModel.OfferTotal.TotalRebate);
         string totalPrice = string.Format(CultureInfo, "{0:#,0.00}", OfferReportModel.OfferTotal.TotalPrice);
         string totalValueAddedTax = string.Format(CultureInfo, "{0:#,0.00}", OfferReportModel.OfferTotal.TotalValueAddedTax);
         string grandTotal = string.Format(CultureInfo, "{0:#,0.00}", OfferReportModel.OfferTotal.GrandTotal);

         row1.AddCell(Arial9BlackLeft, 1, LabelProvider["DOC_RebateTotal"]);
         row1.AddCell(Arial9BlackLeft, 2, totalRebate);

         row1.AddCell(Arial9BlackLeft.Clone().SetRightAlignment(), 3, LabelProvider["DOC_TotalWithoutVAT"]);
         row1.AddCell(Arial9BlackLeft.Clone().SetRightAlignment(), 5, totalPrice);

         row2.AddCell(Arial9BlackLeft.Clone().SetRightAlignment(), 3, LabelProvider["DOC_TotalVAT"]);
         row2.AddCell(Arial9BlackLeft.Clone().SetRightAlignment(), 5, totalValueAddedTax);

         row3.AddCell(Arial9BlackLeftBold.Clone().SetRightAlignment(), 3, LabelProvider["DOC_Total"]);
         row3.AddCell(Arial9BlackLeftBold.Clone().SetRightAlignment(), 5, grandTotal);

         return grid;
      }

      protected void AddBottomLine(PdfPageBase page, float yPosition)
      {
         string controlledBy = LabelProvider["DOC_ControlledBy"] + "    ********";
         string approvedBy = LabelProvider["DOC_ApprovedBy"] + "    ********";

         page.AddText(controlledBy, Arial8BlackLeft, 0, yPosition);
         page.AddText(approvedBy, Arial8BlackLeft, 230, yPosition);

         page.AddText(LabelProvider["DOC_Bottom1"], Arial9BlackLeft, 0, yPosition + 20);
         page.AddText(LabelProvider["DOC_Bottom2"], Arial9BlackLeft, 0, yPosition + 32);
         page.AddText(LabelProvider["DOC_IssuedBy"], Arial9BlackLeft, 0, yPosition + 44);
      }

      #region Private methods

      private void CreateCellStyles()
      {
         Arial12BlackLeft = ReportStyleManager.Instance.Get(CellStyleId.Arial12BlackLeft);
         Arial10BlackLeft = ReportStyleManager.Instance.Get(CellStyleId.Arial10BlackLeft);
         Arial9BlackLeft = ReportStyleManager.Instance.Get(CellStyleId.Arial9BlackLeft);
         Arial8BlackLeft = ReportStyleManager.Instance.Get(CellStyleId.Arial8BlackLeft);
         var arial8BlackLeftGray = ReportStyleManager.Instance.Get(CellStyleId.Arial8BlackLeftGray);

         Arial12BlackLeftBold = Arial12BlackLeft.Clone().SetBold();
         Arial10BlackLeftBold = Arial10BlackLeft.Clone().SetBold();
         Arial9BlackLeftBold = Arial9BlackLeft.Clone().SetBold();

         Arial8BlackLeftGrayTopBorder = arial8BlackLeftGray.Clone().RemoveAllBorders().SetTopBorder().SetBorderBrush(Color.Black, 0.1f);
         Arial8BlackLeftWhiteTopBorder = Arial8BlackLeft.Clone().RemoveAllBorders().SetTopBorder().SetBorderBrush(Color.Black, 0.1f);
         Arial8BlackLeftWhiteBottomBorder = Arial8BlackLeft.Clone().RemoveAllBorders().SetBottomBorder().SetBorderBrush(Color.Black, 0.1f);
         Arial8BlackLeftWhiteLeftBorder = Arial8BlackLeft.Clone().RemoveAllBorders().SetLeftBorder().SetBorderBrush(Color.Black, 0.1f);
         Arial8BlackLeftWhiteRightBorder = Arial8BlackLeft.Clone().RemoveAllBorders().SetRightBorder().SetBorderBrush(Color.Black, 0.1f);
      }

      private void AddHeaderRow(PdfGrid grid)
      {
         PdfGridRow headerRow = grid.AddRow();

         headerRow.AddCell(Arial8BlackLeftGrayTopBorder.Clone().SetLeftBorder(), 0, string.Empty);
         headerRow.AddCell(Arial8BlackLeftGrayTopBorder, 1, LabelProvider["DOC_OrdinalNumber"]);
         headerRow.AddCell(Arial8BlackLeftGrayTopBorder, 2, LabelProvider["DOC_Name"]);
         headerRow.AddCell(Arial8BlackLeftGrayTopBorder, 3, LabelProvider["DOC_Quantity"]);
         headerRow.AddCell(Arial8BlackLeftGrayTopBorder, 4, LabelProvider["DOC_Unit"]);
         headerRow.AddCell(Arial8BlackLeftGrayTopBorder, 5, LabelProvider["DOC_Price"]);
         headerRow.AddCell(Arial8BlackLeftGrayTopBorder, 6, LabelProvider["DOC_Rebate"]);
         headerRow.AddCell(Arial8BlackLeftGrayTopBorder, 7, LabelProvider["DOC_VAT"]);
         headerRow.AddCell(Arial8BlackLeftGrayTopBorder, 8, LabelProvider["DOC_TotalWithoutVAT"].Replace(":", ""));
         headerRow.AddCell(Arial8BlackLeftGrayTopBorder.Clone().SetRightBorder(), 9, LabelProvider["DOC_Amount"]);
      }

      private void AddEmptyRow(PdfGrid grid, bool isTop)
      {
         PdfGridRow emptyRow = grid.AddRow(15);
         CellStyle cellStyle1 = isTop
            ? Arial8BlackLeftWhiteLeftBorder.Clone().SetTopBorder()
            : Arial8BlackLeftWhiteLeftBorder.Clone().SetBottomBorder();
         CellStyle cellStyle2 = isTop
            ? Arial8BlackLeftWhiteTopBorder
            : Arial8BlackLeftWhiteBottomBorder;
         CellStyle cellStyle3 = isTop
            ? Arial8BlackLeftWhiteRightBorder.Clone().SetTopBorder()
            : Arial8BlackLeftWhiteRightBorder.Clone().SetBottomBorder();

         emptyRow.AddCell(cellStyle1, 0, string.Empty);

         for (int j = 1; j < 9; j++)
         {
            emptyRow.AddCell(cellStyle2, j, string.Empty);
         }

         emptyRow.AddCell(cellStyle3, 9, string.Empty);
      }

      #endregion
   }
}

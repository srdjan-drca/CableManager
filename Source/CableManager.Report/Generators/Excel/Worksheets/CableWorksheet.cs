﻿using System.IO;
using System.Drawing;
using System.Globalization;
using OfficeOpenXml;
using CableManager.Report.Models;
using CableManager.Report.Extensions;
using CableManager.Report.Helpers;
using OfficeOpenXml.Style;

namespace CableManager.Report.Generators.Excel.Worksheets
{
   public class CableWorksheet : BaseWorksheet
   {
      private int _rowId = 1;

      private int _columnId = 1;

      private const int DefaultRowHeightInPixels = 20;

      private readonly OfferReportModel _offerReportModel;

      private readonly string _sheetName;

      private readonly CultureInfo _cultureInfo;

      public CableWorksheet(BaseReportModel baseReportModel) : base(baseReportModel)
      {
         _offerReportModel = (OfferReportModel)baseReportModel;

         _sheetName = "Offer";

         _cultureInfo = new CultureInfo(_offerReportModel.LabelProvider.GetCulture());
      }

      public override string Name
      {
         get { return _sheetName; }
         set { }
      }

      public override void AddContent(ExcelWorksheet worksheet)
      {
         worksheet.SetColumnsWidth(1, 10, 16);
         worksheet.Column(1).Width = 5;
         worksheet.Column(2).Width = 40;

         for (int rowCounter = 1; rowCounter < 11; rowCounter++)
         {
            worksheet.Column(rowCounter).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
         }

         InsertCompanyDetailsContent(worksheet);
         InsertCustomerDetailsContent(worksheet);
         InsertLogo(worksheet);
         InsertOfferDetailsGrid(worksheet);
         InsertUserDetails(worksheet);
         InsertCableOfferGrid(worksheet);
         InsertCableOfferTotalsGrid(worksheet);
         InsertBottomLine(worksheet);
      }

      #region Private methods

      private void InsertCompanyDetailsContent(ExcelWorksheet worksheet)
      {
         string taxNumber = string.Join(" ", LabelProvider["DOC_TaxNumber"] + _offerReportModel.CompanyModelPdf?.TaxNumber);

         _rowId = 2;
         _columnId = 1;

         worksheet.Cells[2, 1, 5, 1].Style.Font.Bold = true;

         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CompanyModelPdf?.Name);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CompanyModelPdf?.Street);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CompanyModelPdf?.City);
         worksheet.Cells[_rowId++, _columnId].SetValue(taxNumber);
      }

      private void InsertCustomerDetailsContent(ExcelWorksheet worksheet)
      {
         _rowId = 7;
         _columnId = 2;

         worksheet.Cells[_rowId, _columnId].Style.Font.Bold = true;

         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerModelPdf.Name);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerModelPdf.Street);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerModelPdf.PostCodeAndCity);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerModelPdf.Country);
      }

      private void InsertLogo(ExcelWorksheet worksheet)
      {
         int numberOfRows = 5;
         var pixelTop = 5;
         var pixelLeft = 910;
         int pixelWidth = (numberOfRows+4) * DefaultRowHeightInPixels;
         int pixelHeight = numberOfRows * DefaultRowHeightInPixels;

         if (_offerReportModel.CompanyModelPdf.LogoPath != null)
         {
            string logoPath = new FileInfo(_offerReportModel.CompanyModelPdf.LogoPath).FullName;
            Image logoImage = Image.FromFile(logoPath);

            worksheet.InsertImage(logoImage, "Logo", pixelTop, pixelLeft, pixelWidth, pixelHeight);
         }
      }

      private void InsertOfferDetailsGrid(ExcelWorksheet worksheet)
      {
         _rowId = 7;
         _columnId = 6;

         worksheet.Cells[_rowId, _columnId].Style.Font.Bold = true;

         worksheet.Cells[_rowId++, _columnId].SetValue(LabelProvider["DOC_OfferNumber"]);
         worksheet.Cells[_rowId++, _columnId].SetValue(LabelProvider["DOC_Date"]);
         worksheet.Cells[_rowId++, _columnId].SetValue(LabelProvider["DOC_CustomerTaxNumber"]);
         worksheet.Cells[_rowId++, _columnId].SetValue(LabelProvider["DOC_WorthUntil"]);
         worksheet.Cells[_rowId++, _columnId].SetValue(LabelProvider["DOC_DeliveryDate"]);

         _rowId = 7;
         _columnId++;

         worksheet.Cells[_rowId, _columnId].Style.Font.Bold = true;

         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.Id);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.TimeDate);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerModelPdf.TaxNumber);
         worksheet.Cells[_rowId++, _columnId].SetValue(string.Empty);
         worksheet.Cells[_rowId++, _columnId].SetValue(string.Empty);
      }

      private void InsertUserDetails(ExcelWorksheet worksheet)
      {
         string userDetails = string.Join(" ", LabelProvider["DOC_DocumentComposedBy"], _offerReportModel.UserNumberAndName);
         string userEmail = string.Join(" ", LabelProvider["DOC_Email"], _offerReportModel.CustomerModelPdf.Email);
         string contactInfo = CreateContactInfo();

         worksheet.Cells[12, 1].SetValue(userDetails);
         worksheet.Cells[13, 1].SetValue(userEmail);
         worksheet.Cells[14, 1].SetValue(_offerReportModel.Note);
         worksheet.Cells[15, 1].SetValue(contactInfo);
         worksheet.Cells[16, 1].SetValue(_offerReportModel.CompanyModelPdf?.BankAccounts[0]);
         worksheet.Cells[17, 1].SetValue(_offerReportModel.CompanyModelPdf?.BankAccounts[1]);
      }

      private void InsertCableOfferGrid(ExcelWorksheet worksheet)
      {
         AddHeaderRow(worksheet);

         _rowId = 21;

         foreach (OfferItem offerItem in _offerReportModel.OfferItems)
         {
            _columnId = 1;

            worksheet.Cells[_rowId, _columnId++].SetValue(offerItem.SerialNumber);
            worksheet.Cells[_rowId, _columnId++].SetValue(offerItem.Name);
            worksheet.Cells[_rowId, _columnId++].SetValue(offerItem.Quantity);
            worksheet.Cells[_rowId, _columnId++].SetValue(offerItem.Unit);
            worksheet.Cells[_rowId, _columnId++].SetValue(string.Format(_cultureInfo, "{0:#,#.00}", offerItem.PricePerItem));
            worksheet.Cells[_rowId, _columnId++].SetValue(string.Format(_cultureInfo, "{0:#,#.00}", offerItem.Rebate));
            worksheet.Cells[_rowId, _columnId++].SetValue(string.Format(_cultureInfo, "{0:#,#.00}", offerItem.ValueAddedTax));
            worksheet.Cells[_rowId, _columnId++].SetValue(string.Format(_cultureInfo, "{0:#,#.00}", offerItem.TotalPriceWithRebate));
            worksheet.Cells[_rowId, _columnId++].SetValue(string.Format(_cultureInfo, "{0:#,#.00}", offerItem.TotalPriceWithVat));

            worksheet.Cells[_rowId, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            _rowId++;
         }

         worksheet.Cells[_rowId, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
         worksheet.Cells[_rowId, 1, _rowId, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
      }

      private void InsertCableOfferTotalsGrid(ExcelWorksheet worksheet)
      {
         _rowId += 2;

         string totalRebate = string.Format(_cultureInfo, "{0:#,#.00}", _offerReportModel.OfferTotal.TotalRebate);
         string totalPrice = string.Format(_cultureInfo, "{0:#,#.00}", _offerReportModel.OfferTotal.TotalPrice);
         string totalValueAddedTax = string.Format(_cultureInfo, "{0:#,#.00}", _offerReportModel.OfferTotal.TotalValueAddedTax);
         string grandTotal = string.Format(_cultureInfo, "{0:#,#.00}", _offerReportModel.OfferTotal.GrandTotal);

         worksheet.Cells[_rowId, 2].SetValue(LabelProvider["DOC_RebateTotal"]);
         worksheet.Cells[_rowId, 3].SetValue(totalRebate);

         worksheet.Cells[_rowId, 7].SetValue(LabelProvider["DOC_TotalWithoutVAT"]);
         worksheet.Cells[_rowId++, 9].SetValue(totalPrice);

         worksheet.Cells[_rowId, 7].SetValue(LabelProvider["DOC_TotalVAT"]);
         worksheet.Cells[_rowId++, 9].SetValue(totalValueAddedTax);

         worksheet.Cells[_rowId, 7].Style.Font.Bold = true;
         worksheet.Cells[_rowId, 9].Style.Font.Bold = true;

         worksheet.Cells[_rowId, 7].SetValue(LabelProvider["DOC_Total"]);
         worksheet.Cells[_rowId, 9].SetValue(grandTotal);
      }

      private void InsertBottomLine(ExcelWorksheet worksheet)
      {
         string controlledBy = LabelProvider["DOC_ControlledBy"] + "    ********";
         string approvedBy = LabelProvider["DOC_ApprovedBy"] + "    ********";

         _rowId++;

         worksheet.Cells[_rowId, 1].SetValue(controlledBy);
         worksheet.Cells[_rowId++, 4].SetValue(approvedBy);

         worksheet.Cells[_rowId++, 1].SetValue(LabelProvider["DOC_Bottom1"]);
         worksheet.Cells[_rowId++, 1].SetValue(LabelProvider["DOC_Bottom2"]);
         worksheet.Cells[_rowId++, 1].SetValue(LabelProvider["DOC_IssuedBy"]);
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

      private void AddHeaderRow(ExcelWorksheet worksheet)
      {
         _rowId = 20;
         _columnId = 1;

         worksheet.Cells[_rowId, 1, _rowId, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
         worksheet.Cells[_rowId, 1, _rowId, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
         worksheet.Cells[_rowId, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

         worksheet.Cells[_rowId, 1, _rowId, 9].Style.Font.Bold = true;
         worksheet.Cells[_rowId, 1, _rowId, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
         worksheet.Cells[_rowId, 1, _rowId, 9].Style.Fill.BackgroundColor.SetColor(ColorHelper.GrayF5F5F5);

         worksheet.Cells[_rowId, _columnId++].SetValue(LabelProvider["DOC_OrdinalNumber"]);
         worksheet.Cells[_rowId, _columnId++].SetValue(LabelProvider["DOC_Name"]);
         worksheet.Cells[_rowId, _columnId++].SetValue(LabelProvider["DOC_Quantity"]);
         worksheet.Cells[_rowId, _columnId++].SetValue(LabelProvider["DOC_Unit"]);
         worksheet.Cells[_rowId, _columnId++].SetValue(LabelProvider["DOC_Price"]);
         worksheet.Cells[_rowId, _columnId++].SetValue(LabelProvider["DOC_Rebate"]);
         worksheet.Cells[_rowId, _columnId++].SetValue(LabelProvider["DOC_VAT"]);
         worksheet.Cells[_rowId, _columnId++].SetValue(LabelProvider["DOC_TotalWithoutVAT"].Replace(":", ""));
         worksheet.Cells[_rowId, _columnId++].SetValue(LabelProvider["DOC_Amount"]);
      }

      #endregion Private methods
   }
}
using System.IO;
using OfficeOpenXml;
using CableManager.Report.Models;
using CableManager.Common.Helpers;
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

      public CableWorksheet(BaseReportModel baseReportModel) : base(baseReportModel)
      {
         _offerReportModel = (OfferReportModel)baseReportModel;

         _sheetName = "Test sheet";
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
         string taxNumber = string.Join(" ", LabelProvider["DOC_TaxNumber"] + _offerReportModel.CompanyDetails?.TaxNumber);

         _rowId = 2;
         _columnId = 1;

         worksheet.Cells[2, 1, 5, 1].Style.Font.Bold = true;

         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CompanyDetails?.Name);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CompanyDetails?.Street);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CompanyDetails?.City);
         worksheet.Cells[_rowId++, _columnId].SetValue(taxNumber);
      }

      private void InsertCustomerDetailsContent(ExcelWorksheet worksheet)
      {
         _rowId = 7;
         _columnId = 2;

         worksheet.Cells[_rowId, _columnId].Style.Font.Bold = true;

         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerDetails.Name);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerDetails.Street);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerDetails.PostCodeAndCity);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerDetails.Country);
      }

      private void InsertLogo(ExcelWorksheet worksheet)
      {
         int numberOfRows = 5;
         var pixelTop = 5;
         var pixelLeft = 930;
         int pixelWidth = (numberOfRows+3) * DefaultRowHeightInPixels;
         int pixelHeight = numberOfRows * DefaultRowHeightInPixels;

         var logo = FileHelper.GetResourceStream("CableManager.Report.Resources.Images.Logo.png");

         using (var memoryStream = new MemoryStream())
         {
            logo.CopyTo(memoryStream);

            worksheet.InsertImage(memoryStream, "Logo", pixelTop, pixelLeft, pixelWidth, pixelHeight);
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

         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.Id);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.TimeDate);
         worksheet.Cells[_rowId++, _columnId].SetValue(_offerReportModel.CustomerDetails.TaxNumber);
         worksheet.Cells[_rowId++, _columnId].SetValue(string.Empty);
         worksheet.Cells[_rowId++, _columnId].SetValue(string.Empty);
      }

      private void InsertUserDetails(ExcelWorksheet worksheet)
      {
         string userDetails = string.Join(" ", LabelProvider["DOC_DocumentComposedBy"], _offerReportModel.UserNumberAndName);
         string userEmail = string.Join(" ", LabelProvider["DOC_Email"], _offerReportModel.CustomerDetails.Email);
         string contactInfo = CreateContactInfo();

         worksheet.Cells[12, 1].SetValue(userDetails);
         worksheet.Cells[13, 1].SetValue(userEmail);
         worksheet.Cells[14, 1].SetValue(_offerReportModel.Note);
         worksheet.Cells[15, 1].SetValue(contactInfo);
         worksheet.Cells[16, 1].SetValue(_offerReportModel.CompanyDetails?.BankAccount1);
         worksheet.Cells[17, 1].SetValue(_offerReportModel.CompanyDetails?.BankAccount2);
      }

      private void InsertCableOfferGrid(ExcelWorksheet worksheet)
      {
         AddHeaderRow(worksheet);

         _rowId = 21;

         foreach (CableDetails cableDetails in _offerReportModel.Cables)
         {
            _columnId = 1;

            worksheet.Cells[_rowId, _columnId++].SetValue(cableDetails.SerialNumber);
            worksheet.Cells[_rowId, _columnId++].SetValue(cableDetails.Name);
            worksheet.Cells[_rowId, _columnId++].SetValue(cableDetails.Quantity);
            worksheet.Cells[_rowId, _columnId++].SetValue(cableDetails.Unit);
            worksheet.Cells[_rowId, _columnId++].SetValue(cableDetails.PricePerUnit);
            worksheet.Cells[_rowId, _columnId++].SetValue(cableDetails.Rebate);
            worksheet.Cells[_rowId, _columnId++].SetValue(cableDetails.Vat);
            worksheet.Cells[_rowId, _columnId++].SetValue(cableDetails.TotalPrice);
            worksheet.Cells[_rowId, _columnId++].SetValue(cableDetails.TotalPriceWithVat);

            _rowId++;
         }

         worksheet.Cells[_rowId, 1, _rowId, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
      }

      private void InsertCableOfferTotalsGrid(ExcelWorksheet worksheet)
      {
         _rowId += 2;

         worksheet.Cells[_rowId, 2].SetValue(LabelProvider["DOC_RebateTotal"]);
         worksheet.Cells[_rowId, 3].SetValue("5.284,14");

         worksheet.Cells[_rowId, 7].SetValue(LabelProvider["DOC_TotalWithoutVAT"]);
         worksheet.Cells[_rowId++, 9].SetValue("15.852,36");

         worksheet.Cells[_rowId, 7].SetValue(LabelProvider["DOC_TotalVAT"]);
         worksheet.Cells[_rowId++, 9].SetValue("3.963,09");

         worksheet.Cells[_rowId, 7].Style.Font.Bold = true;
         worksheet.Cells[_rowId, 9].Style.Font.Bold = true;

         worksheet.Cells[_rowId, 7].SetValue(LabelProvider["DOC_Total"]);
         worksheet.Cells[_rowId, 9].SetValue("19.815,45");
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
         string phones = string.Join(", ", _offerReportModel.CompanyDetails?.Phone1, _offerReportModel.CompanyDetails?.Phone2);
         string phoneInfo = string.Join(" ", LabelProvider["DOC_Phone"], phones);
         string faxInfo = string.Join(" ", LabelProvider["DOC_Fax"], _offerReportModel.CompanyDetails?.Fax);
         string mobileInfo = string.Join(" ", LabelProvider["DOC_Mobile"], _offerReportModel.CompanyDetails?.MobilePhone);
         string emailInfo = string.Join(" ", LabelProvider["DOC_Email"], _offerReportModel.CompanyDetails?.Email);
         string contactInfo = string.Join(", ", phoneInfo, faxInfo, mobileInfo, emailInfo);

         return contactInfo;
      }

      private void AddHeaderRow(ExcelWorksheet worksheet)
      {
         _rowId = 20;
         _columnId = 1;

         worksheet.Cells[_rowId, 1, _rowId, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
         worksheet.Cells[_rowId, 1, _rowId, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

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
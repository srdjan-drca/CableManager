using System.IO;
using System.Drawing;
using CableManager.Report.Extensions;
using CableManager.Report.Models;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;

namespace CableManager.Report.Generators.Pdf.Sections.Cable
{
   public class CableFirstPdfSection : BaseCablePdfSection
   {
      public CableFirstPdfSection(BaseReportModel baseReportModel) : base(baseReportModel)
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

         if (OfferReportModel.DisplayTotals)
         {
            page.Add(cableOfferTotalsGrid, 0, 360 + mainTableHeight);
            AddBottomLine(page, 410 + mainTableHeight);
         }

         page.Add(ReportFooter);

         return pdfDocument.ToMemoryStream();
      }

      #region Private methods

      private PdfGrid CreateCustomerDetailsContent()
      {
         PdfGrid grid = CreateGrid(150);
         PdfGridRow row1 = grid.AddRow();
         PdfGridRow row2 = grid.AddRow();
         PdfGridRow row3 = grid.AddRow();
         PdfGridRow row4 = grid.AddRow();

         row1.AddCell(Arial10BlackLeftBold, 0, OfferReportModel.CustomerModelPdf.Name);
         row2.AddCell(Arial10BlackLeft, 0, OfferReportModel.CustomerModelPdf.Street);
         row3.AddCell(Arial10BlackLeft, 0, OfferReportModel.CustomerModelPdf.PostCodeAndCity);
         row4.AddCell(Arial10BlackLeft, 0, OfferReportModel.CustomerModelPdf.Country);

         return grid;
      }

      private PdfImage CreateLogoImage()
      {
         PdfImage logo = null;

         if (!string.IsNullOrEmpty(OfferReportModel.CompanyModelPdf?.LogoPath))
         {
            string logoPath = new FileInfo(OfferReportModel.CompanyModelPdf.LogoPath).FullName;
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

         row1.AddCell(Arial12BlackLeftBold, 0, LabelProvider["DOC_OfferNumber"]);
         row1.AddCell(Arial12BlackLeftBold, 1, OfferReportModel.Id);

         row2.AddCell(Arial9BlackLeft, 0, LabelProvider["DOC_Date"]);
         row2.AddCell(Arial9BlackLeft, 1, OfferReportModel.TimeDate);

         row3.AddCell(Arial9BlackLeft, 0, LabelProvider["DOC_CustomerTaxNumber"]);
         row3.AddCell(Arial9BlackLeft, 1, OfferReportModel.CustomerModelPdf.TaxNumber);

         row4.AddCell(Arial9BlackLeft, 0, LabelProvider["DOC_WorthUntil"]);
         row4.AddCell(Arial9BlackLeft, 1, string.Empty);

         row5.AddCell(Arial9BlackLeft, 0, LabelProvider["DOC_DeliveryDate"]);
         row5.AddCell(Arial9BlackLeft, 1, string.Empty);

         return grid;
      }

      private void AddUserDetails(PdfPageBase page)
      {
         string userDetails = string.Join(" ",LabelProvider["DOC_DocumentComposedBy"], OfferReportModel.UserNumberAndName);
         string userEmail = string.Join(" ", LabelProvider["DOC_Email"], OfferReportModel.CustomerModelPdf.Email);
         string contactInfo = CreateContactInfo();

         page.AddText(userDetails, Arial9BlackLeft, 0, 240);
         page.AddText(userEmail, Arial9BlackLeft, 0, 250);
         page.AddText(OfferReportModel.Note, Arial9BlackLeft, 0, 260);
         page.AddText(contactInfo, Arial9BlackLeft, 0, 290);

         if (OfferReportModel.CompanyModelPdf?.BankAccounts != null)
         {
            float yPositionStart = 310;

            foreach (string bankAccount in OfferReportModel.CompanyModelPdf?.BankAccounts)
            {
               page.AddText(bankAccount, Arial9BlackLeft, 0, yPositionStart);
               yPositionStart += 10;
            }
         }
      }

      private string CreateContactInfo()
      {
         string phones = string.Join(", ", OfferReportModel.CompanyModelPdf?.Phone1, OfferReportModel.CompanyModelPdf?.Phone2);
         string phoneInfo = string.Join(" ", LabelProvider["DOC_Phone"], phones);
         string faxInfo = string.Join(" ", LabelProvider["DOC_Fax"], OfferReportModel.CompanyModelPdf?.Fax);
         string mobileInfo = string.Join(" ", LabelProvider["DOC_Mobile"], OfferReportModel.CompanyModelPdf?.MobilePhone);
         string emailInfo = string.Join(" ", LabelProvider["DOC_Email"], OfferReportModel.CompanyModelPdf?.Email);
         string contactInfo = string.Join(", ", phoneInfo, faxInfo, mobileInfo, emailInfo);

         return contactInfo;
      }

      #endregion
   }
}
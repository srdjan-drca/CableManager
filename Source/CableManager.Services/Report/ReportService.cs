using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Spire.Pdf;
using CableManager.Report;
using CableManager.Report.Models;
using CableManager.Localization;
using CableManager.Common.Helpers;
using CableManager.Common.Result;
using CableManager.Repository.Models;
using CableManager.Services.Calculation.Models;

namespace CableManager.Services.Report
{
   public class ReportService : IReportService
   {
      private readonly ICableManagerReport _cableManagerReport;

      private string _language;

      public ReportService(ICableManagerReport cableManagerReport)
      {
         _cableManagerReport = cableManagerReport;
      }

      public ReturnResult GeneratePdfReport(Offer offer)
      {
         ReturnResult result;

         _language = offer.Language;

         try
         {
            BaseReportModel reportModel = CreateBaseReportModel(offer);
            PdfDocumentBase pdfDocument = _cableManagerReport.GenerateOfferPdf(reportModel);

            //persist & display document
            string offersDirectory = Path.GetDirectoryName(offer.FullName);
            DirectoryHelper.CreateDirectory(offersDirectory);
            pdfDocument.Save(offer.FullName, FileFormat.PDF);
            Process.Start(offer.FullName);

            result = new SuccessResult(string.Empty);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      public ReturnResult GenerateExcelReport(Offer offer)
      {
         ReturnResult result;

         _language = offer.Language;

         try
         {
            BaseReportModel reportModel = CreateBaseReportModel(offer);
            MemoryStream excelDocument = _cableManagerReport.GenerateOfferExcel(reportModel);

            //persist & display document
            string offersDirectory = Path.GetDirectoryName(offer.FullName);
            DirectoryHelper.CreateDirectory(offersDirectory);
            FileHelper.SaveToDisk(excelDocument, offer.FullName);
            Process.Start(offer.FullName);

            excelDocument.Dispose();

            result = new SuccessResult(string.Empty);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      public ReturnResult DeleteReport(List<string> fullFileNames)
      {
         ReturnResult result;

         try
         {
            foreach (string fileName in fullFileNames)
            {
               string offerFile = new FileInfo(fileName).FullName;

               File.Delete(offerFile);
            }

            result = new SuccessResult();
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      #region Private methods

      private BaseReportModel CreateBaseReportModel(Offer offer)
      {
         UserModel user = offer.User;

         var offerReportModel = new OfferReportModel
         {
            Id = "PO" + user.Number + "-" + user.LastOfferNumber,
            UserNumberAndName = string.Join(" ", user.Number, user.FirstName, user.LastName),
            Note = offer.Note,
            CustomerModelPdf = CreateCustomerDetails(offer.Customer),
            CompanyModelPdf = CreateCompanyDetails(offer.Company),
            OfferItems = offer.Items,
            OfferTotal = offer.Total,
            TimeDate = DateTime.Now.ToString("dd/MM/yyyy"),
            LabelProvider = new LabelProvider(),
         };

         offerReportModel.LabelProvider.SetCulture(_language);

         return offerReportModel;
      }

      private CustomerModelPdf CreateCustomerDetails(CustomerModel customer)
      {
         return new CustomerModelPdf
         {
            Name = customer.Name,
            Street = customer.Street,
            PostCodeAndCity = string.Join(" ", customer.PostalCode, customer.City),
            Country = customer.Country,
            TaxNumber = customer.TaxNumber,
            Email = customer.Email
         };
      }

      private CompanyModelPdf CreateCompanyDetails(CompanyModel company)
      {
         CompanyModelPdf companyModelPdf = null;

         if (company != null)
         {
            companyModelPdf = new CompanyModelPdf
            {
               Name = company.Name,
               Street = company.Street,
               City = company.City,
               TaxNumber = company.TaxNumber,
               Phone1 = company.Phone1,
               Phone2 = company.Phone2,
               MobilePhone = company.Mobile,
               Fax = company.Fax,
               Email = company.Email,
               LogoPath = company.LogoPath,
               BankAccounts = company.BankAccounts
            };
         }

         return companyModelPdf;
      }

      #endregion
   }
}
using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using Spire.Pdf;
using CableManager.Report;
using CableManager.Report.Models;
using CableManager.Report.Result;
using CableManager.Localization;
using CableManager.Common.Helpers;
using CableManager.Common.Result;
using CableManager.Repository.Company;
using CableManager.Repository.Customer;
using CableManager.Repository.Models;
using CableManager.Services.User;

namespace CableManager.Services.Report
{
   public class ReportService : IReportService
   {
      private readonly ICableManagerReport _cableManagerReport;

      private readonly ICustomerRepository _customerRepository;

      private readonly ICompanyRepository _companyRepository;

      private readonly IUserService _userService;

      private readonly string _offersDirectory;

      private string _language;

      public ReportService(ICableManagerReport cableManagerReport, ICustomerRepository customerRepository,
         ICompanyRepository companyRepository, IUserService userService)
      {
         _cableManagerReport = cableManagerReport;
         _customerRepository = customerRepository;
         _companyRepository = companyRepository;
         _userService = userService;

         _offersDirectory = new DirectoryInfo(DirectoryHelper.GetApplicationStoragePath() + "/Offers").FullName;
      }

      public OfferResult GeneratePdfReport(string filePrefix, string customerId, string note, string language)
      {
         OfferResult result;
         string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
         var offerName = CreateOfferName(filePrefix, date, ".pdf");
         var offerFullName = CreateFullName(offerName);

         _language = language;

         try
         {
            UserModel user = _userService.GetCurrentlyLoggedInUser();

            DirectoryHelper.CreateDirectory(_offersDirectory);
            string offerFile = new FileInfo(_offersDirectory + @"/" + offerName).FullName;
            BaseReportModel reportModel = CreateBaseReportModel(customerId, note, user);

            _userService.UpdateLastOfferNumber(user.Id, user.LastOfferNumber);

            PdfDocumentBase pdfDocument = _cableManagerReport.GenerateOfferPdf(reportModel);

            pdfDocument.Save(offerFile, FileFormat.PDF);
            Process.Start(offerFile);

            result = new OfferResult(true, string.Empty)
            {
               Date = date,
               FileName = offerName,
               FileFullName = offerFullName
            };
         }
         catch (Exception exception)
         {
            result = new OfferResult(false, exception.Message);
         }

         return result;
      }

      public OfferResult GenerateExcelReport(string filePrefix, string customerId, string note, string language)
      {
         OfferResult result;
         string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
         var offerName = CreateOfferName(filePrefix, date, ".xlsx");
         var offerFullName = CreateFullName(offerName);

         _language = language;

         try
         {
            UserModel user = _userService.GetCurrentlyLoggedInUser();

            DirectoryHelper.CreateDirectory(_offersDirectory);
            BaseReportModel reportModel = CreateBaseReportModel(customerId, note, user);

            _userService.UpdateLastOfferNumber(user.Id, user.LastOfferNumber);

            MemoryStream excelDocument = _cableManagerReport.GenerateOfferExcel(reportModel);
            FileHelper.SaveToDisk(excelDocument, offerFullName);

            excelDocument.Dispose();

            Process.Start(offerFullName);

            result = new OfferResult(true, string.Empty)
            {
               Date = date,
               FileName = offerName,
               FileFullName = offerFullName
            };
         }
         catch (Exception exception)
         {
            result = new OfferResult(false, exception.Message);
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

      private string CreateOfferName(string customerName, string date, string extension)
      {
         string date3 = string.Join("_", date.Split(new[] { " " }, StringSplitOptions.None).Take(2));
         string dateNormalized = date3.Replace("/", "_").Replace(":", "_");

         return customerName + "_" + dateNormalized + extension;
      }

      private string CreateFullName(string name)
      {
         return new FileInfo(DirectoryHelper.GetApplicationStoragePath() + "/Offers/" + name).FullName;
      }

      private BaseReportModel CreateBaseReportModel(string customerId, string note, UserModel user)
      {
         var offerReportModel = new OfferReportModel
         {
            Id = "PO" + user.Number + "-" + user.LastOfferNumber,
            UserNumberAndName = string.Join(" ", user.Number, user.FirstName, user.LastName),
            Note = note,
            CustomerDetails = CreateCustomerDetails(customerId),
            CompanyDetails = CreateCompanyDetails(),
            Cables = CreateCableOffer(),
            TimeDate = DateTime.Now.ToString("dd/MM/yyyy"),
            LabelProvider = new LabelProvider()
         };

         offerReportModel.LabelProvider.SetCulture(_language);

         return offerReportModel;
      }

      private CustomerDetails CreateCustomerDetails(string customerId)
      {
         CustomerModel customer = _customerRepository.Get(customerId);

         return new CustomerDetails
         {
            Name = customer.Name,
            Street = customer.Street,
            PostCodeAndCity = string.Join(" ", customer.PostalCode, customer.City),
            Country = customer.Country,
            TaxNumber = customer.TaxNumber,
            Email = customer.Email
         };
      }

      private CompanyDetails CreateCompanyDetails()
      {
         CompanyModel company = _companyRepository.GetAll().FirstOrDefault();
         CompanyDetails companyDetails = null;

         if (company != null)
         {
            companyDetails = new CompanyDetails
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
               BankAccount1 = "ERSTE BANK: HR3124020061100678549",
               BankAccount2 = "SOCIETE GENERALE BANKA: HR7823300031153012079"
            };
         }

         return companyDetails;
      }

      private List<CableDetails> CreateCableOffer()
      {
         return new List<CableDetails>
         {
            CreateItem1(),
            CreateItem2()
         };
      }

      private CableDetails CreateItem1()
      {
         return new CableDetails
         {
            SerialNumber = 1,
            Name = "KABEL PPY 3x1,5mm NYM-J",
            Quantity = 1057,
            Unit = "m",
            PricePerUnit = 4.07f,
            Rebate = 25,
            Vat = 25,
            TotalPrice = 3226.49f,
            TotalPriceWithVat = 4033.11f
         };
      }

      private CableDetails CreateItem2()
      {
         return new CableDetails
         {
            SerialNumber = 2,
            Name = "KABEL PPY 5x1,5mm NYM-J (100)",
            Quantity = 296,
            Unit = "m",
            PricePerUnit = 6.77f,
            Rebate = 25,
            Vat = 25,
            TotalPrice = 1502.94f,
            TotalPriceWithVat = 1878.68f
         };
      }
   }
}
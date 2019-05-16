using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Spire.Pdf;
using CableManager.Report;
using CableManager.Localization;
using CableManager.Common.Helpers;
using CableManager.Common.Result;
using CableManager.Report.Models;
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

      public ReportService(ICableManagerReport cableManagerReport, ICustomerRepository customerRepository,
         ICompanyRepository companyRepository, IUserService userService)
      {
         _cableManagerReport = cableManagerReport;
         _customerRepository = customerRepository;
         _companyRepository = companyRepository;
         _userService = userService;

         _offersDirectory = new DirectoryInfo(DirectoryHelper.GetApplicationStoragePath() + "/Offers").FullName;
      }

      public ReturnResult GeneratePdfReport(string fileName, string language)
      {
         ReturnResult result;

         try
         {
            DirectoryHelper.CreateDirectory(_offersDirectory);
            string offerFile = new FileInfo(_offersDirectory + @"/" + fileName).FullName;
            BaseReportModel reportModel = CreateBaseReportModel(language);

            var pdfDocument = _cableManagerReport.GenerateOfferPdf(reportModel);

            pdfDocument.Save(offerFile, FileFormat.PDF);
            Process.Start(offerFile);

            result = new SuccessResult();
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      public ReturnResult GenerateExcelReport(string fileName, string language)
      {
         ReturnResult result;

         try
         {
            DirectoryHelper.CreateDirectory(_offersDirectory);
            string offerFile = new FileInfo(_offersDirectory + @"/" + fileName).FullName;
            BaseReportModel reportModel = CreateBaseReportModel(language);

            result = _cableManagerReport.GenerateOfferExcel(offerFile, reportModel);

            Process.Start(offerFile);
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

      //--------/////////////////////////////////////////////////////////////////////
      // CODE BELOW IS TEMPORARY, TO BE DELETED WHEN DTO BUILDERS ARE FINISHED
      //--------/////////////////////////////////////////////////////////////////////

      private BaseReportModel CreateBaseReportModel(string language)
      {
         UserModel user = _userService.GetCurrentlyLoggedInUser();

         var offerReportModel = new OfferReportModel
         {
            Id = "PO0005544",
            UserNumberAndName = string.Join(" ", user.Number, user.FirstName, user.LastName),
            Note = "kablovi vrtić",
            CustomerDetails = CreateCustomerDetails(),
            CompanyDetails = CreateCompanyDetails(),
            Cables = CreateCableOffer(),
            TimeDate = DateTime.Now.ToString("dd/MM/yyyy"),
            LabelProvider = new LabelProvider()
         };

         offerReportModel.LabelProvider.SetCulture(language);

         return offerReportModel;
      }

      private CustomerDetails CreateCustomerDetails()
      {
         return new CustomerDetails
         {
            Name = "BGS ELEKTRIKA d.o.o.",
            Street = "Trešnjica 5",
            PostCodeAndCity = "35000 SLAVONSKI BROD",
            Country = "HRVATSKA",
            TaxNumber = "91792295423",
            Email = "info@bgs.hr"
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
            ItemCode = 200002,
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
            ItemCode = 200016,
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
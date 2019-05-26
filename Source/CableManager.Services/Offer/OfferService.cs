using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CableManager.Common.Helpers;
using CableManager.Localization;
using CableManager.Report.Models;
using CableManager.Repository.Cable;
using CableManager.Repository.Company;
using CableManager.Repository.Customer;
using CableManager.Repository.Models;
using CableManager.Repository.PriceDocument;
using CableManager.Services.Helpers;
using CableManager.Services.Offer.Models;
using CableManager.Services.Search;
using CableManager.Services.Search.Model;
using CableManager.Services.User;
using OfficeOpenXml;
using CableManager.PriceLoader.Core;
using CableManager.PriceLoader.Models;

namespace CableManager.Services.Offer
{
   public class OfferService : IOfferService
   {
      private readonly LabelProvider _labelProvider;

      private readonly ICableRepository _cableRepository;

      private readonly ICompanyRepository _companyRepository;

      private readonly ICablePriceDocumentRepository _cablePriceDocumentRepository;

      private readonly ICustomerRepository _customerRepository;

      private readonly ICableSearchService _cableSearchService;

      private readonly IUserService _userService;

      private readonly IPriceLoader _priceLoader;

      public OfferService(LabelProvider labelProvider, ICableRepository cableRepository, ICompanyRepository companyRepository,
         ICablePriceDocumentRepository cablePriceDocumentRepository, ICustomerRepository customerRepository,
         ICableSearchService cableSearchService, IUserService userService, IPriceLoader priceLoader)
      {
         _labelProvider = labelProvider;
         _cableRepository = cableRepository;
         _companyRepository = companyRepository;
         _cablePriceDocumentRepository = cablePriceDocumentRepository;
         _customerRepository = customerRepository;
         _cableSearchService = cableSearchService;
         _userService = userService;
         _priceLoader = priceLoader;
      }

      public OfferModel CreateOffer(string customerRequestFilePath, string customerId, string note, OfferType offerType)
      {
         var fileInfo = new FileInfo(customerRequestFilePath);
         Stream customerRequestFile = File.OpenRead(fileInfo.FullName);
         List<CableModel> cableNames = _cableRepository.GetAll();
         List<List<string>> searchCriteriaList = CreateSearchCriteria(cableNames);
         List<Cable> cables = _cableSearchService.GetCables(customerRequestFile, searchCriteriaList);
         List<PriceDocumentModel> cablePriceDocuments = _cablePriceDocumentRepository.GetAll();
         List<PriceModel> prices = LoadPrices(cablePriceDocuments);

         string extension = offerType == OfferType.Excel ? ".xlsx" : ".pdf";
         CustomerModel customer = _customerRepository.Get(customerId);
         string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
         var offerName = CreateOfferName(customer.Name, date, extension);
         var offerFullName = CreateFullName(offerName);
         CompanyModel company = _companyRepository.GetAll().FirstOrDefault();
         UserModel user = _userService.GetCurrentlyLoggedInUser();
         List<OfferItem> offerItems = CreateOfferItems(cables, customer);

         var offer = new OfferModel
         {
            Name = offerName,
            FullName = offerFullName,
            Note = note,
            Date = date,
            Language = _labelProvider.GetCulture(),
            Customer = customer,
            Company = company,
            User = user,
            Items = offerItems,
            Total = CreateOfferTotals(offerItems)
         };

         return offer;
      }

      public List<CableModel> LoadCableNames(string fileName)
      {
         Dictionary<string, string> cableDefinitions = _cableRepository.GetAll().ToDictionary(x => x.Name, x => x.Synonyms);
         var cables = new List<CableModel>();
         var fileInfo = new FileInfo(fileName);

         using (var excelPackage = new ExcelPackage(fileInfo))
         {
            ExcelWorksheet cableNamesWorksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
            ExcelCellAddress start = cableNamesWorksheet?.Dimension.Start;
            ExcelCellAddress end = cableNamesWorksheet?.Dimension.End;

            if (start != null && end != null)
            {
               for (int row = start.Row; row <= end.Row; row++)
               {
                  string name = cableNamesWorksheet.Cells[row, 1].Text?.Trim();

                  if (!cableDefinitions.ContainsKey(name) && cables.All(x => x.Name != name))
                  {
                     string synonyms = cableNamesWorksheet.Cells[row, 2].Text?.Trim();

                     cables.Add(new CableModel(name, synonyms));
                  }
               }
            }
         }

         return cables;
      }

      #region Private methods

      private List<List<string>> CreateSearchCriteria(List<CableModel> cablesDb)
      {
         var cableNames = new List<List<string>>();

         foreach (CableModel cableModel in cablesDb)
         {
            List<string> synonyms = cableModel.Synonyms.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            synonyms.Add(cableModel.Name.Trim());

            cableNames.Add(synonyms);
         }

         return cableNames;
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

      private List<OfferItem> CreateOfferItems(List<Cable> cables, CustomerModel customer)
      {
         var offerItems = new List<OfferItem>();
         int serialNumber = 1;
         int valueAddedTax = 25;

         foreach (Cable cable in cables)
         {
            float totalPrice = cable.Quantity * cable.Price;
            float totalPriceWithVat = totalPrice + CalculationHelper.CalculatePercent(totalPrice, valueAddedTax);

            var offerItem = new OfferItem
            {
               SerialNumber = serialNumber,
               Name = cable.Name,
               Quantity = cable.Quantity,
               Unit = "m",
               PricePerItem = cable.Price,
               Rebate = (float)Convert.ToDouble(customer.Rebate),
               TotalPrice = totalPrice,
               TotalPriceWithVat = totalPriceWithVat,
               ValueAddedTax = valueAddedTax
            };

            offerItems.Add(offerItem);

            serialNumber++;
         }

         return offerItems;
      }

      private OfferTotal CreateOfferTotals(List<OfferItem> offerItems)
      {
         float totalPrice = CalculationHelper.CalculateTotalPrice(offerItems);
         float totalPriceWithVat = CalculationHelper.CalculateTotalPriceWithVat(offerItems);
         float rebate = CalculationHelper.CalculateRebate(offerItems);

         var offerTotals = new OfferTotal
         {
            TotalPrice = totalPrice,
            TotalValueAddedTax = totalPriceWithVat - totalPrice,
            GrandTotal = totalPriceWithVat,
            TotalRebate = rebate
         };

         return offerTotals;
      }

      private List<PriceModel> LoadPrices(List<PriceDocumentModel> priceDocuments)
      {
         var prices = new List<PriceModel>();

         foreach (PriceDocumentModel priceDocument in priceDocuments)
         {
            string extension = Path.GetExtension(priceDocument.Path);

            switch (extension)
            {
               case ".pdf":
                  List<PriceModel> priceModelsPdf = _priceLoader.LoadPricesFromPdf(priceDocument.Path, priceDocument.Id);
                  prices.AddRange(priceModelsPdf);
                  break;

               case ".xlsx":
                  List<PriceModel> priceModelsExcel = _priceLoader.LoadPricesFromExcel(priceDocument.Path, priceDocument.Id);
                  prices.AddRange(priceModelsExcel);
                  break;

               default:
                  throw new Exception("Document format not supported");
            }
         }

         return prices;
      }

      #endregion
   }
}

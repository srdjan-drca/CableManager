using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using OfficeOpenXml;
using CableManager.Common.Helpers;
using CableManager.Localization;
using CableManager.ModelConverter;
using CableManager.Report.Models;
using CableManager.Repository.CableName;
using CableManager.Repository.CablePrice;
using CableManager.Repository.Company;
using CableManager.Repository.Customer;
using CableManager.Repository.Models;
using CableManager.Services.Helpers;
using CableManager.Services.Offer.Models;
using CableManager.Services.Search;
using CableManager.Services.Search.Model;
using CableManager.Services.User;
using CableManager.PriceLoader.Models;

namespace CableManager.Services.Offer
{
   public class OfferService : IOfferService
   {
      private readonly LabelProvider _labelProvider;

      private readonly ICableNameRepository _cableNameRepository;

      private readonly ICablePriceRepository _cablePriceRepository;

      private readonly ICompanyRepository _companyRepository;

      private readonly ICustomerRepository _customerRepository;

      private readonly ICableSearchService _cableSearchService;

      private readonly IUserService _userService;

      private readonly ICablePriceModelConverter _cablePriceModelConverter;

      public OfferService(LabelProvider labelProvider, ICableNameRepository cableNameRepository, ICablePriceRepository cablePriceRepository,
         ICompanyRepository companyRepository, ICustomerRepository customerRepository, ICableSearchService cableSearchService,
         IUserService userService, ICablePriceModelConverter cablePriceModelConverter)
      {
         _labelProvider = labelProvider;
         _cableNameRepository = cableNameRepository;
         _cablePriceRepository = cablePriceRepository;
         _companyRepository = companyRepository;
         _customerRepository = customerRepository;
         _cableSearchService = cableSearchService;
         _userService = userService;
         _cablePriceModelConverter = cablePriceModelConverter;
      }

      public OfferModel CreateOffer(OfferParameters offerParameters)
      {
         var fileInfo = new FileInfo(offerParameters.CustomerRequestFilePath);
         Stream customerRequestFile = File.OpenRead(fileInfo.FullName);
         List<CableModel> cableNames = _cableNameRepository.GetAll();
         List<List<string>> searchCriteriaList = CreateSearchCriteria(cableNames);
         List<CableDetails> cables = _cableSearchService.GetCables(customerRequestFile, searchCriteriaList);

         List<CablePriceDbModel> cablePriceDbModels = _cablePriceRepository.GetAll();
         List<CablePriceModel> prices = _cablePriceModelConverter.ToModels(cablePriceDbModels);
         List<CablePriceModel> filteredPrices = prices.Where(x => offerParameters.PriceDocumentIds.Contains(x.DocumentGuid)).ToList();

         foreach (CableDetails cable in cables)
         {
            float price = FindPrice(cable, filteredPrices);

            cable.Price = price;
         }

         string extension = offerParameters.OfferType == OfferType.Excel ? ".xlsx" : ".pdf";
         CustomerModel customer = _customerRepository.Get(offerParameters.CustomerId);
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
            Note = offerParameters.Note,
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
         Dictionary<string, string> cableDefinitions = _cableNameRepository.GetAll().ToDictionary(x => x.Name, x => x.Synonyms);
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

      private float FindPrice(CableDetails cableDetails, List<CablePriceModel> prices)
      {
         float price = 0;

         foreach (CablePriceModel priceModel in prices)
         {
            bool isFound = priceModel.GetPrice(cableDetails.SearchCriteria, cableDetails.CableType, out price);

            if (isFound)
            {
               break;
            }
         }

         return price;
      }

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

      private List<OfferItem> CreateOfferItems(List<CableDetails> cables, CustomerModel customer)
      {
         var offerItems = new List<OfferItem>();
         int serialNumber = 1;
         int valueAddedTax = 25;

         foreach (CableDetails cable in cables)
         {
            float rebate = string.IsNullOrEmpty(customer.Rebate) ? 0 : (float)Convert.ToDouble(customer.Rebate);
            float totalPrice = CalculationHelper.CalculatePrice(cable.Price, 0, cable.Quantity);
            float totalPriceWithRebate = CalculationHelper.CalculatePrice(cable.Price, rebate, cable.Quantity);
            float totalPriceWithVat = CalculationHelper.CalculatePriceWithVat(totalPriceWithRebate, valueAddedTax);

            var offerItem = new OfferItem
            {
               SerialNumber = serialNumber,
               Name = string.Join("", cable.Name.Take(36)),
               Quantity = cable.Quantity,
               Unit = "m",
               PricePerItem = cable.Price,
               Rebate = rebate,
               TotalPrice = totalPrice,
               TotalPriceWithRebate = totalPriceWithRebate,
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
         float totalPriceWithRebate = CalculationHelper.CalculateTotalPriceWithRebate(offerItems);
         float totalPriceWithRebateAndVat = CalculationHelper.CalculateTotalPriceWithVat(offerItems);

         var offerTotals = new OfferTotal
         {
            TotalPrice = totalPriceWithRebate,
            TotalValueAddedTax = totalPriceWithRebateAndVat - totalPriceWithRebate,
            GrandTotal = totalPriceWithRebateAndVat,
            TotalRebate = totalPrice - totalPriceWithRebate
         };

         return offerTotals;
      }

      #endregion
   }
}

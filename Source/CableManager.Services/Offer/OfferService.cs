using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using OfficeOpenXml;
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

      public OfferModel CreateOffer(OfferParameters offerParameters)
      {
         var fileInfo = new FileInfo(offerParameters.CustomerRequestFilePath);
         Stream customerRequestFile = File.OpenRead(fileInfo.FullName);
         List<CableModel> cableNames = _cableRepository.GetAll();
         List<List<string>> searchCriteriaList = CreateSearchCriteria(cableNames);
         List<Cable> cables = _cableSearchService.GetCables(customerRequestFile, searchCriteriaList);
         List<PriceDocumentModel> cablePriceDocuments = _cablePriceDocumentRepository.GetAll();
         List<PriceModel> prices = LoadPrices(cablePriceDocuments, searchCriteriaList);
         List<PriceModel> filteredPrices = prices.Where(x => offerParameters.PriceDocumentIds.Contains(x.DocumentGuid)).ToList();

         foreach (Cable cable in cables)
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

      private float FindPrice(Cable cable, List<PriceModel> prices)
      {
         foreach (PriceModel priceModel in prices)
         {
            foreach (string priceModelCableName in priceModel.CableNames)
            {
               if (cable.SearchCriteria.Contains(priceModelCableName))
               {
                  string cableName = cable.Name.Split(' ').FirstOrDefault(x => x.Contains("mm2"))?.Replace(" ", string.Empty).Replace(",", ".").Replace("mm2", string.Empty);

                  if (cableName != null)
                  {
                     foreach (PriceItem priceItem in priceModel.PriceItems)
                     {
                        if (priceItem.Name.Contains(cableName))
                        {
                           return priceItem.Price;
                        }
                     }
                  }
               }
            }
         }

         return 0;
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
            float rebate = (float) Convert.ToDouble(customer.Rebate);
            float totalPrice = CalculationHelper.CalculatePrice(cable.Price, 0, cable.Quantity);
            float totalPriceWithRebate = CalculationHelper.CalculatePrice(cable.Price, rebate, cable.Quantity);
            float totalPriceWithVat = CalculationHelper.CalculatePriceWithVat(totalPriceWithRebate, valueAddedTax);

            var offerItem = new OfferItem
            {
               SerialNumber = serialNumber,
               Name = cable.Name,
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

      private List<PriceModel> LoadPrices(List<PriceDocumentModel> priceDocuments, List<List<string>> searchCriteriaList)
      {
         var prices = new List<PriceModel>();

         foreach (PriceDocumentModel priceDocument in priceDocuments)
         {
            string extension = Path.GetExtension(priceDocument.Path);

            switch (extension)
            {
               case ".pdf":
                  List<PriceModel> priceModelsPdf = _priceLoader.LoadPricesFromPdf(priceDocument.Path, priceDocument.Id);

                  foreach (PriceModel priceModel in priceModelsPdf)
                  {
                     foreach (List<string> searchCriteria in searchCriteriaList)
                     {
                        var intersect = priceModel.CableNames.Intersect(searchCriteria);

                        if (intersect.Any())
                        {
                           priceModel.CableNames.AddRange(searchCriteria);
                           priceModel.CableNames = priceModel.CableNames.Distinct().ToList();
                        }
                     }
                  }

                  prices.AddRange(priceModelsPdf);
                  break;

               case ".xlsx":
                  List<PriceModel> priceModelsExcel = _priceLoader.LoadPricesFromExcel(priceDocument.Path, priceDocument.Id);

                  foreach (PriceModel priceModel in priceModelsExcel)
                  {
                     foreach (List<string> searchCriteria in searchCriteriaList)
                     {
                        var intersect = priceModel.CableNames.Intersect(searchCriteria);

                        if (intersect.Any())
                        {
                           priceModel.CableNames.AddRange(searchCriteria);
                           priceModel.CableNames = priceModel.CableNames.Distinct().ToList();
                        }
                     }
                  }

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

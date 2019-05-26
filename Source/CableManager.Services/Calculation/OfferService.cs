using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using CableManager.Common.Extensions;
using CableManager.Common.Helpers;
using CableManager.Localization;
using CableManager.Report.Models;
using CableManager.Repository.Cable;
using CableManager.Repository.Company;
using CableManager.Repository.Customer;
using CableManager.Repository.Models;
using CableManager.Repository.PriceDocument;
using CableManager.Services.Calculation.Models;
using CableManager.Services.Helpers;
using CableManager.Services.Search;
using CableManager.Services.Search.Model;
using CableManager.Services.User;
using OfficeOpenXml;

namespace CableManager.Services.Calculation
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

      private readonly CultureInfo _commaCulture = new CultureInfo("en")
      {
         NumberFormat =
         {
            NumberDecimalSeparator = ","
         }
      };

      public OfferService(LabelProvider labelProvider, ICableRepository cableRepository, ICompanyRepository companyRepository,
         ICablePriceDocumentRepository cablePriceDocumentRepository, ICustomerRepository customerRepository,
         ICableSearchService cableSearchService, IUserService userService)
      {
         _labelProvider = labelProvider;
         _cableRepository = cableRepository;
         _companyRepository = companyRepository;
         _cablePriceDocumentRepository = cablePriceDocumentRepository;
         _customerRepository = customerRepository;
         _cableSearchService = cableSearchService;
         _userService = userService;
      }

      public Offer CreateOffer(string customerRequestFilePath, string customerId, string note, OfferType offerType)
      {
         var fileInfo = new FileInfo(customerRequestFilePath);
         Stream customerRequestFile = File.OpenRead(fileInfo.FullName);
         List<CableModel> cableNames = _cableRepository.GetAll();
         List<List<string>> searchCriteriaList = CreateSearchCriteria(cableNames);
         List<Cable> cables = _cableSearchService.GetCables(customerRequestFile, searchCriteriaList);
         List<string> cablePriceDocuments = _cablePriceDocumentRepository.GetAll().Select(x => x.Path).ToList();
         Dictionary<string, float> prices = LoadPrices(cablePriceDocuments);

         string extension = offerType == OfferType.Excel ? ".xlsx" : ".pdf";
         CustomerModel customer = _customerRepository.Get(customerId);
         string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
         var offerName = CreateOfferName(customer.Name, date, extension);
         var offerFullName = CreateFullName(offerName);
         CompanyModel company = _companyRepository.GetAll().FirstOrDefault();
         UserModel user = _userService.GetCurrentlyLoggedInUser();
         List<OfferItem> offerItems = CreateOfferItems(cables, customer);

         var offer = new Offer
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

      private Dictionary<string, float> LoadPrices(List<string> priceDocumentPaths)
      {
         var prices = new Dictionary<string, float>();

         foreach (string priceDocumentPath in priceDocumentPaths)
         {
            string extension = Path.GetExtension(priceDocumentPath);

            switch (extension)
            {
               case ".pdf":
                  var pricesPdf = LoadPdf(priceDocumentPath);
                  prices.AddRange(pricesPdf);
                  break;

               case ".xlsx":
                  var pricesExcel = LoadExcel(priceDocumentPath);
                  prices.AddRange(pricesExcel);
                  break;

               default:
                  throw new Exception("Document format not supported");
            }
         }

         return prices;
      }

      private Dictionary<string, float> LoadPdf(string priceDocumentPath)
      {
         List<string> allPagesContent = PdfDocumentHelper.GetPagesWithPrices(priceDocumentPath);
         var prices = new Dictionary<string, float>();
         string cableName = null;

         foreach (string page in allPagesContent)
         {
            string[] pageLines = page.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(x => x.TrimStart()).ToArray();

            pageLines = pageLines.Skip(2).Where(x => x.Length > 0).Reverse().Skip(1).Reverse().ToArray();

            foreach (string line in pageLines)
            {
               string[] lineItems = line.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);

               if (lineItems.Length == 1)
               {
                  cableName = lineItems.FirstOrDefault();
               }

               if (lineItems.Length >= 2)
               {
                  float price;

                  if (float.TryParse(lineItems[1], NumberStyles.Any, _commaCulture, out price))
                  {
                     string dimension = lineItems[0];
                     var key = cableName + " " + dimension;

                     prices.Add(key, price);
                  }
                  else
                  {
                     if (lineItems.Length <= 3)
                     {
                        cableName = lineItems[0] + " " + lineItems[1];
                     }
                  }
               }
            }
         }

         return prices;
      }

      private Dictionary<string, float> LoadExcel(string priceDocumentPath)
      {
         var prices = new Dictionary<string, float>();
         var fileInfo = new FileInfo(priceDocumentPath);

         using (var excelPackage = new ExcelPackage(fileInfo))
         {
            ExcelWorksheet priceWorksheet;

            try
            {
               priceWorksheet = excelPackage.Workbook.Worksheets["Lista"];
            }
            catch
            {
               priceWorksheet = excelPackage.Workbook.Worksheets["Lista"];
            }

            var start = priceWorksheet.Dimension.Start;
            var end = priceWorksheet.Dimension.End;

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
               string name = priceWorksheet.Cells[row, 2].Text;
               string priceRaw = priceWorksheet.Cells[row, 4].Text;
               float price;

               if (float.TryParse(priceRaw, NumberStyles.Any, _commaCulture, out price))
               {
                  prices.Add(name, price);
               }
            }
         }

         return prices;
      }

      #endregion
   }
}

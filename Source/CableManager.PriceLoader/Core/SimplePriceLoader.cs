using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using CableManager.PriceLoader.Helpers;
using CableManager.PriceLoader.Models;
using OfficeOpenXml;

namespace CableManager.PriceLoader.Core
{
   public class SimplePriceLoader : IPriceLoader
   {
      private readonly CultureInfo _commaCulture = new CultureInfo("en")
      {
         NumberFormat =
         {
            NumberDecimalSeparator = ","
         }
      };

      public List<PriceModel> LoadPricesFromPdf(string path, string documentId)
      {
         List<string> allPagesContent = PdfDocumentHelper.GetPagesWithPrices(path);
         var priceModels = new List<PriceModel>();

         List<PageLineItem> pageItems = GetPageItems(allPagesContent);

         foreach (PageLineItem pageItem in pageItems)
         {
            if (!pageItem.IsPrice)
            {
               if (pageItem.LineItems.FirstOrDefault() != "Singlemode")
               {
                  List<string> cableNames = pageItem.LineItems.Select(x => x.Trim()).ToList();
                  priceModels.Add(new PriceModel(documentId, cableNames));
               }
            }
            else
            {
               string name = GetName(pageItem.LineItems);
               float price = GetPrice(pageItem.LineItems);

               priceModels.Last().PriceItems.Add(new PriceItem(name, price));
            }
         }

         return priceModels;
      }

      public List<PriceModel> LoadPricesFromExcel(string path, string documentId)
      {
         var priceModels = new List<PriceModel>();

         List<PageLineItem> pageItems = GetPageItems(path);

         foreach (PageLineItem pageItem in pageItems)
         {
            if (!pageItem.IsPrice)
            {
               List<string> cableNames = pageItem.LineItems.Select(x => x.Trim()).ToList();
               priceModels.Add(new PriceModel(documentId, cableNames));
            }
            else
            {
               string name = GetName(pageItem.LineItems);
               float price = GetPrice(pageItem.LineItems);

               priceModels.Last().PriceItems.Add(new PriceItem(name, price));
            }
         }

         return priceModels;
      }

      #region Private methods

      private List<PageLineItem> GetPageItems(List<string> allPagesContent)
      {
         var priceItems = new List<PageLineItem>();

         foreach (string page in allPagesContent)
         {
            string[] pageLines = page.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
               .Select(x => x.TrimStart()).ToArray();

            pageLines = pageLines.Skip(2).Where(x => x.Length > 0).Reverse().Skip(1).Reverse().ToArray();

            foreach (string line in pageLines)
            {
               List<string> lineItems = GetLineItems(line);

               if (lineItems.FirstOrDefault() != "Dimenzije" && lineItems.FirstOrDefault() != "(mm2)")
               {
                  bool isLineWithPrices = IsLineWithPrices(lineItems);

                  priceItems.Add(new PageLineItem(isLineWithPrices, lineItems));
               }
            }
         }

         return priceItems;
      }

      private List<PageLineItem> GetPageItems(string path)
      {
         var priceItems = new List<PageLineItem>();
         var fileInfo = new FileInfo(path);

         using (var excelPackage = new ExcelPackage(fileInfo))
         {
            ExcelWorksheet priceWorksheet;

            try
            {
               priceWorksheet = excelPackage.Workbook.Worksheets["Cjenik"];
            }
            catch
            {
               priceWorksheet = excelPackage.Workbook.Worksheets["Cjenik"];
            }

            ExcelCellAddress start = priceWorksheet.Dimension.Start;
            ExcelCellAddress end = priceWorksheet.Dimension.End;

            for (int row = start.Row; row <= end.Row; row++)
            {
               List<string> lineItems = priceWorksheet.Cells[row, start.Column, row, 6].Where(x => x.Text != string.Empty).Select(x => x.Text).ToList();
               var lineItemsOutput = new List<string>();

               if (lineItems.Any() && !(lineItems.First().StartsWith("Konstrukcija") || lineItems.First().StartsWith("HRN")))
               {
                  bool isLineWithPrices = IsLineWithPrices(lineItems);

                  if (isLineWithPrices)
                  {
                     lineItemsOutput = lineItems.Where(x => x != "9").Skip(1).Take(2).ToList();
                  }
                  else
                  {
                     foreach (string lineItem in lineItems.Where(x => x != "0").Take(2))
                     {
                        lineItemsOutput.AddRange(lineItem.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList());
                     }
                  }

                  if (lineItemsOutput.Any())
                  {
                     priceItems.Add(new PageLineItem(isLineWithPrices, lineItemsOutput));
                  }
               }
            }
         }

         return priceItems;
      }

      private List<string> GetLineItems(string line)
      {
         var lineItems = new List<string>();

         foreach (string lineItem in line.Split(new[] {"  "}, StringSplitOptions.RemoveEmptyEntries))
         {
            foreach (string lineSubItem in lineItem.Split(new[] {" / "}, StringSplitOptions.RemoveEmptyEntries))
            {
               lineItems.Add(lineSubItem);
            }
         }

         return lineItems;
      }

      private string GetName(List<string> lineItems)
      {
         string name = lineItems.FirstOrDefault();

         foreach (string lineItem in lineItems.Skip(1))
         {
            float price;

            if (float.TryParse(lineItem, NumberStyles.Any, _commaCulture, out price))
            {
               break;
            }

            name = string.Join(" ", name, lineItem);
         }

         return name?.Replace(",", ".").Replace(" ", string.Empty);
      }

      private float GetPrice(List<string> lineItems)
      {
         float price = -1;

         foreach (string lineItem in lineItems.Skip(1))
         {
            if (float.TryParse(lineItem, NumberStyles.Any, _commaCulture, out price))
            {
               break;
            }
         }

         return price;
      }

      private bool IsLineWithPrices(List<string> lineItems)
      {
         foreach (string lineItem in lineItems)
         {
            float number;

            if (float.TryParse(lineItem, out number) && number > 0)
            {
               return true;
            }
         }

         return false;
      }

      #endregion
   }
}

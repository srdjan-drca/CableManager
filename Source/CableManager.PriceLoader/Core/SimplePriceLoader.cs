using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
         var fileInfo = new FileInfo(path);

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

            ExcelCellAddress start = priceWorksheet.Dimension.Start;
            ExcelCellAddress end = priceWorksheet.Dimension.End;
            string previousGroup = string.Empty;

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
               string nameOriginal = priceWorksheet.Cells[row, 2].Text;
               string group = priceWorksheet.Cells[row, 3].Text;
               float price = GetPrice(priceWorksheet.Cells[row, 4].Text);
               string name = nameOriginal.Replace(group, string.Empty).Replace(",", ".").Trim();

               if (previousGroup != group)
               {
                  var cableNames = new List<string> { group };
                  var priceModel = new PriceModel(documentId, cableNames);
                  priceModel.PriceItems.Add(new PriceItem(name, price));

                  priceModels.Add(priceModel);
                  previousGroup = group;
               }
               else
               {
                  priceModels.Last().PriceItems.Add(new PriceItem(name, price));
               }
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
               string[] lineItems = GetLineItems(line);

               bool isLineWithPrices = IsLineWithPrices(lineItems);

               if (lineItems.FirstOrDefault() != "Dimenzije" && lineItems.FirstOrDefault() != "(mm2)")
               {
                  priceItems.Add(new PageLineItem(isLineWithPrices, lineItems.ToList()));
               }
            }
         }

         return priceItems;
      }

      private string[] GetLineItems(string line)
      {
         var items = new List<string>();

         foreach (string lineItem in line.Split(new[] {"  "}, StringSplitOptions.RemoveEmptyEntries))
         {
            foreach (string lineSubItem in lineItem.Split(new[] {" / "}, StringSplitOptions.RemoveEmptyEntries))
            {
               items.Add(lineSubItem);
            }
         }

         return items.ToArray();
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

      private float GetPrice(string priceRaw)
      {
         float price;

         if (float.TryParse(priceRaw, NumberStyles.Any, _commaCulture, out price))
         {
            return price;
         }

         return -1;
      }

      private bool IsLineWithPrices(string[] lineItems)
      {
         foreach (string lineItem in lineItems)
         {
            float number;

            if (float.TryParse(lineItem, out number))
            {
               return true;
            }
         }

         return false;
      }

      #endregion
   }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CableManager.Services.Helpers;
using OfficeOpenXml;

namespace CableManager.Services.DocumentLoaders.Price
{
   public class PriceLoader : IPriceLoader
   {
      private readonly CultureInfo _commaCulture = new CultureInfo("en")
      {
         NumberFormat =
         {
            NumberDecimalSeparator = ","
         }
      };

      public Dictionary<string, double> LoadPdf(string path)
      {
         List<string> allPagesContent = PdfDocumentHelper.GetPagesWithPrices(path);
         var prices = new Dictionary<string, double>();
         string cableName = null;

         foreach (string page in allPagesContent)
         {
            string[] pageLines = page.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(x => x.TrimStart()).ToArray();

            pageLines = pageLines.Skip(2).Where(x => x.Length > 0).Reverse().Skip(1).Reverse().ToArray();

            foreach (string line in pageLines)
            {
               string[] lineItems = line.Split(new[] {"  "}, StringSplitOptions.RemoveEmptyEntries);

               if (lineItems.Length == 1)
               {
                  cableName = lineItems.FirstOrDefault();
               }

               if (lineItems.Length >= 2)
               {
                  double price;

                  if (double.TryParse(lineItems[1], NumberStyles.Any, _commaCulture, out price))
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

      public Dictionary<string, double> LoadExcel(string path)
      {
         var prices = new Dictionary<string, double>();
         var fileInfo = new FileInfo(path);

         using (var excelPackage = new ExcelPackage(fileInfo))
         {
            ExcelWorksheet priceWorksheet = excelPackage.Workbook.Worksheets["Lista"];
            var start = priceWorksheet.Dimension.Start;
            var end = priceWorksheet.Dimension.End;

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
               string name = priceWorksheet.Cells[row, 2].Text;
               string priceRaw = priceWorksheet.Cells[row, 4].Text;
               double price;

               if (double.TryParse(priceRaw, NumberStyles.Any, _commaCulture, out price))
               {
                  prices.Add(name, price);
               }
            }
         }

         return prices;
      }
   }
}

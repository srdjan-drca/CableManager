using System;
using System.IO;
using System.Collections.Generic;
using CableManager.Report.Extensions;
using CableManager.Services.Search.Iterator;
using CableManager.Services.Search.Model;
using OfficeOpenXml;

namespace CableManager.Services.Search
{
   public class CableSearchService : ICableSearchService
   {
      public List<CableDetails> GetCables(Stream customerRequestFile, List<List<string>> searchCriteriaList)
      {
         var cableDetails = new List<CableDetails>();

         using (var excelPackage = new ExcelPackage(customerRequestFile))
         {
            var cellIterator = new CellIterator(excelPackage);

            foreach (CellItem cellItem in cellIterator)
            {
               if (string.IsNullOrEmpty(cellItem.Text))
               {
                  continue;
               }

               int columnIdStart = cellItem.Column;
               int columnIdEnd = excelPackage.Workbook.Worksheets[cellItem.Worksheet].Dimension.End.Column;
               bool isFound = false;

               foreach (List<string> searchCriteria in searchCriteriaList)
               {
                  foreach (string cableName in searchCriteria)
                  {
                     string cableName2 = CableNamePatch(cableName);

                     if (cellItem.Text.Contains(cableName2, StringComparison.OrdinalIgnoreCase))
                     {
                        var worksheet = excelPackage.Workbook.Worksheets[cellItem.Worksheet];
                        string cableUnit = FindCableUnit(worksheet, cellItem.Row, columnIdStart, columnIdEnd);

                        if (cableUnit != null)
                        {
                           float quantity = FindCableQuantity(worksheet, cellItem.Row, columnIdStart, columnIdEnd);

                           cableDetails.Add(new CableDetails(cellItem.Text.Trim(), quantity, searchCriteria));
                           isFound = true;
                           break;
                        }
                     }
                  }

                  if (isFound)
                     break;
               }
            }
         }

         return cableDetails;
      }

      private string CableNamePatch(string cableName)
      {
         string cableName2;
         cableName2 = cableName;
         if (cableName == "P")
         {
            cableName2 = " P ";
         }
         else if (cableName == "PL")
         {
            cableName2 = " PL ";
         }
         else if (cableName == "LAN")
         {
            cableName2 = " LAN ";
         }
         else if (cableName == "PPR")
         {
            cableName2 = " PPR ";
         }
         else if (cableName == "YM")
         {
            cableName2 = " YM ";
         }
         else if (cableName == "PF")
         {
            cableName2 = " PF ";
         }
         else if (cableName == "PP00")
         {
            cableName2 = " PPOO ";
         }

         return cableName2;
      }

      private string FindCableUnit(ExcelWorksheet worksheet, int rowId, int columnIdStart, int columnIdEnd)
      {
         for (int columnId = columnIdStart; columnId <= columnIdEnd; columnId++)
         {
            string cellValue = worksheet.Cells[rowId, columnId].Text;

            if (cellValue.Equals("m", StringComparison.OrdinalIgnoreCase) && cellValue.Length == 1 ||
                cellValue.Equals("m'", StringComparison.OrdinalIgnoreCase) && cellValue.Length == 2)
            {
               return "m";
            }
         }

         return null;
      }

      private float FindCableQuantity(ExcelWorksheet worksheet, int rowId, int columnIdStart, int columnIdEnd)
      {
         float quantity = -1;

         for (int columnId = columnIdStart + 1; columnId <= columnIdEnd; columnId++)
         {
            string cellValue = worksheet.Cells[rowId, columnId].Text;

            if (float.TryParse(cellValue, out quantity))
            {
               return quantity;
            }
         }

         return quantity;
      }
   }
}

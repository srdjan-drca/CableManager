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
      public List<Cable> GetCables(Stream customerRequestFile, List<List<string>> searchCriteriaList)
      {
         var cableDetails = new List<Cable>();

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

               foreach (List<string> searchCriteria in searchCriteriaList)
               {
                  foreach (string cableName in searchCriteria)
                  {
                     if (cellItem.Text.Contains(cableName, StringComparison.OrdinalIgnoreCase))
                     {
                        var worksheet = excelPackage.Workbook.Worksheets[cellItem.Worksheet];
                        string cableUnit = FindCableUnit(worksheet, cellItem.Row, columnIdStart, columnIdEnd);

                        if (cableUnit == "m")
                        {
                           float quantity = FindCableQuantity(worksheet, cellItem.Row, columnIdStart, columnIdEnd);

                           cableDetails.Add(new Cable(cellItem.Text, quantity, 1, searchCriteria));
                        }
                     }
                  }
               }
            }
         }

         return cableDetails;
      }

      private string FindCableUnit(ExcelWorksheet worksheet, int rowId, int columnIdStart, int columnIdEnd)
      {
         for (int columnId = columnIdStart; columnId <= columnIdEnd; columnId++)
         {
            string cellValue = worksheet.Cells[rowId, columnId].Text;

            if (cellValue.Equals("m", StringComparison.OrdinalIgnoreCase) && cellValue.Length == 1)
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

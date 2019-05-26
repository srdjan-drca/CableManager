using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml;

namespace CableManager.Services.Search.Iterator
{
   public class CellIterator : IEnumerable<CellItem>
   {
      private readonly List<CellItem> _cellItems;

      public CellIterator(ExcelPackage excelPackage)
      {
         _cellItems = new List<CellItem>();

         foreach (ExcelWorksheet excelWorksheet in excelPackage.Workbook.Worksheets)
         {
            var start = excelWorksheet.Dimension.Start;
            var end = excelWorksheet.Dimension.End;

            for (int row = start.Row; row <= end.Row; row++)
            {
               for (int column = start.Column; column <= end.Column; column++)
               {
                  string cellValue = excelWorksheet.Cells[row, column].Text;

                  _cellItems.Add(new CellItem(excelWorksheet.Index, row, column, cellValue));
               }
            }
         }
      }

      public IEnumerator<CellItem> GetEnumerator()
      {
         foreach (CellItem cellItem in _cellItems)
         {
            yield return cellItem;
         }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}

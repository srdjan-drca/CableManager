using System.IO;
using System.Linq;
using System.Collections.Generic;
using OfficeOpenXml;
using CableManager.Repository.Cable;
using CableManager.Repository.Models;

namespace CableManager.Services.DocumentLoaders.CableName
{
   public class CableNameLoader : ICableNameLoader
   {
      private readonly ICableRepository _cableRepository;

      public CableNameLoader(ICableRepository cableRepository)
      {
         _cableRepository = cableRepository;
      }

      public List<CableModel> Load(string fileName)
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
   }
}

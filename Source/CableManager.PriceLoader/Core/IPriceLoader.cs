using System.Collections.Generic;
using CableManager.PriceLoader.Models;

namespace CableManager.PriceLoader.Core
{
   public interface IPriceLoader
   {
      List<PriceModel> LoadPricesFromPdf(string path, string documentId);

      List<PriceModel> LoadPricesFromExcel(string path, string documentId);
   }
}

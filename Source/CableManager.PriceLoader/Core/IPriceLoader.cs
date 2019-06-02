using System.Collections.Generic;
using CableManager.PriceLoader.Models;

namespace CableManager.PriceLoader.Core
{
   public interface IPriceLoader
   {
      List<CablePriceModel> LoadPricesFromPdf(string path, string documentId);

      List<CablePriceModel> LoadPricesFromExcel(string path, string documentId);
   }
}

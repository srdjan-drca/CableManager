using System.Collections.Generic;

namespace CableManager.Services.DocumentLoaders.Price
{
   public interface IPriceLoader
   {
      Dictionary<string, double> LoadPdf(string path);

      Dictionary<string, double> LoadExcel(string path);
   }
}

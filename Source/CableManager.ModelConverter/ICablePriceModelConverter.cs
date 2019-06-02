using System.Collections.Generic;
using CableManager.PriceLoader.Models;
using CableManager.Repository.Models;

namespace CableManager.ModelConverter
{
   public interface ICablePriceModelConverter
   {
      List<CablePriceDbModel> ToDbModels(List<CablePriceModel> cablePriceModels);

      List<CablePriceModel> ToModels(List<CablePriceDbModel> cablePriceDbModels);
   }
}

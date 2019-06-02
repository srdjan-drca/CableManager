using CableManager.PriceLoader.Models;
using CableManager.Repository.Models;
using System.Collections.Generic;
using System.Linq;

namespace CableManager.ModelConverter
{
   public class CablePriceModelConverter : ICablePriceModelConverter
   {
      public List<CablePriceDbModel> ToDbModels(List<CablePriceModel> cablePriceModels)
      {
         List<CablePriceDbModel> cablePriceDbModels = cablePriceModels.Select(
            x => new CablePriceDbModel(x.DocumentGuid, x.CableNames, ConvertToCableNamePriceDbModels(x.PricesByType))).ToList();

         return cablePriceDbModels;
      }

      public List<CablePriceModel> ToModels(List<CablePriceDbModel> cablePriceDbModels)
      {
         List<CablePriceModel> cablePriceModels = cablePriceDbModels.Select(
            x => new CablePriceModel(x.DocumentId, x.CableNames, ConvertToCableNamePriceModels(x.PriceItems))).ToList();

         return cablePriceModels;
      }

      private List<CableNamePriceDbModel> ConvertToCableNamePriceDbModels(List<TypePriceModel> priceItems)
      {
         List<CableNamePriceDbModel> cableNamePriceDbModels = priceItems.Select(
            x => new CableNamePriceDbModel(x.Type, x.Price)).ToList();

         return cableNamePriceDbModels;
      }

      private List<TypePriceModel> ConvertToCableNamePriceModels(List<CableNamePriceDbModel> cableNamePriceDbModels)
      {
         List<TypePriceModel> priceItems = cableNamePriceDbModels.Select(x => new TypePriceModel(x.Name, x.Price)).ToList();

         return priceItems;
      }
   }
}

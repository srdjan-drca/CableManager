using System.Linq;
using System.Collections.Generic;

namespace CableManager.PriceLoader.Models
{
   public class CablePriceModel
   {
      public string DocumentGuid { get; set; }

      public List<string> CableNames { get; set; }

      public List<TypePriceModel> PricesByType { get; set; }

      public CablePriceModel(string documentGuid, List<string> cableNames)
      {
         DocumentGuid = documentGuid;
         CableNames = cableNames;
         PricesByType = new List<TypePriceModel>();
      }

      public CablePriceModel(string documentGuid, List<string> cableNames, List<TypePriceModel> pricesByType)
      {
         DocumentGuid = documentGuid;
         CableNames = cableNames;
         PricesByType = pricesByType;
      }

      public bool GetPrice(List<string> cableNames, string cableType, out float price)
      {
         price = 0;

         bool isSuccess = IsCableNameFound(cableNames) && IsCableTypeCorrect(cableType);

         if (isSuccess)
         {
            isSuccess = GetPrice(cableType, out price);

            if (!isSuccess)
            {
               cableType = cableType.Substring(cableType.LastIndexOf("x") + 1);

               isSuccess = GetPrice(cableType, out price);
            }
         }

         return isSuccess;
      }

      private bool IsCableNameFound(List<string> cableNames)
      {
         bool isCableNameFound = CableNames.Any(cableName => cableNames.Contains(cableName));

         if (isCableNameFound)
         {
            return true;
         }

         return false;
      }

      private bool IsCableTypeCorrect(string cableType)
      {
         return !string.IsNullOrEmpty(cableType);
      }

      private bool GetPrice(string cableType, out float price)
      {
         price = 0;

         foreach (TypePriceModel priceByType in PricesByType)
         {
            if (priceByType.Type.Contains(cableType))
            {
               price = priceByType.Price;

               return true;
            }
         }

         return false;
      }
   }
}

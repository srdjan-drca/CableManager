using System.Collections.Generic;

namespace CableManager.Repository.Models
{
   public class CablePriceDbModel
   {
      public string Id { get; set; }

      public string DocumentId { get; set; }

      public List<string> CableNames { get; set; }

      public List<CableNamePriceDbModel> PriceItems { get; set; }

      public CablePriceDbModel(string documentId, List<string> cableNames, List<CableNamePriceDbModel> priceItems)
      {
         DocumentId = documentId;
         CableNames = cableNames;
         PriceItems = priceItems;
      }
   }
}

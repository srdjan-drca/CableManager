using System.Collections.Generic;

namespace CableManager.PriceLoader.Models
{
   public class PriceModel
   {
      public string DocumentGuid { get; set; }

      public List<string> CableNames { get; set; }

      public List<PriceItem> PriceItems { get; set; }

      public PriceModel(string documentGuid, List<string> cableNames)
      {
         DocumentGuid = documentGuid;
         CableNames = cableNames;
         PriceItems = new List<PriceItem>();
      }
   }
}

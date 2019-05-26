using System.Collections.Generic;

namespace CableManager.PriceLoader.Models
{
   public struct PageLineItem
   {
      public bool IsPrice { get; set; }

      public List<string> LineItems { get; set; }

      public PageLineItem(bool isPrice, List<string> lineItems)
      {
         IsPrice = isPrice;
         LineItems = lineItems;
      }
   }
}

using System.Collections.Generic;

namespace CableManager.Services.Offer.Models
{
   public class OfferParameters
   {
      public OfferType OfferType { get; set; }

      public HashSet<string> PriceDocumentIds { get; set; }

      public string CustomerRequestFilePath { get; set; }

      public string CustomerId { get; set; }

      public string Note { get; set; }
   }
}

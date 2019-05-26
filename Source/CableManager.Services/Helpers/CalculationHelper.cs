using System.Linq;
using System.Collections.Generic;
using CableManager.Report.Models;

namespace CableManager.Services.Helpers
{
   public static class CalculationHelper
   {
      public static float CalculatePercent(float number, int percent)
      {
         return ((float)number * percent) / 100;
      }

      public static float CalculateTotalPrice(List<OfferItem> offerItems)
      {
         return offerItems.Sum(x => x.TotalPrice);
      }

      public static float CalculateTotalPriceWithVat(List<OfferItem> offerItems)
      {
         return offerItems.Sum(x => x.TotalPriceWithVat);
      }

      public static float CalculateRebate(List<OfferItem> offerItems)
      {
         return 0;
      }
   }
}

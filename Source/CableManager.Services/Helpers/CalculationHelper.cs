using System.Linq;
using System.Collections.Generic;
using CableManager.Report.Models;

namespace CableManager.Services.Helpers
{
   public static class CalculationHelper
   {
      public static float CalculatePrice(float price, float rebate, float quantity)
      {
         float rebateQuota = ((100 - rebate) / 100);

         return price * rebateQuota * quantity;
      }

      public static float CalculatePriceWithVat(float price, float valueAddedTax)
      {
         float vatQuota = ((100 + valueAddedTax) / 100);

         return price * vatQuota;
      }

      public static float CalculateTotalPrice(List<OfferItem> offerItems)
      {
         return offerItems.Sum(x => x.TotalPrice);
      }

      public static float CalculateTotalPriceWithRebate(List<OfferItem> offerItems)
      {
         return offerItems.Sum(x => x.TotalPriceWithRebate);
      }

      public static float CalculateTotalPriceWithVat(List<OfferItem> offerItems)
      {
         return offerItems.Sum(x => x.TotalPriceWithVat);
      }
   }
}

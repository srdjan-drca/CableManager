namespace CableManager.Report.Models
{
   public class OfferItem
   {
      public int SerialNumber { get; set; }

      public string Name { get; set; }

      public float Quantity { get; set; }

      public string Unit { get; set; }

      public float PricePerItem { get; set; }

      public float Rebate { get; set; }

      public float ValueAddedTax { get; set; }

      public float TotalPrice { get; set; }

      public float TotalPriceWithVat { get; set; }
   }
}
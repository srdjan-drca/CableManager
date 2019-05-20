namespace CableManager.Report.Models
{
   public class CableDetails
   {
      public int SerialNumber { get; set; }

      public string Name { get; set; }

      public int Quantity { get; set; }

      public string Unit { get; set; }

      public float PricePerUnit { get; set; }

      public float Rebate { get; set; }

      public float Vat { get; set; }

      public float TotalPrice { get; set; }

      public float TotalPriceWithVat { get; set; }
   }
}
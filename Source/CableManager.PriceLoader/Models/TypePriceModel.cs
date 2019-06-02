namespace CableManager.PriceLoader.Models
{
   public struct TypePriceModel
   {
      public string Type { get; set; }

      public float Price { get; set; }

      public TypePriceModel(string type, float price)
      {
         Type = type;
         Price = price;
      }
   }
}

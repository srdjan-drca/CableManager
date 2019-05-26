namespace CableManager.PriceLoader.Models
{
   public struct PriceItem
   {
      public string Name { get; set; }

      public float Price { get; set; }

      public PriceItem(string name, float price)
      {
         Name = name;
         Price = price;
      }
   }
}

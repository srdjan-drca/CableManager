using System.Globalization;

namespace CableManager.Repository.Models
{
   public struct CableNamePriceDbModel
   {
      public string Name { get; set; }

      public float Price { get; set; }

      public CableNamePriceDbModel(string name, float price)
      {
         Name = name;
         Price = price;
      }

      public CableNamePriceDbModel(string name, string price)
      {
         var dotCulture = new CultureInfo("en")
         {
            NumberFormat =
            {
               NumberDecimalSeparator = "."
            }
         };
         float priceAsNumber;

         Name = name;
         Price = float.TryParse(price, NumberStyles.Any, dotCulture, out priceAsNumber) ? priceAsNumber : 0;
      }
   }
}

using System.Collections.Generic;

namespace CableManager.Services.Search.Model
{
   public class Cable
   {
      public string Name { get; set; }

      public float Quantity { get; set; }

      public float Price { get; set; }

      public List<string> SearchCriteria { get; set; }

      public Cable(string name, float quantity, float price, List<string> searchCriteria)
      {
         Name = name;
         Quantity = quantity;
         Price = price;
         SearchCriteria = searchCriteria;
      }
   }
}

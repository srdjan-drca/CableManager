using System.Collections.Generic;

namespace CableManager.Services.Search.Model
{
   public class CableDetails
   {
      public string Name { get; set; }

      public float Quantity { get; set; }

      public List<string> SearchCriteria { get; set; }

      public float Price { get; set; }

      public CableDetails(string name, float quantity, List<string> searchCriteria)
      {
         Name = name;
         Quantity = quantity;
         SearchCriteria = searchCriteria;
      }
   }
}

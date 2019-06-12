using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CableManager.Services.Search.Model
{
   public class CableDetails
   {
      public string Name { get; set; }

      public float Quantity { get; set; }

      public List<string> SearchCriteria { get; set; }

      public float Price { get; set; }

      public string CableType
      {
         get
         {
            string cableTypeRaw = GetCableTypeRaw();
            string cableType = GetCableType(cableTypeRaw);

            return cableType;
         }
      }

      public CableDetails(string name, float quantity, List<string> searchCriteria)
      {
         Name = name;
         Quantity = quantity;
         SearchCriteria = searchCriteria;
      }

      private string GetCableTypeRaw()
      {
         string cableTypeRaw = string.Empty;

         foreach (string searchCriterion in SearchCriteria)
         {
            int index = Name.ToLower().IndexOf(searchCriterion.ToLower());

            if (index != -1)
            {
               index += searchCriterion.Length;
               cableTypeRaw = Name.Substring(index);
               break;
            }
         }

         return cableTypeRaw;
      }

      private string GetCableType(string cableTypeRaw)
      {
         Regex pattern = new Regex(@"[\d][x][\d][x][\d][,.]?[\d]?");
         Match match = pattern.Match(cableTypeRaw);
         string whatYouAreLookingFor = match.Groups[0].Value;

         if (string.IsNullOrEmpty(whatYouAreLookingFor))
         {
            pattern = new Regex(@"[\d][x][\d][,.]?[\d]?");
            match = pattern.Match(cableTypeRaw);
            whatYouAreLookingFor = match.Groups[0].Value;
         }

         if (string.IsNullOrEmpty(whatYouAreLookingFor))
         {
            pattern = new Regex(@"[\d]+");
            match = pattern.Match(cableTypeRaw);
            whatYouAreLookingFor = match.Groups[0].Value;
         }

         return whatYouAreLookingFor.Replace(",", ".").Replace(" ", string.Empty);
      }
   }
}

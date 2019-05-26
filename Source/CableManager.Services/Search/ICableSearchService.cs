using System.IO;
using System.Collections.Generic;
using CableManager.Services.Search.Model;

namespace CableManager.Services.Search
{
   public interface ICableSearchService
   {
      List<Cable> GetCables(Stream customerRequestFile, List<List<string>> searchCriteriaList);
   }
}

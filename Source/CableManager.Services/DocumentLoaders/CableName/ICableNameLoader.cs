using System.Collections.Generic;
using CableManager.Repository.Models;

namespace CableManager.Services.DocumentLoaders.CableName
{
   public interface ICableNameLoader
   {
      List<CableModel> Load(string fileName);
   }
}

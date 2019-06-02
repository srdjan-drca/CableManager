using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Repository.Models;

namespace CableManager.Repository.CablePrice
{
   public interface ICablePriceRepository
   {
      ReturnResult Save(CablePriceDbModel cablePrice);

      ReturnResult SaveAll(List<CablePriceDbModel> cables);

      List<CablePriceDbModel> GetAll();

      ReturnResult DeleteAll();
   }
}

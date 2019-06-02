using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Repository.Models;

namespace CableManager.Repository.CablePriceDocument
{
   public interface ICablePriceDocumentRepository
   {
      ReturnResult Save(PriceDocumentModel priceDocument);

      ReturnResult DeleteAll();

      List<PriceDocumentModel> GetAll();
   }
}
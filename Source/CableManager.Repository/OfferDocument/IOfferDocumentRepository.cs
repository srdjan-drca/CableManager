using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Repository.Models;

namespace CableManager.Repository.OfferDocument
{
   public interface IOfferDocumentRepository
   {
      ReturnResult Save(OfferDocumentModel offerDocument);

      List<OfferDocumentModel> GetAll();

      ReturnResult DeleteAll(List<string> offerIds);
   }
}
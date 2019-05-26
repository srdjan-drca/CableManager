using System.Collections.Generic;
using CableManager.Repository.Models;
using CableManager.Services.Offer.Models;

namespace CableManager.Services.Offer
{
   public interface IOfferService
   {
      OfferModel CreateOffer(string customerRequestFilePath, string customerId, string note, OfferType offerType);

      List<CableModel> LoadCableNames(string fileName);
   }
}

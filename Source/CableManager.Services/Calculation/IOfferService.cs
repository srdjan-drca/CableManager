using System.Collections.Generic;
using CableManager.Repository.Models;
using CableManager.Services.Calculation.Models;

namespace CableManager.Services.Calculation
{
   public interface IOfferService
   {
      Offer CreateOffer(string customerRequestFilePath, string customerId, string note, OfferType offerType);

      List<CableModel> LoadCableNames(string fileName);
   }
}

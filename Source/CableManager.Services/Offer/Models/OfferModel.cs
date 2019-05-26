using System.Collections.Generic;
using CableManager.Report.Models;
using CableManager.Repository.Models;

namespace CableManager.Services.Offer.Models
{
   public class OfferModel
   {
      public string Name { get; set; }

      public string FullName { get; set; }

      public string Note { get; set; }

      public string Date { get; set; }

      public string Language { get; set; }

      public CustomerModel Customer { get; set; }

      public CompanyModel Company { get; set; }

      public UserModel User { get; set; }

      public List<OfferItem> Items { get; set; }

      public OfferTotal Total { get; set; }
   }
}

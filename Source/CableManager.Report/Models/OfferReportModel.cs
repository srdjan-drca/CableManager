using System.Collections.Generic;

namespace CableManager.Report.Models
{
   public class OfferReportModel : BaseReportModel
   {
      public string Id { get; set; }

      public string UserNumberAndName { get; set; }

      public string Note { get; set; }

      public CustomerDetails CustomerDetails { get; set; }

      public CompanyDetails CompanyDetails { get; set; }

      public List<CableDetails> Cables { get; set; }
   }
}
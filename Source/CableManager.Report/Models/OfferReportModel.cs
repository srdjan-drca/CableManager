using System.Collections.Generic;

namespace CableManager.Report.Models
{
   public class OfferReportModel : BaseReportModel
   {
      public string Id { get; set; }

      public string UserNumberAndName { get; set; }

      public string Note { get; set; }

      public CustomerModelPdf CustomerModelPdf { get; set; }

      public CompanyModelPdf CompanyModelPdf { get; set; }

      public List<OfferItem> OfferItems { get; set; }

      public OfferTotal OfferTotal { get; set; }
   }
}
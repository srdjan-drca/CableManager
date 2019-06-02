using System;
using System.Collections.Generic;

namespace CableManager.Report.Models
{
   public class OfferReportModel : BaseReportModel, ICloneable
   {
      public string Id { get; set; }

      public string UserNumberAndName { get; set; }

      public string Note { get; set; }

      public CustomerModelPdf CustomerModelPdf { get; set; }

      public CompanyModelPdf CompanyModelPdf { get; set; }

      public List<OfferItem> OfferItems { get; set; }

      public OfferTotal OfferTotal { get; set; }

      public OfferReportModel()
      {
      }

      public OfferReportModel(OfferReportModel clone)
      {
         Id = clone.Id;
         UserNumberAndName = clone.UserNumberAndName;
         Note = clone.Note;
         CustomerModelPdf = clone.CustomerModelPdf;
         CompanyModelPdf = clone.CompanyModelPdf;
         OfferItems = new List<OfferItem>();
         OfferTotal = clone.OfferTotal;
         LabelProvider = clone.LabelProvider;
         TimeDate = clone.TimeDate;
      }

      public object Clone()
      {
         return new OfferReportModel(this);
      }
   }
}
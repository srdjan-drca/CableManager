using System;
using System.Collections.Generic;
using System.Linq;
using CableManager.Common.Extensions;
using CableManager.Report.Models;
using CableManager.Report.Generators.Pdf.Sections.Cable;

namespace CableManager.Report.Generators.Pdf.Documents
{
   public class CableOfferPdfDocument : BasePdfDocument
   {
      public CableOfferPdfDocument(BaseReportModel baseReportModel) : base(baseReportModel)
      {
      }

      protected override void CreateSections()
      {
         var offerReportModel = (OfferReportModel)BaseReportModel;
         List<List<OfferItem>> partitionedOfferItems = CreatePartitionedOfferItems(offerReportModel.OfferItems);
         int totalPages = partitionedOfferItems.Count;
         CableFirstPdfSection firstSection = CreateFirstSection(offerReportModel, partitionedOfferItems.First(), totalPages);
         List<CableOtherPdfSection> otherSections = CreateOtherSections(offerReportModel, partitionedOfferItems.Skip(1).ToList());

         Sections.Add(firstSection);
         Sections.AddRange(otherSections);
      }

      private List<List<OfferItem>> CreatePartitionedOfferItems(List<OfferItem> offerItems)
      {
         var items = new List<List<OfferItem>>
         {
            offerItems.Take(35).ToList()
         };

         if (offerItems.Count > 35)
         {
            List<OfferItem> offerItemsOther = offerItems.Skip(35).ToList();

            foreach (List<OfferItem> offerItemsChunk in offerItemsOther.BatchBy(35))
            {
               items.Add(offerItemsChunk);
            }
         }

         return items;
      }

      private CableFirstPdfSection CreateFirstSection(OfferReportModel offerReportModel, List<OfferItem> offerItems, int totalPages)
      {
         var offerReportModelClone = offerReportModel.Clone() as OfferReportModel;
         CableFirstPdfSection section;

         if (offerReportModelClone != null)
         {
            offerReportModelClone.OfferItems.AddRange(offerItems);
            offerReportModelClone.PageNumber = 1;
            offerReportModelClone.PageTotal = totalPages;
            offerReportModelClone.DisplayTotals = totalPages <= 1;

            section = new CableFirstPdfSection(offerReportModelClone);
         }
         else
         {
            throw new Exception("Object cannot be cloned!");
         }

         return section;
      }

      private List<CableOtherPdfSection> CreateOtherSections(OfferReportModel offerReportModel, List<List<OfferItem>> partitionedOfferItems)
      {
         var otherSections = new List<CableOtherPdfSection>();
         int pageCounter = 2;

         foreach (List<OfferItem> offerItems in partitionedOfferItems)
         {
            var offerReportModelClone = offerReportModel.Clone() as OfferReportModel;

            if (offerReportModelClone?.OfferItems != null)
            {
               offerReportModelClone.OfferItems.AddRange(offerItems);
               offerReportModelClone.PageNumber = pageCounter;
               offerReportModelClone.PageTotal = partitionedOfferItems.Count + 1;

               otherSections.Add(new CableOtherPdfSection(offerReportModelClone));

               pageCounter++;
            }
            else
            {
               throw new Exception("Object cannot be cloned!");
            }
         }

         return otherSections;
      }
   }
}
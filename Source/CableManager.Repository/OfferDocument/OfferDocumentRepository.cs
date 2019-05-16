using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using CableManager.Common.Helpers;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Models;

namespace CableManager.Repository.OfferDocument
{
   public class OfferDocumentRepository : RepositoryBase, IOfferDocumentRepository
   {
      private readonly string _repositoryFileName;

      private readonly XDocument _offerDocumentsXDocument;

      public OfferDocumentRepository(LabelProvider labelProvider) : base(labelProvider)
      {
         _repositoryFileName = new FileInfo(DirectoryHelper.GetApplicationStoragePath() + "/Repository/OfferDocuments.xml").FullName;

         _offerDocumentsXDocument = GetXDocument(_repositoryFileName);
      }

      protected override string RootNodeName => "OfferDocuments";

      public ReturnResult Save(OfferDocumentModel offerDocument)
      {
         bool isNewRecord = string.IsNullOrEmpty(offerDocument.Id);
         ReturnResult result;

         try
         {
            if (isNewRecord)
            {
               offerDocument.Id = Guid.NewGuid().ToString();

               XElement offerDocumentElement = ConvertToDatabaseModel(offerDocument);

               _offerDocumentsXDocument?.Root?.Add(offerDocumentElement);
            }

            _offerDocumentsXDocument?.Save(_repositoryFileName);

            result = new SuccessResult(LabelProvider["UI_OfferDocumentSaved"]);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      public List<OfferDocumentModel> GetAll()
      {
         List<XElement> offerDocumentElements = _offerDocumentsXDocument.Descendants("OfferDocument").ToList();
         List<OfferDocumentModel> offerDocuments = new List<OfferDocumentModel>();

         foreach (XElement offerDocumentElement in offerDocumentElements)
         {
            OfferDocumentModel offerDocument = ConvertFromDatabaseModel(offerDocumentElement);

            offerDocuments.Add(offerDocument);
         }

         return offerDocuments;
      }

      public ReturnResult DeleteAll(List<string> offerIds)
      {
         if (offerIds == null || !offerIds.Any())
         {
            return new FailResult(LabelProvider["UI_OfferDocumentNotSelected"]);
         }

         IEnumerable<XElement> allElements = _offerDocumentsXDocument.Descendants("OfferDocument");
         List<XElement> elementsToDelete = allElements.Where(x => offerIds.Contains(x.Element("Id")?.Value)).ToList();
         ReturnResult result;

         try
         {
            foreach (XElement element in elementsToDelete)
            {
               element?.Remove();
            }

            _offerDocumentsXDocument?.Save(_repositoryFileName);

            string message = LabelProvider["UI_NumberOfOffersDeleted"] + " " + elementsToDelete.Count;

            result = new SuccessResult(message);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      #region Private methods

      private XElement ConvertToDatabaseModel(OfferDocumentModel offerDocument)
      {
         var offerDocumentElement = new XElement("OfferDocument",
            new XElement("Id", offerDocument.Id),
            new XElement("Name", offerDocument.Name),
            new XElement("Path", offerDocument.Path),
            new XElement("Date", offerDocument.Date));

         return offerDocumentElement;
      }

      private OfferDocumentModel ConvertFromDatabaseModel(XElement offerDocumentElement)
      {
         var offerDocument = new OfferDocumentModel(
            offerDocumentElement.Element("Id")?.Value,
            offerDocumentElement.Element("Name")?.Value,
            offerDocumentElement.Element("Path")?.Value,
            offerDocumentElement.Element("Date")?.Value
         );

         return offerDocument;
      }

      #endregion Private methods
   }
}
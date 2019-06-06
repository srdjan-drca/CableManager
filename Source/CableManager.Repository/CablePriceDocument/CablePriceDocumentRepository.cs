using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CableManager.Common.Extensions;
using CableManager.Common.Helpers;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Models;

namespace CableManager.Repository.CablePriceDocument
{
   public class CablePriceDocumentRepository : RepositoryBase, ICablePriceDocumentRepository
   {
      private readonly string _repositoryFileName;

      private readonly XDocument _priceDocumentsXDocument;

      public CablePriceDocumentRepository(LabelProvider labelProvider) : base(labelProvider)
      {
         _repositoryFileName = new FileInfo(DirectoryHelper.GetApplicationStoragePath() + "/Repository/CablePriceDocuments.xml").FullName;

         _priceDocumentsXDocument = GetXDocument(_repositoryFileName);
      }

      protected override string RootNodeName => "PriceDocuments";

      public ReturnResult Save(PriceDocumentModel priceDocument)
      {
         bool isNewRecord = string.IsNullOrEmpty(priceDocument.Id);
         ReturnResult result;

         try
         {
            if (isNewRecord)
            {
               priceDocument.Id = Guid.NewGuid().ToString();

               XElement priceDocumentElement = ConvertToDatabaseModel(priceDocument);

               _priceDocumentsXDocument?.Root?.Add(priceDocumentElement);
            }
            else
            {
               XElement priceDocumentFound = _priceDocumentsXDocument?.Find(priceDocument.Id);

               if (priceDocumentFound != null)
               {
                  UpdateDatabaseModel(priceDocumentFound, priceDocument);
               }
            }

            _priceDocumentsXDocument?.Save(_repositoryFileName);

            result = new SuccessResult(LabelProvider["UI_PriceDocumentsAdded"]);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      public ReturnResult DeleteAll()
      {
         _priceDocumentsXDocument?.Root?.RemoveAll();

         return new SuccessResult();
      }

      public List<PriceDocumentModel> GetAll()
      {
         List<XElement> priceDocumentElements = _priceDocumentsXDocument.Descendants("PriceDocument").ToList();
         List<PriceDocumentModel> priceDocuments = new List<PriceDocumentModel>();

         foreach (XElement priceDocumentElement in priceDocumentElements)
         {
            PriceDocumentModel priceDocument = ConvertToModel(priceDocumentElement);

            priceDocuments.Add(priceDocument);
         }

         return priceDocuments;
      }

      #region Private methods

      private XElement ConvertToDatabaseModel(PriceDocumentModel priceDocument)
      {
         XElement priceDocumentElement = new XElement("PriceDocument",
            new XElement("Id", priceDocument.Id),
            new XElement("Name", priceDocument.Name),
            new XElement("Path", priceDocument.Path),
            new XElement("Date", priceDocument.Date),
            new XElement("IsSelected", priceDocument.IsSelected));

         return priceDocumentElement;
      }

      private void UpdateDatabaseModel(XElement priceDocumentElement, PriceDocumentModel priceDocument)
      {
         priceDocumentElement.Element("Name")?.SetValue(priceDocument.Name ?? string.Empty);
         priceDocumentElement.Element("Path")?.SetValue(priceDocument.Path ?? string.Empty);
         priceDocumentElement.Element("Date")?.SetValue(priceDocument.Date ?? string.Empty);
         priceDocumentElement.Element("IsSelected")?.SetValue(priceDocument.IsSelected);
      }

      private PriceDocumentModel ConvertToModel(XElement priceDocumentElement)
      {
         bool isSelected;

         bool.TryParse(priceDocumentElement.Element("IsSelected")?.Value, out isSelected);

         var priceDocument = new PriceDocumentModel(
            priceDocumentElement.Element("Id")?.Value,
            priceDocumentElement.Element("Name")?.Value,
            priceDocumentElement.Element("Path")?.Value,
            priceDocumentElement.Element("Date")?.Value,
            isSelected);

         return priceDocument;
      }

      #endregion Private methods
   }
}
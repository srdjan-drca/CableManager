using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CableManager.Common.Helpers;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Models;

namespace CableManager.Repository.CablePrice
{
   public class CablePriceRepository : RepositoryBase, ICablePriceRepository
   {
      private readonly string _repositoryFileName;

      private readonly XDocument _cablePricesXDocument;

      public CablePriceRepository(LabelProvider labelProvider) : base(labelProvider)
      {
         _repositoryFileName = new FileInfo(DirectoryHelper.GetApplicationStoragePath() + "/Repository/CablePrices.xml").FullName;

         _cablePricesXDocument = GetXDocument(_repositoryFileName);
      }

      protected override string RootNodeName => "CablePrices";

      public ReturnResult Save(CablePriceDbModel cablePrice)
      {
         ReturnResult result;

         try
         {
            bool isNewRecord = string.IsNullOrEmpty(cablePrice.Id);

            if (isNewRecord)
            {
               cablePrice.Id = Guid.NewGuid().ToString();

               XElement cablePriceElement = ConvertToDatabaseModel(cablePrice);

               _cablePricesXDocument?.Root?.Add(cablePriceElement);
            }

            _cablePricesXDocument?.Save(_repositoryFileName);

            result = new SuccessResult(LabelProvider["UI_CableNameSaved"]);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      public ReturnResult SaveAll(List<CablePriceDbModel> cables)
      {
         foreach (CablePriceDbModel cable in cables)
         {
            Save(cable);
         }

         string message = LabelProvider["UI_NumberOfCablesLoaded"] + " " + cables.Count;

         return new SuccessResult(message);
      }

      public List<CablePriceDbModel> GetAll()
      {
         List<XElement> cableElements = _cablePricesXDocument.Descendants("CablePrice").ToList();
         var cablePriceDbModels = new List<CablePriceDbModel>();

         foreach (XElement cableElement in cableElements)
         {
            CablePriceDbModel cablePriceDbModel = ConvertFromDatabaseModel(cableElement);

            cablePriceDbModels.Add(cablePriceDbModel);
         }

         return cablePriceDbModels;
      }

      public ReturnResult DeleteAll()
      {
         ReturnResult result = new SuccessResult();

         try
         {
            _cablePricesXDocument?.Root?.RemoveAll();
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      #region Private methods

      private XElement ConvertToDatabaseModel(CablePriceDbModel cablePrice)
      {
         List<XElement> cableNames = CreateCableNames(cablePrice.CableNames);
         List<XElement> priceItems = CreatePriceItems(cablePrice.PriceItems);

         XElement cableElement = new XElement("CablePrice",
            new XElement("Id", cablePrice.Id),
            new XElement("DocumentId", cablePrice.DocumentId),
            new XElement("CableNames", cableNames),
            new XElement("PriceItems", priceItems));

         return cableElement;
      }

      private CablePriceDbModel ConvertFromDatabaseModel(XElement cablePriceElement)
      {
         string id = cablePriceElement.Element("Id")?.Value;
         string documentId = cablePriceElement.Element("DocumentId")?.Value;
         List<string> cableNames = cablePriceElement.Element("CableNames")?.Descendants("CableName").Select(x => x.Value).ToList();
         List<CableNamePriceDbModel> priceItems = cablePriceElement.Element("PriceItems")?.Descendants("PriceItem")
            .Select(x => new CableNamePriceDbModel(x.FirstAttribute.Value, x.LastAttribute.Value)).ToList();

         var cablePriceDbModel = new CablePriceDbModel(documentId, cableNames, priceItems)
         {
            Id = id
         };

         return cablePriceDbModel;
      }

      private List<XElement> CreateCableNames(List<string> cableNames)
      {
         var cableNameElements = new List<XElement>();

         foreach (string cableName in cableNames)
         {
            cableNameElements.Add(new XElement("CableName", cableName));
         }

         return cableNameElements;
      }

      private List<XElement> CreatePriceItems(List<CableNamePriceDbModel> priceItems)
      {
         var cableNameElements = new List<XElement>();

         foreach (CableNamePriceDbModel priceItem in priceItems)
         {
            cableNameElements.Add(new XElement("PriceItem",
               new XAttribute("Name", priceItem.Name),
               new XAttribute("Price", priceItem.Price)));
         }

         return cableNameElements;
      }

      #endregion
   }
}

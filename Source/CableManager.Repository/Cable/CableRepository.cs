using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using CableManager.Common.Extensions;
using CableManager.Common.Helpers;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Models;

namespace CableManager.Repository.Cable
{
   public class CableRepository : RepositoryBase, ICableRepository
   {
      private readonly string _repositoryFileName;

      private readonly XDocument _cablesXDocument;

      public CableRepository(LabelProvider labelProvider) : base(labelProvider)
      {
         _repositoryFileName = new FileInfo(DirectoryHelper.GetApplicationStoragePath() + "/Repository/Cables.xml").FullName;

         _cablesXDocument = GetXDocument(_repositoryFileName);
      }

      protected override string RootNodeName => "Cables";

      public ReturnResult Save(CableModel cable)
      {
         ReturnResult result = ValidateSave(cable);

         if (result.IsSuccess)
         {
            try
            {
               bool isNewRecord = string.IsNullOrEmpty(cable.Id);

               if (isNewRecord)
               {
                  cable.Id = Guid.NewGuid().ToString();

                  XElement cableElement = ConvertToDatabaseModel(cable);

                  _cablesXDocument?.Root?.Add(cableElement);
               }
               else
               {
                  XElement cableFound = _cablesXDocument?.Find(cable.Id);

                  if (cableFound != null)
                  {
                     UpdateDatabaseModel(cableFound, cable);
                  }
               }

               _cablesXDocument?.Save(_repositoryFileName);

               result = new SuccessResult(LabelProvider["UI_CableNameSaved"]);
            }
            catch (Exception exception)
            {
               result = new FailResult(exception.Message);
            }
         }

         return result;
      }

      public ReturnResult SaveAll(List<CableModel> cables)
      {
         foreach (CableModel cable in cables)
         {
            Save(cable);
         }

         string message = LabelProvider["UI_NumberOfCablesLoaded"] + " " + cables.Count;

         return new SuccessResult(message);
      }

      public CableModel Get(string cableId)
      {
         CableModel cable = GetAll().FirstOrDefault(x => x.Id == cableId);

         return cable;
      }

      public List<CableModel> GetAll()
      {
         List<XElement> cableElements = _cablesXDocument.Descendants("Cable").ToList();
         List<CableModel> cables = new List<CableModel>();

         foreach (XElement cableElement in cableElements)
         {
            CableModel cable = ConvertFromDatabaseModel(cableElement);

            cables.Add(cable);
         }

         return cables;
      }

      public ReturnResult DeleteAll(List<string> cableIds)
      {
         if (cableIds == null || !cableIds.Any())
         {
            return new FailResult(LabelProvider["UI_CablesNotSelected"]);
         }

         IEnumerable<XElement> allElements = _cablesXDocument.Descendants("Cable");
         List<XElement> elementsToDelete = allElements.Where(x => cableIds.Contains(x.Element("Id")?.Value)).ToList();
         ReturnResult result;

         try
         {
            foreach (XElement element in elementsToDelete)
            {
               element?.Remove();
            }

            _cablesXDocument?.Save(_repositoryFileName);

            string message = LabelProvider["UI_NumberOfCablesDeleted"] + " " + elementsToDelete.Count;

            result = new SuccessResult(message);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      #region Private methods

      private ReturnResult ValidateSave(CableModel cable)
      {
         if (cable == null)
         {
            return new FailResult(LabelProvider["UI_CableNotSelected"]);
         }

         if (string.IsNullOrEmpty(cable.Name))
         {
            return new FailResult(LabelProvider["UI_CableNameMustBeEntered"]);
         }

         bool isNewRecord = string.IsNullOrEmpty(cable.Id);
         List<CableModel> cables = GetAll();
         List<CableModel> otherCables = isNewRecord ? cables : cables.Where(x => x.Id != cable.Id).ToList();

         if (otherCables.Any(x => x.Name == cable.Name))
         {
            return new FailResult(LabelProvider["UI_CableNameAlreadySaved"]);
         }

         return new SuccessResult();
      }

      private XElement ConvertToDatabaseModel(CableModel cable)
      {
         XElement cableElement = new XElement("Cable",
            new XElement("Id", cable.Id),
            new XElement("Name", cable.Name),
            new XElement("Synonyms", cable.Synonyms));

         return cableElement;
      }

      private void UpdateDatabaseModel(XElement cableElement, CableModel cable)
      {
         cableElement.Element("Name")?.SetValue(cable.Name ?? string.Empty);
         cableElement.Element("Synonyms")?.SetValue(cable.Synonyms ?? string.Empty);
      }

      private CableModel ConvertFromDatabaseModel(XElement customerElement)
      {
         string id = customerElement.Element("Id")?.Value;
         string name = customerElement.Element("Name")?.Value;
         string synonyms = customerElement.Element("Synonyms")?.Value;

         var cable = new CableModel(id, name, synonyms);

         return cable;
      }

      #endregion
   }
}

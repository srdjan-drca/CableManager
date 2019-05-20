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

namespace CableManager.Repository.User
{
   public class UserRepository : RepositoryBase, IUserRepository
   {
      private readonly string _repositoryFileName;

      private readonly XDocument _usersXDocument;

      public UserRepository(LabelProvider labelProvider) : base(labelProvider)
      {
         _repositoryFileName = new FileInfo(DirectoryHelper.GetApplicationStoragePath() + "/Repository/Users.xml").FullName;

         _usersXDocument = GetXDocument(_repositoryFileName);
      }

      protected override string RootNodeName => "Users";

      public ReturnResult Save(UserModel user)
      {
         bool isNewRecord = string.IsNullOrEmpty(user.Id);
         ReturnResult result;

         try
         {
            if (isNewRecord)
            {
               user.Id = Guid.NewGuid().ToString();
               user.Number = GenerateUserNumber();
               user.LastOfferNumber = 0.ToString("D7");

               XElement userElement = ConvertToDatabaseModel(user);

               _usersXDocument?.Root?.Add(userElement);
            }
            else
            {
               XElement userFound = _usersXDocument?.Find(user.Id);

               if (userFound != null)
               {
                  UpdateDatabaseModel(userFound, user);
               }
            }

            _usersXDocument?.Save(_repositoryFileName);

            result = new SuccessResult(LabelProvider["UI_UserSuccessfullySaved"]);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      public UserModel Get(string userId)
      {
         var user = GetAll().FirstOrDefault(x => x.Id == userId);

         return user;
      }

      public UserModel GetByNameAndPassword(string userName, string password)
      {
         byte[] passwordHash = CryptoHelper.HashPassword(password);

         var user = GetAll().FirstOrDefault(x =>
            x.Name == userName &&
            x.Password == ConversionHelper.ToString(passwordHash));

         return user;
      }


      public List<UserModel> GetAll()
      {
         List<XElement> userElements = _usersXDocument.Descendants("User").ToList();
         List<UserModel> users = new List<UserModel>();

         foreach (XElement userElement in userElements)
         {
            UserModel user = ConvertToModel(userElement);

            users.Add(user);
         }

         return users;
      }

      public void UpdateLastOfferNumber(string userId, string lastOfferNumber)
      {
         int lastOfferNumberInteger;
         XElement userFound = _usersXDocument?.Find(userId);

         int.TryParse(lastOfferNumber, out lastOfferNumberInteger);

         lastOfferNumberInteger++;

         userFound?.Element("LastOfferNumber")?.SetValue(lastOfferNumberInteger.ToString("D7"));

         _usersXDocument?.Save(_repositoryFileName);
      }

      #region Private methods

      private string GenerateUserNumber()
      {
         List<XElement> userElements = _usersXDocument.Descendants("User").ToList();
         XElement lastElement = userElements.LastOrDefault();
         string userCode = 0.ToString("D3");

         if (lastElement != null)
         {
            int userCodeAsNumber;

            userCode = lastElement.Element("Number")?.Value;
            int.TryParse(userCode, out userCodeAsNumber);

            userCodeAsNumber++;

            userCode = userCodeAsNumber.ToString("D3");
         }

         return userCode;
      }

      private XElement ConvertToDatabaseModel(UserModel user)
      {
         byte[] passwordHash = CryptoHelper.HashPassword(user.Password);
         var userElement = new XElement("User",
            new XElement("Id", user.Id),
            new XElement("Number", user.Number),
            new XElement("Name", user.Name),
            new XElement("Password", passwordHash),
            new XElement("FirstName", user.FirstName),
            new XElement("LastName", user.LastName),
            new XElement("LastOfferNumber", user.LastOfferNumber));

         return userElement;
      }

      private void UpdateDatabaseModel(XElement userElement, UserModel user)
      {
         userElement.Element("Name")?.SetValue(user.Name);
         userElement.Element("Password")?.SetValue(user.Password);

         UpdateName(userElement, "FirstName", user.FirstName);
         UpdateName(userElement, "LastName", user.LastName);
      }

      private void UpdateName(XElement userElement, string nodeName, string nodeValue)
      {
         var userNode = userElement.Element(nodeName);

         if (userNode == null)
         {
            userElement.Element("Password")?.AddAfterSelf(new XElement(nodeName, nodeValue));
         }
         else
         {
            userElement.Element(nodeName)?.SetValue(nodeValue);
         }
      }

      private UserModel ConvertToModel(XElement userElement)
      {
         var user = new UserModel
         {
            Id = userElement.Element("Id")?.Value,
            Number = userElement.Element("Number")?.Value,
            Name = userElement.Element("Name")?.Value,
            Password = userElement.Element("Password")?.Value,
            FirstName = userElement.Element("FirstName")?.Value,
            LastName = userElement.Element("LastName")?.Value,
            LastOfferNumber = userElement.Element("LastOfferNumber")?.Value
         };

         return user;
      }

      #endregion Private methods
   }
}
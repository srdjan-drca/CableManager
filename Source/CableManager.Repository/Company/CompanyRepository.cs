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

namespace CableManager.Repository.Company
{
   public class CompanyRepository : RepositoryBase, ICompanyRepository
   {
      private readonly string _repositoryFileName;

      private readonly XDocument _companiesXDocument;

      public CompanyRepository(LabelProvider labelProvider) : base(labelProvider)
      {
         _repositoryFileName = new FileInfo(DirectoryHelper.GetApplicationStoragePath() + "/Repository/Companies.xml").FullName;

         _companiesXDocument = GetXDocument(_repositoryFileName);
      }

      protected override string RootNodeName => "Companies";

      public ReturnResult Save(CompanyModel company)
      {
         bool isNewRecord = string.IsNullOrEmpty(company.Id);
         ReturnResult result;

         try
         {
            if (isNewRecord)
            {
               company.Id = Guid.NewGuid().ToString();

               XElement companyElement = ConvertToDatabaseModel(company);

               _companiesXDocument?.Root?.Add(companyElement);
            }
            else
            {
               XElement companyFound = _companiesXDocument?.Find(company.Id);

               if (companyFound != null)
               {
                  UpdateDatabaseModel(companyFound, company);
               }
            }

            _companiesXDocument?.Save(_repositoryFileName);

            result = new SuccessResult(LabelProvider["UI_CompanyContactSaved"]);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      public List<CompanyModel> GetAll()
      {
         List<XElement> companyElements = _companiesXDocument.Descendants("Company").ToList();
         List<CompanyModel> companies = new List<CompanyModel>();

         foreach (XElement customerElement in companyElements)
         {
            CompanyModel company = ConvertToModel(customerElement);

            companies.Add(company);
         }

         return companies;
      }

      #region Private methods

      private XElement ConvertToDatabaseModel(CompanyModel company)
      {
         XElement companyElement = new XElement("Company",
            new XElement("Id", company.Id),
            new XElement("Name", company.Name),
            new XElement("Street", company.Street),
            new XElement("City", company.City),
            new XElement("TaxNumber", company.TaxNumber),
            new XElement("Phone1", company.Phone1),
            new XElement("Phone2", company.Phone2),
            new XElement("Fax", company.Fax),
            new XElement("Mobile", company.Mobile),
            new XElement("Email", company.Email));

         return companyElement;
      }

      private void UpdateDatabaseModel(XElement companyElement, CompanyModel company)
      {
         companyElement.Element("Name")?.SetValue(company.Name ?? string.Empty);
         companyElement.Element("Street")?.SetValue(company.Street ?? string.Empty);
         companyElement.Element("City")?.SetValue(company.City ?? string.Empty);
         companyElement.Element("TaxNumber")?.SetValue(company.TaxNumber ?? string.Empty);
         companyElement.Element("Phone1")?.SetValue(company.Phone1 ?? string.Empty);
         companyElement.Element("Phone2")?.SetValue(company.Phone2 ?? string.Empty);
         companyElement.Element("Fax")?.SetValue(company.Fax ?? string.Empty);
         companyElement.Element("Mobile")?.SetValue(company.Mobile ?? string.Empty);
         companyElement.Element("Email")?.SetValue(company.Email ?? string.Empty);
      }

      private CompanyModel ConvertToModel(XElement customerElement)
      {
         var company = new CompanyModel
         {
            Id = customerElement.Element("Id")?.Value,
            Name = customerElement.Element("Name")?.Value,
            Street = customerElement.Element("Street")?.Value,
            City = customerElement.Element("City")?.Value,
            TaxNumber = customerElement.Element("TaxNumber")?.Value,
            Phone1 = customerElement.Element("Phone1")?.Value,
            Phone2 = customerElement.Element("Phone2")?.Value,
            Fax = customerElement.Element("Fax")?.Value,
            Mobile = customerElement.Element("Mobile")?.Value,
            Email = customerElement.Element("Email")?.Value
         };

         return company;
      }

      #endregion
   }
}

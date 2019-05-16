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

namespace CableManager.Repository.Customer
{
   public class CustomerRepository : RepositoryBase, ICustomerRepository
   {
      private readonly string _repositoryFileName;

      private readonly XDocument _customersXDocument;

      public CustomerRepository(LabelProvider labelProvider) : base(labelProvider)
      {
         _repositoryFileName = new FileInfo(DirectoryHelper.GetApplicationStoragePath() + "/Repository/Customers.xml").FullName;

         _customersXDocument = GetXDocument(_repositoryFileName);
      }

      protected override string RootNodeName => "Customers";

      public ReturnResult Save(CustomerModel customer)
      {
         ReturnResult result = ValidateSave(customer);

         if (result.IsSuccess)
         {
            try
            {
               bool isNewRecord = string.IsNullOrEmpty(customer.Id);

               if (isNewRecord)
               {
                  customer.Id = Guid.NewGuid().ToString();

                  XElement customerElement = ConvertToDatabaseModel(customer);

                  _customersXDocument?.Root?.Add(customerElement);
               }
               else
               {
                  XElement customerFound = _customersXDocument?.Find(customer.Id);

                  if (customerFound != null)
                  {
                     UpdateDatabaseModel(customerFound, customer);
                  }
               }

               _customersXDocument?.Save(_repositoryFileName);

               result = new SuccessResult(LabelProvider["UI_CustomerSaved"]);
            }
            catch (Exception exception)
            {
               result = new FailResult(exception.Message);
            }
         }

         return result;
      }

      public CustomerModel Get(string customerId)
      {
         CustomerModel customer = GetAll().FirstOrDefault(x => x.Id == customerId);

         return customer;
      }

      public List<CustomerModel> GetAll()
      {
         List<XElement> customerElements = _customersXDocument.Descendants("Customer").ToList();
         List<CustomerModel> customers = new List<CustomerModel>();

         foreach (XElement customerElement in customerElements)
         {
            CustomerModel customer = ConvertToModel(customerElement);

            customers.Add(customer);
         }

         return customers;
      }

      public ReturnResult DeleteAll(List<string> customerIds)
      {
         if (customerIds == null || !customerIds.Any())
         {
            return new FailResult(LabelProvider["UI_CustomerMustBeSelected"]);
         }

         IEnumerable<XElement> allElements = _customersXDocument.Descendants("Customer");
         List<XElement> elementsToDelete = allElements.Where(x => customerIds.Contains(x.Element("Id")?.Value)).ToList();
         ReturnResult result;

         try
         {
            foreach (XElement element in elementsToDelete)
            {
               element?.Remove();
            }

            _customersXDocument?.Save(_repositoryFileName);

            string message = LabelProvider["UI_NumberOfCustomersDeleted"] + " " + elementsToDelete.Count;

            result = new SuccessResult(message);
         }
         catch (Exception exception)
         {
            result = new FailResult(exception.Message);
         }

         return result;
      }

      #region Private methods

      private ReturnResult ValidateSave(CustomerModel customer)
      {
         if (customer == null)
         {
            return new FailResult(LabelProvider["UI_CustomerMustBeSelected"]);
         }

         if (string.IsNullOrEmpty(customer.Name))
         {
            return new FailResult(LabelProvider["UI_CustomerNameMustBeEntered"]);
         }

         if (string.IsNullOrEmpty(customer.TaxNumber))
         {
            return new FailResult(LabelProvider["UI_CustomerTaxNumberMustBeEntered"]);
         }

         bool isNewRecord = string.IsNullOrEmpty(customer.Id);
         List<CustomerModel> customers = GetAll();
         List<CustomerModel> otherCustomers = isNewRecord
            ? customers
            : customers.Where(x => x.Id != customer.Id).ToList();

         if (otherCustomers.Any(x => x.Name == customer.Name))
         {
            return new FailResult(LabelProvider["UI_CustomerNameAlreadySaved"]);
         }

         if (otherCustomers.Any(x => x.TaxNumber == customer.TaxNumber))
         {
            return new FailResult(LabelProvider["UI_CustomerTaxNumberMustBeUnique"]);
         }

         return new SuccessResult();
      }

      private XElement ConvertToDatabaseModel(CustomerModel customer)
      {
         XElement customerElement = new XElement("Customer",
            new XElement("Id", customer.Id),
            new XElement("Name", customer.Name),
            new XElement("Street", customer.Street),
            new XElement("PostalCode", customer.PostalCode),
            new XElement("City", customer.City),
            new XElement("Country", customer.Country),
            new XElement("TaxNumber", customer.TaxNumber),
            new XElement("Rebate", customer.Rebate),
            new XElement("Email", customer.Email));

         return customerElement;
      }

      private void UpdateDatabaseModel(XElement customerElement, CustomerModel customer)
      {
         customerElement.Element("Name")?.SetValue(customer.Name);
         customerElement.Element("Street")?.SetValue(customer.Street);
         customerElement.Element("PostalCode")?.SetValue(customer.PostalCode);
         customerElement.Element("City")?.SetValue(customer.City);
         customerElement.Element("Country")?.SetValue(customer.Country);
         customerElement.Element("TaxNumber")?.SetValue(customer.TaxNumber);
         customerElement.Element("Rebate")?.SetValue(customer.Rebate);
         customerElement.Element("Email")?.SetValue(customer.Email);
      }

      private CustomerModel ConvertToModel(XElement customerElement)
      {
         var customer = new CustomerModel
         {
            Id = customerElement.Element("Id")?.Value,
            Name = customerElement.Element("Name")?.Value,
            Street = customerElement.Element("Street")?.Value,
            PostalCode = customerElement.Element("PostalCode")?.Value,
            City = customerElement.Element("City")?.Value,
            Country = customerElement.Element("Country")?.Value,
            TaxNumber = customerElement.Element("TaxNumber")?.Value,
            Rebate = customerElement.Element("Rebate")?.Value,
            Email = customerElement.Element("Email")?.Value
         };

         return customer;
      }

      #endregion Private methods
   }
}
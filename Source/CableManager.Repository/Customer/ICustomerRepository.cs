using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Repository.Models;

namespace CableManager.Repository.Customer
{
   public interface ICustomerRepository
   {
      ReturnResult Save(CustomerModel customer);

      CustomerModel Get(string customerId);

      List<CustomerModel> GetAll();

      ReturnResult DeleteAll(List<string> customerIds);
   }
}
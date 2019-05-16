using CableManager.Repository.Customer;
using GalaSoft.MvvmLight.Command;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Models;

namespace CableManager.UI.ViewModels.Pages
{
   public class CustomerAddPageViewModel : RootViewModel
   {
      private readonly ICustomerRepository _customerRepository;

      public CustomerAddPageViewModel(LabelProvider labelProvider, ICustomerRepository customerRepository) : base(labelProvider)
      {
         _customerRepository = customerRepository;

         SaveCustomerCommand = new RelayCommand<object>(SaveCustomer);
      }

      public RelayCommand<object> SaveCustomerCommand { get; set; }

      #region Private methods

      private void SaveCustomer(object parameter)
      {
         CustomerModel customer = ConvertToModel(parameter);
         ReturnResult result = _customerRepository.Save(customer);

         StatusMessage = result.Message;
      }

      private CustomerModel ConvertToModel(object parameter)
      {
         var customerParameters = (object[])parameter;
         var name = customerParameters[0] as string;
         var street = customerParameters[1] as string;
         var postalCode = customerParameters[2] as string;
         var city = customerParameters[3] as string;
         var country = customerParameters[4] as string;
         var taxNumber = customerParameters[5] as string;
         var rebate = customerParameters[6] as string;
         var email = customerParameters[7] as string;

         var customer = new CustomerModel
         {
            Name = name,
            Street = street,
            PostalCode = postalCode,
            City = city,
            Country = country,
            TaxNumber = taxNumber,
            Rebate = rebate,
            Email = email
         };

         return customer;
      }

      #endregion Private methods
   }
}
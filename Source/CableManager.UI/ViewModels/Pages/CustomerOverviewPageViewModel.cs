using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Models;
using CableManager.Repository.Customer;

namespace CableManager.UI.ViewModels.Pages
{
   public class CustomerOverviewPageViewModel : RootViewModel
   {
      private readonly ICustomerRepository _customerRepository;

      private CustomerModel _selectedCustomer;

      public CustomerOverviewPageViewModel(LabelProvider labelProvider, ICustomerRepository customerRepository) : base(labelProvider)
      {
         _customerRepository = customerRepository;

         SaveSelectedItemCommand = new RelayCommand<object>(SaveSelectedItem);
         DeleteSelectedItemsCommand = new RelayCommand<object>(DeleteSelectedItems);
      }

      public List<CustomerModel> Customers
      {
         get
         {
            List<CustomerModel> customers = _customerRepository.GetAll();

            return customers;
         }
      }

      public CustomerModel SelectedCustomer
      {
         get { return _selectedCustomer; }
         set
         {
            _selectedCustomer = value;
            RaisePropertyChanged(nameof(SelectedCustomer));

            StatusMessage = string.Empty;
         }
      }

      public RelayCommand<object> SaveSelectedItemCommand { get; set; }

      public RelayCommand<object> DeleteSelectedItemsCommand { get; set; }

      #region Private methods

      private void SaveSelectedItem(object parameter)
      {
         ReturnResult result = _customerRepository.Save(SelectedCustomer);

         if (SelectedCustomer != null)
         {
            string selectedCustomerId = SelectedCustomer.Id;

            RaisePropertyChanged(nameof(Customers));

            SelectedCustomer = _customerRepository.Get(selectedCustomerId);
         }

         StatusMessage = result.Message;
      }

      private void DeleteSelectedItems(object parameter)
      {
         List<string> selectedCustomerIds = ConvertToList(parameter)?.Select(x => x.Id).ToList();
         ReturnResult result = _customerRepository.DeleteAll(selectedCustomerIds);

         if (result.IsSuccess)
         {
            RaisePropertyChanged(nameof(Customers));
         }

         StatusMessage = result.Message;
      }

      private List<CustomerModel> ConvertToList(object parameter)
      {
         System.Collections.IList customerModels = (System.Collections.IList)parameter;
         List<CustomerModel> customers = customerModels.OfType<CustomerModel>().ToList();

         return customers;
      }

      #endregion Private methods
   }
}
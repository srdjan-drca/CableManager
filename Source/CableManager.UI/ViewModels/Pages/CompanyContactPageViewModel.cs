using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Company;
using CableManager.Repository.Models;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CableManager.UI.ViewModels.Pages
{
   public class CompanyContactPageViewModel : RootViewModel
   {
      private readonly ICompanyRepository _companyRepository;

      private CompanyModel _companyModel;

      public CompanyContactPageViewModel(LabelProvider labelProvider, ICompanyRepository companyRepository) : base(labelProvider)
      {
         _companyRepository = companyRepository;

         Company = _companyRepository.GetAll().FirstOrDefault() ?? new CompanyModel();
         BankAccounts = new ObservableCollection<string>();

         UpdateBankAccounts(Company.BankAccounts);

         SaveCompanyContactCommand = new RelayCommand<object>(SaveCompanyContact);
         BrowseLogoCommand = new RelayCommand<object>(BrowseLogo);
         DeleteAccountCommand = new RelayCommand<object>(DeleteAccount);
         AddAccountCommand = new RelayCommand<object>(AddAccount);
      }

      public CompanyModel Company
      {
         get
         {
            return _companyModel;
         }
         set
         {
            _companyModel = value;
            RaisePropertyChanged(nameof(Company));
         }
      }

      public ObservableCollection<string> BankAccounts { get; set; }

      public RelayCommand<object> SaveCompanyContactCommand { get; set; }

      public RelayCommand<object> BrowseLogoCommand { get; set; }

      public RelayCommand<object> DeleteAccountCommand { get; set; }

      public RelayCommand<object> AddAccountCommand { get; set; }

      private void SaveCompanyContact(object parameter)
      {
         ReturnResult result = _companyRepository.Save(Company);

         StatusMessage = result.Message;
      }

      private void BrowseLogo(object parameter)
      {
         var openFileDialog = new OpenFileDialog
         {
            Title = "Select logo file",
            Filter = "Logo files (*.PNG;*.JPG)|*.PNG;*.JPG"
         };

         bool? isSuccess = openFileDialog.ShowDialog();

         if (isSuccess != null && isSuccess.Value)
         {
            Company.LogoPath = openFileDialog.FileName;
            RaisePropertyChanged(nameof(Company));

            _companyRepository.Save(Company);
         }
      }

      private void DeleteAccount(object parameter)
      {
         int selectedIndex = (int)parameter;
         string bankAccount = Company.BankAccounts[selectedIndex];

         Company.BankAccounts.Remove(bankAccount);
         UpdateBankAccounts(Company.BankAccounts);

         _companyRepository.Save(Company);
      }

      private void AddAccount(object parameter)
      {
         string bankAccount = parameter as string;

         if (Company.BankAccounts.Count >= 5)
         {
            StatusMessage = LabelProvider["UI_MaxNumberOfBankAccountsIsFive"];
            return;
         }

         Company.BankAccounts.Add(bankAccount);
         UpdateBankAccounts(Company.BankAccounts);

         _companyRepository.Save(Company);
      }

      private void UpdateBankAccounts(List<string> bankAccounts)
      {
         BankAccounts.Clear();

         foreach (string bankAccount in bankAccounts)
         {
            BankAccounts.Add(bankAccount);
         }
      }
   }
}

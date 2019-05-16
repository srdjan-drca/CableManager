using System.Linq;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Company;
using CableManager.Repository.Models;
using GalaSoft.MvvmLight.Command;

namespace CableManager.UI.ViewModels.Pages
{
   public class CompanyContactPageViewModel : RootViewModel
   {
      private readonly ICompanyRepository _companyRepository;

      private CompanyModel _companyModel;

      public CompanyContactPageViewModel(LabelProvider labelProvider, ICompanyRepository companyRepository) : base(labelProvider)
      {
         _companyRepository = companyRepository;
         _companyModel = _companyRepository.GetAll().FirstOrDefault() ?? new CompanyModel();

         SaveCompanyContactCommand = new RelayCommand<object>(SaveCompanyContact);
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

      public RelayCommand<object> SaveCompanyContactCommand { get; set; }

      private void SaveCompanyContact(object parameter)
      {
         ReturnResult result = _companyRepository.Save(Company);

         StatusMessage = result.Message;
      }
   }
}

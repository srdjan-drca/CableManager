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
         _companyModel = _companyRepository.GetAll().FirstOrDefault() ?? new CompanyModel();

         SaveCompanyContactCommand = new RelayCommand<object>(SaveCompanyContact);
         BrowseLogoCommand = new RelayCommand<object>(BrowseLogo);
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

      public RelayCommand<object> BrowseLogoCommand { get; set; }

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
            Filter = "Logo files (*.PNG)|*.PNG"
         };

         bool? isSuccess = openFileDialog.ShowDialog();

         if (isSuccess != null && isSuccess.Value)
         {
            Company.LogoPath = openFileDialog.FileName;
            RaisePropertyChanged(nameof(Company));

            _companyRepository.Save(Company);
         }
      }
   }
}

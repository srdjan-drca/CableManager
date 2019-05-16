using System.Windows;
using GalaSoft.MvvmLight.Command;
using CableManager.Services.User;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.UI.Helpers;
using CableManager.UI.Navigation;
using CableManager.UI.Views.Windows;

namespace CableManager.UI.ViewModels.Windows
{
   public class MainWindowViewModel : RootViewModel
   {
      private readonly IFrameNavigationService _navigationService;

      private readonly IUserService _userService;

      public MainWindowViewModel(LabelProvider labelProvider, IFrameNavigationService navigationService, IUserService userService)
         : base(labelProvider)
      {
         _navigationService = navigationService;
         _userService = userService;

         //Offer
         NavigateOfferCreatePageCommand = new RelayCommand<object>(NavigateOfferCreatePage);
         NavigateOfferOverviewPageCommand = new RelayCommand<object>(NavigateOfferOverviewPage);
         //Cable
         NavigateCableNameAddPageCommand = new RelayCommand<object>(NavigateCableNameAddPage);
         NavigateCableNameOverviewPageCommand = new RelayCommand<object>(NavigateCableNameOverviewPage);
         NavigateCablePriceAddPageCommand = new RelayCommand<object>(NavigateCablePriceAddPage);
         //Customer
         NavigateCustomerAddPageCommand = new RelayCommand<object>(NavigateCustomerAddPage);
         NavigateCustomerOverviewPageCommand = new RelayCommand<object>(NavigateCustomerOverviewPage);
         //Company
         NavigateCompanyContactPageCommand = new RelayCommand<object>(NavigateCompanyContactPage);
         NavigateCompanyUserPageCommand = new RelayCommand<object>(NavigateCompanyUserPage);
         //Control
         NavigateSettingsPageCommand = new RelayCommand<object>(NavigateSettingsPage);
         UserLogoutCommand = new RelayCommand<object>(UserLogOut);
      }

      public string CurrentUserName
      {
         get
         {
            CurrentUser = _userService.GetCurrentlyLoggedInUser();

            return CurrentUser.Name;
         }
      }

      //Icons
      public string OfferCreateIconPath => IconProvider.GetImagePath("OfferCreate.ico");

      public string OfferOverviewIconPath => IconProvider.GetImagePath("OfferOverview.ico");

      public string CableNameAddIconPath => IconProvider.GetImagePath("CableNameAdd.ico");

      public string CableNameOverviewIconPath => IconProvider.GetImagePath("CableNameOverview.ico");

      public string CablePriceAddIconPath => IconProvider.GetImagePath("CablePriceAdd.ico");

      public string CustomerAddIconPath => IconProvider.GetImagePath("CustomerAdd.ico");

      public string CustomerOverviewIconPath => IconProvider.GetImagePath("CustomerOverview.ico");

      public string CompanyContactIconPath => IconProvider.GetImagePath("CompanyContact.ico");

      public string CompanyUserIconPath => IconProvider.GetImagePath("CompanyUser.ico");

      public string SettingsIconPath => IconProvider.GetImagePath("Settings.ico");

      public string LogOutIconPath => IconProvider.GetImagePath("LogOut.ico");

      //Offer
      public RelayCommand<object> NavigateOfferCreatePageCommand { get; }

      public RelayCommand<object> NavigateOfferOverviewPageCommand { get; }

      //Cable
      public RelayCommand<object> NavigateCableNameAddPageCommand { get; }

      public RelayCommand<object> NavigateCableNameOverviewPageCommand { get; }

      public RelayCommand<object> NavigateCablePriceAddPageCommand { get; }

      //Customer
      public RelayCommand<object> NavigateCustomerAddPageCommand { get; }

      public RelayCommand<object> NavigateCustomerOverviewPageCommand { get; }

      //Company
      public RelayCommand<object> NavigateCompanyContactPageCommand { get; }

      public RelayCommand<object> NavigateCompanyUserPageCommand { get; }

      //Control
      public RelayCommand<object> NavigateSettingsPageCommand { get; }

      public RelayCommand<object> UserLogoutCommand { get; }

      #region Private methods

      private void NavigateOfferCreatePage(object parameter)
      {
         _navigationService.NavigateTo(PageName.OfferCreatePage);
      }

      private void NavigateOfferOverviewPage(object parameter)
      {
         _navigationService.NavigateTo(PageName.OfferOverviewPage);
      }

      private void NavigateCableNameAddPage(object parameter)
      {
         _navigationService.NavigateTo(PageName.CableNameAddPage);
      }

      private void NavigateCableNameOverviewPage(object parameter)
      {
         _navigationService.NavigateTo(PageName.CableNameOverviewPage);
      }

      private void NavigateCablePriceAddPage(object parameter)
      {
         _navigationService.NavigateTo(PageName.CablePriceAddPage);
      }

      private void NavigateCustomerAddPage(object parameter)
      {
         _navigationService.NavigateTo(PageName.CustomerAddPage);
      }

      private void NavigateCustomerOverviewPage(object parameter)
      {
         _navigationService.NavigateTo(PageName.CustomerOverviewPage);
      }

      private void NavigateCompanyContactPage(object parameter)
      {
         _navigationService.NavigateTo(PageName.CompanyContactPage);
      }


      private void NavigateCompanyUserPage(object parameter)
      {
         _navigationService.NavigateTo(PageName.CompanyUserPage);
      }

      private void NavigateSettingsPage(object parameter)
      {
         _navigationService.NavigateTo(PageName.SettingsPage);
      }

      private void UserLogOut(object parameter)
      {
         ReturnResult result = _userService.LogOut();

         if (result.IsSuccess)
         {
            Window loginWindow = new LoginWindow();
            loginWindow.Show();

            var mainWindow = WindowHelper.GetWindowRef("MainWindowId");
            mainWindow.Close();

            Application.Current.MainWindow = loginWindow;
         }
      }

      #endregion Private methods
   }
}
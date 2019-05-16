using CableManager.UI.ViewModels.Controls;
using CableManager.UI.ViewModels.Pages;
using CableManager.UI.ViewModels.Windows;
using CommonServiceLocator;

namespace CableManager.UI.ViewModels
{
   public class ViewModelLocator
   {
      //Windows
      public LoginWindowViewModel LoginWindowViewModel => ServiceLocator.Current.GetInstance<LoginWindowViewModel>();

      public MainWindowViewModel MainWindowViewModel => ServiceLocator.Current.GetInstance<MainWindowViewModel>();

      public LicenseWindowViewModel LicenseWindowViewModel => ServiceLocator.Current.GetInstance<LicenseWindowViewModel>();

      //Controls
      public DragDropBoxViewModel DragDropBoxViewModel => ServiceLocator.Current.GetInstance<DragDropBoxViewModel>();

      //Pages
      public OfferCreatePageViewModel OfferCreatePageViewModel => ServiceLocator.Current.GetInstance<OfferCreatePageViewModel>();

      public OfferOverviewPageViewModel OfferOverviewPageViewModel => ServiceLocator.Current.GetInstance<OfferOverviewPageViewModel>();

      public CableNameAddPageViewModel CableNameAddPageViewModel => ServiceLocator.Current.GetInstance<CableNameAddPageViewModel>();

      public CableNameOverviewPageViewModel CableNameOverviewPageViewModel => ServiceLocator.Current.GetInstance<CableNameOverviewPageViewModel>();

      public CablePriceAddPageViewModel CablePriceAddPageViewModel => ServiceLocator.Current.GetInstance<CablePriceAddPageViewModel>();

      public CustomerAddPageViewModel CustomerAddPageViewModel => ServiceLocator.Current.GetInstance<CustomerAddPageViewModel>();

      public CustomerOverviewPageViewModel CustomerOverviewPageViewModel => ServiceLocator.Current.GetInstance<CustomerOverviewPageViewModel>();

      public CompanyContactPageViewModel CompanyContactPageViewModel => ServiceLocator.Current.GetInstance<CompanyContactPageViewModel>();

      public CompanyUserPageViewModel CompanyUserPageViewModel => ServiceLocator.Current.GetInstance<CompanyUserPageViewModel>();

      public SettingsPageViewModel SettingsPageViewModel => ServiceLocator.Current.GetInstance<SettingsPageViewModel>();
   }
}
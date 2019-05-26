using System;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using CableManager.Localization;
using CableManager.Repository.User;
using CableManager.Repository.Cable;
using CableManager.Repository.Company;
using CableManager.Repository.Customer;
using CableManager.Repository.OfferDocument;
using CableManager.Repository.PriceDocument;
using CableManager.Services.User;
using CableManager.Services.Report;
using CableManager.Services.License;
using CableManager.UI.Navigation;
using CableManager.UI.ViewModels.Controls;
using CableManager.UI.ViewModels.Pages;
using CableManager.UI.ViewModels.Windows;
using CableManager.Report;
using CableManager.Services.Offer;
using CableManager.Services.Search;
using CableManager.PriceLoader.Core;

namespace CableManager.UI.Configuration
{
   public static class BootStrapper
   {
      public static void Initialize()
      {
         ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

         var navigationService = CreateNavigationService();

         //Windows
         SimpleIoc.Default.Register<LoginWindowViewModel>();
         SimpleIoc.Default.Register<MainWindowViewModel>();
         SimpleIoc.Default.Register<LicenseWindowViewModel>();

         //Controls
         SimpleIoc.Default.Register<DragDropBoxViewModel>();

         //Pages
         SimpleIoc.Default.Register<OfferCreatePageViewModel>();
         SimpleIoc.Default.Register<OfferOverviewPageViewModel>();
         SimpleIoc.Default.Register<CableNameAddPageViewModel>();
         SimpleIoc.Default.Register<CableNameOverviewPageViewModel>();
         SimpleIoc.Default.Register<CablePriceAddPageViewModel>();
         SimpleIoc.Default.Register<CustomerAddPageViewModel>();
         SimpleIoc.Default.Register<CustomerOverviewPageViewModel>();
         SimpleIoc.Default.Register<CompanyContactPageViewModel>();
         SimpleIoc.Default.Register<CompanyUserPageViewModel>();
         SimpleIoc.Default.Register<SettingsPageViewModel>();

         //Services
         SimpleIoc.Default.Register<IUserService, UserService>();
         SimpleIoc.Default.Register<IReportService, ReportService>();
         SimpleIoc.Default.Register<ILicenseService, LicenseService>();
         SimpleIoc.Default.Register<IOfferService, OfferService>();
         SimpleIoc.Default.Register<ICableSearchService, CableSearchService>();
         //Other
         SimpleIoc.Default.Register<LabelProvider>();
         SimpleIoc.Default.Register<ICableManagerReport, CableManagerReport>();
         SimpleIoc.Default.Register<IPriceLoader, SimplePriceLoader>();
         SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);

         //Repositories
         SimpleIoc.Default.Register<ICustomerRepository, CustomerRepository>();
         SimpleIoc.Default.Register<IUserRepository, UserRepository>();
         SimpleIoc.Default.Register<ICablePriceDocumentRepository, CablePriceDocumentRepository>();
         SimpleIoc.Default.Register<IOfferDocumentRepository, OfferDocumentRepository>();
         SimpleIoc.Default.Register<ICompanyRepository, CompanyRepository>();
         SimpleIoc.Default.Register<ICableRepository, CableRepository>();
      }

      private static IFrameNavigationService CreateNavigationService()
      {
         var navigationService = new FrameNavigationService();

         //Offer
         navigationService.Configure(PageName.OfferCreatePage, new Uri("../../Views/Pages/OfferCreatePage.xaml", UriKind.Relative));
         navigationService.Configure(PageName.OfferOverviewPage, new Uri("../../Views/Pages/OfferOverviewPage.xaml", UriKind.Relative));
         //Cable
         navigationService.Configure(PageName.CableNameAddPage, new Uri("../../Views/Pages/CableNameAddPage.xaml", UriKind.Relative));
         navigationService.Configure(PageName.CableNameOverviewPage, new Uri("../../Views/Pages/CableNameOverviewPage.xaml", UriKind.Relative));
         navigationService.Configure(PageName.CablePriceAddPage, new Uri("../../Views/Pages/CablePriceAddPage.xaml", UriKind.Relative));
         //Customer
         navigationService.Configure(PageName.CustomerAddPage, new Uri("../../Views/Pages/CustomerAddPage.xaml", UriKind.Relative));
         navigationService.Configure(PageName.CustomerOverviewPage, new Uri("../../Views/Pages/CustomerOverviewPage.xaml", UriKind.Relative));
         //Company
         navigationService.Configure(PageName.CompanyContactPage, new Uri("../../Views/Pages/CompanyContactPage.xaml", UriKind.Relative));
         navigationService.Configure(PageName.CompanyUserPage, new Uri("../../Views/Pages/CompanyUserPage.xaml", UriKind.Relative));
         //Controls
         navigationService.Configure(PageName.SettingsPage, new Uri("../../Views/Pages/SettingsPage.xaml", UriKind.Relative));

         return navigationService;
      }
   }
}
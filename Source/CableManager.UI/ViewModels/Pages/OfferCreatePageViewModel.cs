using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using CableManager.Common.Helpers;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.OfferDocument;
using CableManager.Repository.Customer;
using CableManager.Repository.Models;
using CableManager.Services.Report;
using CableManager.UI.Notification;

namespace CableManager.UI.ViewModels.Pages
{
   public class OfferCreatePageViewModel : RootViewModel
   {
      private readonly ICustomerRepository _customerRepository;

      private readonly IOfferDocumentRepository _offerDocumentRepository;

      private readonly IReportService _reportService;

      private string _customerRequestFile;

      private string _selectedCustomer;

      private ObservableCollection<string> _customers;

      private string _note;

      public OfferCreatePageViewModel(LabelProvider labelProvider, ICustomerRepository customerRepository,
         IOfferDocumentRepository offerDocumentRepository, IReportService reportService) : base(labelProvider)
      {
         _customerRepository = customerRepository;
         _offerDocumentRepository = offerDocumentRepository;
         _reportService = reportService;

         BrowseCustomerRequestFileCommand = new RelayCommand<object>(BrowseCustomerRequestFile);
         CreateOfferPdfCommand = new RelayCommand<object>(CreateOfferPdf);
         CreateOfferExcelCommand = new RelayCommand<object>(CreateOfferExcel);

         MessengerInstance.Register<Message>(this, NotificationHandler);
      }

      public string CustomerRequestFile
      {
         get { return _customerRequestFile; }
         set
         {
            _customerRequestFile = value;
            RaisePropertyChanged(nameof(CustomerRequestFile));
         }
      }

      public string SelectedCustomer
      {
         get { return _selectedCustomer; }
         set
         {
            string customerName = _customerRepository.GetAll()
               .FirstOrDefault(x => string.Compare(x.Name, value, StringComparison.OrdinalIgnoreCase) == 0)?.Name;

            _selectedCustomer = string.IsNullOrEmpty(customerName)
               ? value
               : customerName;

            RaisePropertyChanged(nameof(SelectedCustomer));
         }
      }

      public ObservableCollection<string> Customers
      {
         get
         {
            _customers = new ObservableCollection<string>();

            foreach (string customerName in _customerRepository.GetAll().Select(x => x.Name).ToList())
            {
               _customers.Add(customerName);
            }

            return _customers;
         }
      }

      public string Note
      {
         get { return _note; }
         set
         {
            _note = value;
            RaisePropertyChanged(nameof(Note));
         }
      }

      public RelayCommand<object> BrowseCustomerRequestFileCommand { get; }

      public RelayCommand<object> CreateOfferPdfCommand { get; }

      public RelayCommand<object> CreateOfferExcelCommand { get; }

      #region Private methods

      private void BrowseCustomerRequestFile(object parameter)
      {
         var openFileDialog = new OpenFileDialog
         {
            Title = "Select customer request file",
            Filter = "Input files (*.XLS;*.XLSX)|*.XLS;*.XLSX"
         };

         bool? isSuccess = openFileDialog.ShowDialog();

         if (isSuccess != null && isSuccess.Value)
         {
            CustomerRequestFile = openFileDialog.FileName;
            StatusMessage = LabelProvider["UI_CustomerRequestAdded"];
         }
      }

      private void CreateOfferPdf(object parameter)
      {
         if (string.IsNullOrEmpty(SelectedCustomer))
         {
            StatusMessage = LabelProvider["UI_CustomerNameMustBeEntered"];
            return;
         }

         if (string.IsNullOrEmpty(CustomerRequestFile))
         {
            StatusMessage = LabelProvider["UI_CustomerRequestMustBeSelected"];
            return;
         }

         string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
         var offerName = CreateOfferName(SelectedCustomer, date, ".pdf");
         var path = CreateFullPath(offerName);
         string selectedLanguage = Properties.Settings.Default.SelectedLanguage;

         ReturnResult result = _reportService.GeneratePdfReport(offerName, selectedLanguage);

         if (result.IsSuccess)
         {
            var offerDocument = new OfferDocumentModel(offerName, path, date);

            result = _offerDocumentRepository.Save(offerDocument);
         }

         StatusMessage = result.Message;
      }

      private void CreateOfferExcel(object parameter)
      {
         if (string.IsNullOrEmpty(SelectedCustomer))
         {
            StatusMessage = LabelProvider["UI_CustomerNameMustBeEntered"];
            return;
         }

         if (string.IsNullOrEmpty(CustomerRequestFile))
         {
            StatusMessage = LabelProvider["UI_CustomerRequestMustBeSelected"];
            return;
         }

         string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
         var offerName = CreateOfferName(SelectedCustomer, date, ".xlsx");
         var path = CreateFullPath(offerName);

         ReturnResult result = _reportService.GenerateExcelReport(offerName, Properties.Settings.Default.SelectedLanguage);

         if (result.IsSuccess)
         {
            var offerDocument = new OfferDocumentModel(offerName, path, date);

            result = _offerDocumentRepository.Save(offerDocument);
         }

         StatusMessage = result.Message;
      }

      private string CreateOfferName(string customerName, string date, string extension)
      {
         string date3 = string.Join("_", date.Split(new[] { " " }, StringSplitOptions.None).Take(2));
         string dateNormalized = date3.Replace("/", "_").Replace(":", "_");

         return customerName + "_" + dateNormalized + extension;
      }

      private string CreateFullPath(string name)
      {
         return new FileInfo(DirectoryHelper.GetApplicationStoragePath() + "/Offers/" + name).FullName;
      }

      private void NotificationHandler(Message message)
      {
         if (message.Type == MessageType.CustomerRequest)
         {
            string notification = message.RecordId;

            CustomerRequestFile = notification;
            StatusMessage = LabelProvider["UI_CustomerRequestAdded"];
         }
      }

      #endregion Private methods
   }
}
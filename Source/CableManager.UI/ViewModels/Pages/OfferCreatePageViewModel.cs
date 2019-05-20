using Microsoft.Win32;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using CableManager.Localization;
using CableManager.Report.Result;
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

      private CustomerModel _selectedCustomer;

      private string _customerRequestFile;

      private string _selectedCustomerName;

      private ObservableCollection<string> _customerNames;

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

      public string SelectedCustomerName
      {
         get { return _selectedCustomerName; }
         set
         {
            _selectedCustomer = _customerRepository.GetAll().FirstOrDefault(x =>
               string.Compare(x.Name, value, StringComparison.OrdinalIgnoreCase) == 0);

            _selectedCustomerName = string.IsNullOrEmpty(_selectedCustomer?.Name)
               ? value
               : _selectedCustomer.Name;

            RaisePropertyChanged(nameof(SelectedCustomerName));
         }
      }

      public ObservableCollection<string> CustomerNames
      {
         get
         {
            _customerNames = new ObservableCollection<string>();

            foreach (string customerName in _customerRepository.GetAll().Select(x => x.Name).ToList())
            {
               _customerNames.Add(customerName);
            }

            return _customerNames;
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
         if (string.IsNullOrEmpty(SelectedCustomerName))
         {
            StatusMessage = LabelProvider["UI_CustomerNameMustBeEntered"];
            return;
         }

         if (string.IsNullOrEmpty(CustomerRequestFile))
         {
            StatusMessage = LabelProvider["UI_CustomerRequestMustBeSelected"];
            return;
         }

         string selectedLanguage = Properties.Settings.Default.SelectedLanguage;
         OfferResult result = _reportService.GeneratePdfReport(SelectedCustomerName, _selectedCustomer.Id, Note, selectedLanguage);

         if (result.IsSuccess)
         {
            var offerDocument = new OfferDocumentModel(result.FileName, result.FileFullName, result.Date);

            result.Message = _offerDocumentRepository.Save(offerDocument).Message;
         }

         StatusMessage = result.Message;
      }

      private void CreateOfferExcel(object parameter)
      {
         if (string.IsNullOrEmpty(SelectedCustomerName))
         {
            StatusMessage = LabelProvider["UI_CustomerNameMustBeEntered"];
            return;
         }

         if (string.IsNullOrEmpty(CustomerRequestFile))
         {
            StatusMessage = LabelProvider["UI_CustomerRequestMustBeSelected"];
            return;
         }

         string selectedLanguage = Properties.Settings.Default.SelectedLanguage;
         OfferResult result = _reportService.GenerateExcelReport(SelectedCustomerName, _selectedCustomer.Id, Note, selectedLanguage);

         if (result.IsSuccess)
         {
            var offerDocument = new OfferDocumentModel(result.FileName, result.FileFullName, result.Date);

            result.Message = _offerDocumentRepository.Save(offerDocument).Message;
         }

         StatusMessage = result.Message;
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
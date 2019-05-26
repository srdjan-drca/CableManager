using Microsoft.Win32;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using CableManager.Common.Result;
using GalaSoft.MvvmLight.Command;
using CableManager.Localization;
using CableManager.Repository.OfferDocument;
using CableManager.Repository.Customer;
using CableManager.Repository.Models;
using CableManager.Services.Calculation;
using CableManager.Services.Calculation.Models;
using CableManager.Services.Report;
using CableManager.Services.User;
using CableManager.UI.Notification;

namespace CableManager.UI.ViewModels.Pages
{
   public class OfferCreatePageViewModel : RootViewModel
   {
      private readonly ICustomerRepository _customerRepository;

      private readonly IOfferDocumentRepository _offerDocumentRepository;

      private readonly IUserService _userService;

      private readonly IOfferService _offerService;

      private readonly IReportService _reportService;

      private CustomerModel _selectedCustomer;

      private string _customerRequestFile;

      private string _selectedCustomerName;

      private ObservableCollection<string> _customerNames;

      private string _note;

      public OfferCreatePageViewModel(LabelProvider labelProvider, ICustomerRepository customerRepository, IOfferDocumentRepository offerDocumentRepository,
         IUserService userService, IOfferService offerService, IReportService reportService) : base(labelProvider)
      {
         _customerRepository = customerRepository;
         _offerDocumentRepository = offerDocumentRepository;
         _userService = userService;
         _offerService = offerService;
         _reportService = reportService;

         BrowseCustomerRequestFileCommand = new RelayCommand<object>(BrowseCustomerRequestFile);
         CreateOfferPdfCommand = new RelayCommand<object>(CreateOfferPdf);
         CreateOfferExcelCommand = new RelayCommand<object>(CreateOfferExcel);
         ClearOfferCommand = new RelayCommand<object>(ClearOffer);

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

      public RelayCommand<object> ClearOfferCommand { get; }

      #region Private methods

      private void BrowseCustomerRequestFile(object parameter)
      {
         var openFileDialog = new OpenFileDialog
         {
            Title = "Select customer request file",
            Filter = "Input files (*.XLSX)|*.XLSX"
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
         Offer offer = null;
         ReturnResult result = ValidateInput(SelectedCustomerName, CustomerRequestFile);

         if (result.IsSuccess)
         {
            try
            {
               offer = _offerService.CreateOffer(CustomerRequestFile, _selectedCustomer.Id, Note, OfferType.Pdf);
            }
            catch (Exception exception)
            {
               result.Message = exception.Message;
            }

            if (offer != null)
            {
               result = _reportService.GeneratePdfReport(offer);

               if (result.IsSuccess)
               {
                  _userService.UpdateLastOfferNumber(offer.User.Id, offer.User.LastOfferNumber);

                  var offerDocument = new OfferDocumentModel(offer.Name, offer.FullName, offer.Date);

                  result.Message = _offerDocumentRepository.Save(offerDocument).Message;
               }
            }
         }

         StatusMessage = result.Message;
      }

      private void CreateOfferExcel(object parameter)
      {
         Offer offer = null;
         ReturnResult result = ValidateInput(SelectedCustomerName, CustomerRequestFile);

         if (result.IsSuccess)
         {
            try
            {
               offer = _offerService.CreateOffer(CustomerRequestFile, _selectedCustomer.Id, Note, OfferType.Excel);
            }
            catch (Exception exception)
            {
               result.Message = exception.Message;
            }

            if (offer != null)
            {
               result = _reportService.GenerateExcelReport(offer);

               if (result.IsSuccess)
               {
                  _userService.UpdateLastOfferNumber(offer.User.Id, offer.User.LastOfferNumber);

                  var offerDocument = new OfferDocumentModel(offer.Name, offer.FullName, offer.Date);

                  result.Message = _offerDocumentRepository.Save(offerDocument).Message;
               }
            }
         }

         StatusMessage = result.Message;
      }

      private void ClearOffer(object parameter)
      {
         CustomerRequestFile = string.Empty;
         SelectedCustomerName = string.Empty;
      }

      private ReturnResult ValidateInput(string customerName, string customerRequestFile)
      {
         string message;

         if (string.IsNullOrEmpty(customerName))
         {
            message = LabelProvider["UI_CustomerNameMustBeEntered"];

            return new FailResult(message);
         }

         if (string.IsNullOrEmpty(customerRequestFile))
         {
            message = LabelProvider["UI_CustomerRequestMustBeSelected"];

            return new FailResult(message);
         }

         return new SuccessResult();
      }

      private void NotificationHandler(Message message)
      {
         if (message.Type == MessageType.CustomerRequest)
         {
            string recordId = message.RecordId;

            if (string.IsNullOrEmpty(recordId))
            {
               StatusMessage = LabelProvider["UI_CustomerRequestMustBeXlsx"];
            }
            else
            {
               CustomerRequestFile = recordId;
               StatusMessage = LabelProvider["UI_CustomerRequestAdded"];
            }
         }
      }

      #endregion Private methods
   }
}
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using CableManager.Common.Extensions;
using CableManager.Common.Result;
using GalaSoft.MvvmLight.Command;
using CableManager.Localization;
using CableManager.Repository.CablePriceDocument;
using CableManager.Repository.OfferDocument;
using CableManager.Repository.Customer;
using CableManager.Repository.Models;
using CableManager.Services.Offer;
using CableManager.Services.Offer.Models;
using CableManager.Services.Report;
using CableManager.Services.User;
using CableManager.UI.Notification;

namespace CableManager.UI.ViewModels.Pages
{
   public class OfferCreatePageViewModel : RootViewModel
   {
      private readonly ICustomerRepository _customerRepository;

      private readonly IOfferDocumentRepository _offerDocumentRepository;

      private readonly ICablePriceDocumentRepository _cablePriceDocumentRepository;

      private readonly IUserService _userService;

      private readonly IOfferService _offerService;

      private readonly IReportService _reportService;

      private CustomerModel _selectedCustomer;

      private string _customerRequestFile;

      private string _selectedCustomerName;

      private ObservableCollection<string> _customerNames;

      private string _note;

      public OfferCreatePageViewModel(LabelProvider labelProvider, ICustomerRepository customerRepository, IOfferDocumentRepository offerDocumentRepository,
         ICablePriceDocumentRepository cablePriceDocumentRepository, IUserService userService, IOfferService offerService, IReportService reportService) : base(labelProvider)
      {
         _customerRepository = customerRepository;
         _offerDocumentRepository = offerDocumentRepository;
         _cablePriceDocumentRepository = cablePriceDocumentRepository;
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
         List<PriceDocumentModel> priceDocuments = _cablePriceDocumentRepository.GetAll();
         ReturnResult result = ValidateInput(SelectedCustomerName, CustomerRequestFile, priceDocuments);
         OfferModel offer = null;

         if (result.IsSuccess)
         {
            try
            {
               var offerParameters = new OfferParameters
               {
                  OfferType = OfferType.Pdf,
                  PriceDocumentIds = priceDocuments.Where(x => x.IsSelected).ToHashSet(x => x.Id),
                  CustomerRequestFilePath = CustomerRequestFile,
                  CustomerId = _selectedCustomer.Id,
                  Note = Note
               };

               offer = _offerService.CreateOffer(offerParameters);
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
         List<PriceDocumentModel> priceDocuments = _cablePriceDocumentRepository.GetAll();
         ReturnResult result = ValidateInput(SelectedCustomerName, CustomerRequestFile, priceDocuments);
         OfferModel offer = null;

         if (result.IsSuccess)
         {
            try
            {
               var offerParameters = new OfferParameters
               {
                  OfferType = OfferType.Excel,
                  PriceDocumentIds = priceDocuments.Where(x => x.IsSelected).ToHashSet(x => x.Id),
                  CustomerRequestFilePath = CustomerRequestFile,
                  CustomerId = _selectedCustomer.Id,
                  Note = Note
               };

               offer = _offerService.CreateOffer(offerParameters);
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

      private ReturnResult ValidateInput(string customerName, string customerRequestFile, List<PriceDocumentModel> priceDocuments)
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

         if (priceDocuments.All(x => x.IsSelected == false))
         {
            message = LabelProvider["UI_SelectPriceListDocument"];

            return new FailResult(message);
         }

         return new SuccessResult();
      }

      private void NotificationHandler(Message message)
      {
         if (message.Type == MessageType.CustomerRequest)
         {
            string recordId = message.RecordId as string;

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
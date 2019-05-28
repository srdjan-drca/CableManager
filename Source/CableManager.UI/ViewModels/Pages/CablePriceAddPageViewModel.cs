using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using CableManager.Localization;
using CableManager.Repository.Models;
using CableManager.Repository.PriceDocument;

namespace CableManager.UI.ViewModels.Pages
{
   public class CablePriceAddPageViewModel : RootViewModel
   {
      private readonly ICablePriceDocumentRepository _cablePriceDocumentRepository;

      private List<PriceDocumentModel> _priceDocuments;

      private PriceDocumentModel _selectedPriceDocument;

      public CablePriceAddPageViewModel(LabelProvider labelProvider, ICablePriceDocumentRepository cablePriceDocumentRepository)
         : base(labelProvider)
      {
         _cablePriceDocumentRepository = cablePriceDocumentRepository;
         PriceDocuments = _cablePriceDocumentRepository.GetAll();

         BrowsePriceDocumentCommand = new RelayCommand<object>(BrowsePriceDocument);
         SelectPriceDocumentForOfferCommand = new RelayCommand<object>(SelectPriceDocumentForOffer);
      }

      public List<PriceDocumentModel> PriceDocuments
      {
         get
         {
            return _priceDocuments;
         }
         set
         {
            _priceDocuments = value;
            RaisePropertyChanged(nameof(PriceDocuments));
         }
      }

      public PriceDocumentModel SelectedPriceDocument
      {
         get { return _selectedPriceDocument; }
         set
         {
            _selectedPriceDocument = value;
            RaisePropertyChanged(nameof(SelectedPriceDocument));
         }
      }

      public RelayCommand<object> BrowsePriceDocumentCommand { get; }

      public RelayCommand<object> SelectPriceDocumentForOfferCommand { get; }

      #region Private methods

      private void BrowsePriceDocument(object parameter)
      {
         var openFileDialog = new OpenFileDialog
         {
            Title = "Select price list files",
            Filter = "Price list files (*.XLSX;*.PDF)|*.XLSX;*.PDF",
            Multiselect = true
         };

         bool? isSuccess = openFileDialog.ShowDialog();

         if (isSuccess != null && isSuccess.Value)
         {
            _cablePriceDocumentRepository.DeleteAll();

            foreach (string fullFileName in openFileDialog.FileNames)
            {
               string fileName = Path.GetFileName(fullFileName);
               string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);

               var priceDocument = new PriceDocumentModel(fileName, fullFileName, date);

               _cablePriceDocumentRepository.Save(priceDocument);
            }

            PriceDocuments?.Clear();
            PriceDocuments = _cablePriceDocumentRepository.GetAll();

            StatusMessage = LabelProvider["UI_PriceDocumentsLoaded"];
         }
      }

      private void SelectPriceDocumentForOffer(object parameter)
      {
         string documentId = SelectedPriceDocument.Id;
         bool? isSelected = parameter as bool?;

         if (isSelected != null)
         {
            var priceDocument = PriceDocuments.FirstOrDefault(x => x.Id == documentId);

            if (priceDocument != null)
            {
               priceDocument.IsSelected = isSelected.Value;
               _cablePriceDocumentRepository.Save(priceDocument);
            }
         }
      }

      #endregion Private methods
   }
}
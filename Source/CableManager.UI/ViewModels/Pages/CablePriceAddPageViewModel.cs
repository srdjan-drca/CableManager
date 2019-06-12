using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using CableManager.Localization;
using CableManager.PriceLoader.Core;
using CableManager.PriceLoader.Models;
using CableManager.Repository.CableName;
using CableManager.Repository.CablePrice;
using CableManager.Repository.CablePriceDocument;
using CableManager.Repository.Models;
using CableManager.ModelConverter;

namespace CableManager.UI.ViewModels.Pages
{
   public class CablePriceAddPageViewModel : RootViewModel
   {
      private readonly ICableNameRepository _cableNameRepository;

      private readonly ICablePriceRepository _cablePriceRepository;

      private readonly ICablePriceDocumentRepository _cablePriceDocumentRepository;

      private readonly IPriceLoader _priceLoader;

      private readonly ICablePriceModelConverter _cablePriceModelConverter;

      private List<PriceDocumentModel> _priceDocuments;

      private PriceDocumentModel _selectedPriceDocument;

      public CablePriceAddPageViewModel(LabelProvider labelProvider, ICableNameRepository cableNameRepository,
         ICablePriceRepository cablePriceRepository, ICablePriceDocumentRepository cablePriceDocumentRepository,
         IPriceLoader priceLoader, ICablePriceModelConverter cablePriceModelConverter) : base(labelProvider)
      {
         _cableNameRepository = cableNameRepository;
         _cablePriceRepository = cablePriceRepository;
         _cablePriceDocumentRepository = cablePriceDocumentRepository;
         _priceLoader = priceLoader;
         _cablePriceModelConverter = cablePriceModelConverter;

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
            StatusMessage = LabelProvider["UI_PriceDocumentsAreLoading"];

            foreach (string fullFileName in openFileDialog.FileNames)
            {
               string fileName = Path.GetFileName(fullFileName);
               string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
               PriceDocumentModel priceDocumentModel = PriceDocuments.FirstOrDefault(x => x.Name == fileName);

               _cablePriceDocumentRepository.Delete(priceDocumentModel?.Id);

               var priceDocument = new PriceDocumentModel(fileName, fullFileName, date);

               _cablePriceDocumentRepository.Save(priceDocument);
            }

            PriceDocuments?.Clear();
            PriceDocuments = _cablePriceDocumentRepository.GetAll();

            List<CableModel> cableNames = _cableNameRepository.GetAll();
            List<List<string>> searchCriteriaList = CreateSearchCriteria(cableNames);
            List<CablePriceModel> cablePriceModels = LoadPrices(PriceDocuments, searchCriteriaList);
            List<CablePriceDbModel> cablePriceDbModels = _cablePriceModelConverter.ToDbModels(cablePriceModels);

            _cablePriceRepository.DeleteAll();
            _cablePriceRepository.SaveAll(cablePriceDbModels);

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

      private List<List<string>> CreateSearchCriteria(List<CableModel> cablesDb)
      {
         var cableNames = new List<List<string>>();

         foreach (CableModel cableModel in cablesDb)
         {
            List<string> synonyms = cableModel.Synonyms.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            synonyms.Add(cableModel.Name.Trim());

            cableNames.Add(synonyms);
         }

         return cableNames;
      }

      private List<CablePriceModel> LoadPrices(List<PriceDocumentModel> priceDocuments, List<List<string>> searchCriteriaList)
      {
         var prices = new List<CablePriceModel>();

         foreach (PriceDocumentModel priceDocument in priceDocuments)
         {
            string extension = Path.GetExtension(priceDocument.Path);

            switch (extension)
            {
               case ".pdf":
                  List<CablePriceModel> priceModelsPdf = _priceLoader.LoadPricesFromPdf(priceDocument.Path, priceDocument.Id);

                  UpdatePriceModels(priceModelsPdf, searchCriteriaList);

                  prices.AddRange(priceModelsPdf);
                  break;

               case ".xlsx":
                  List<CablePriceModel> priceModelsExcel = _priceLoader.LoadPricesFromExcel(priceDocument.Path, priceDocument.Id);

                  UpdatePriceModels(priceModelsExcel, searchCriteriaList);

                  prices.AddRange(priceModelsExcel);
                  break;

               default:
                  throw new Exception("Document format not supported");
            }
         }

         return prices;
      }

      private void UpdatePriceModels(List<CablePriceModel> priceModelsPdf, List<List<string>> searchCriteriaList)
      {
         foreach (CablePriceModel priceModel in priceModelsPdf)
         {
            foreach (List<string> searchCriteria in searchCriteriaList)
            {
               var intersect = priceModel.CableNames.Intersect(searchCriteria);

               if (intersect.Any())
               {
                  priceModel.CableNames.AddRange(searchCriteria);
                  priceModel.CableNames = priceModel.CableNames.Distinct().ToList();
               }
            }
         }
      }

      #endregion Private methods
   }
}
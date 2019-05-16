using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Models;
using CableManager.Repository.OfferDocument;
using CableManager.Services.Report;
using GalaSoft.MvvmLight.Command;

namespace CableManager.UI.ViewModels.Pages
{
   public class OfferOverviewPageViewModel : RootViewModel
   {
      private readonly IOfferDocumentRepository _offerDocumentRepository;

      private readonly IReportService _reportService;

      public OfferOverviewPageViewModel(LabelProvider labelProvider, IOfferDocumentRepository offerDocumentRepository,
         IReportService reportService) : base(labelProvider)
      {
         _offerDocumentRepository = offerDocumentRepository;
         _reportService = reportService;

         DeleteSelectedItemsCommand = new RelayCommand<object>(DeleteSelectedItems);
         OpenSelectedItemCommand = new RelayCommand<object>(OpenSelectedItem);
      }

      public List<OfferDocumentModel> OfferDocuments
      {
         get
         {
            List<OfferDocumentModel> offerDocuments = _offerDocumentRepository.GetAll();

            return offerDocuments;
         }
      }

      public RelayCommand<object> DeleteSelectedItemsCommand { get; set; }

      public RelayCommand<object> OpenSelectedItemCommand { get; set; }

      #region Private methods

      private void DeleteSelectedItems(object parameter)
      {
         List<OfferDocumentModel> selectedDocuments = ConvertToList(parameter);
         List<string> selectedDocumentIds = selectedDocuments?.Select(x => x.Id).ToList();
         List<string> selectedDocumentPaths = selectedDocuments?.Select(x => x.Path).ToList();

         ReturnResult result = _reportService.DeleteReport(selectedDocumentPaths);

         if (result.IsSuccess)
         {
            result = _offerDocumentRepository.DeleteAll(selectedDocumentIds);

            RaisePropertyChanged(nameof(OfferDocuments));
         }

         StatusMessage = result.Message;
      }

      private void OpenSelectedItem(object parameter)
      {
         var selectedDocument = parameter as OfferDocumentModel;

         if (string.IsNullOrEmpty(selectedDocument?.Id))
         {
            StatusMessage = LabelProvider["UI_OfferDocumentNotSelected"];
            return;
         }

         Process.Start(selectedDocument.Path);
      }

      private List<OfferDocumentModel> ConvertToList(object parameter)
      {
         System.Collections.IList cableModels = (System.Collections.IList)parameter;
         List<OfferDocumentModel> offerDocuments = cableModels.OfType<OfferDocumentModel>().ToList();

         return offerDocuments;
      }

      #endregion
   }
}
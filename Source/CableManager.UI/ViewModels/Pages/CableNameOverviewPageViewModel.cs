using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.CableName;
using CableManager.Repository.Models;

namespace CableManager.UI.ViewModels.Pages
{
   public class CableNameOverviewPageViewModel : RootViewModel
   {
      private readonly ICableNameRepository _cableNameRepository;

      private CableModel _selectedCable;

      public CableNameOverviewPageViewModel(LabelProvider labelProvider, ICableNameRepository cableNameRepository) : base(labelProvider)
      {
         _cableNameRepository = cableNameRepository;

         SaveSelectedItemCommand = new RelayCommand<object>(SaveSelectedItem);
         DeleteSelectedItemsCommand = new RelayCommand<object>(DeleteSelectedItems);
      }

      public List<CableModel> Cables
      {
         get {
            List<CableModel> cables = _cableNameRepository.GetAll();

            return cables;
         }
      }

      public CableModel SelectedCable
      {
         get { return _selectedCable; }
         set
         {
            _selectedCable = value;
            RaisePropertyChanged(nameof(SelectedCable));

            StatusMessage = string.Empty;
         }
      }

      public RelayCommand<object> SaveSelectedItemCommand { get; set; }

      public RelayCommand<object> DeleteSelectedItemsCommand { get; set; }

      #region Private methods

      private void SaveSelectedItem(object parameter)
      {
         ReturnResult result = _cableNameRepository.Save(SelectedCable);

         if (SelectedCable != null)
         {
            string selectedCableId = SelectedCable.Id;

            RaisePropertyChanged(nameof(Cables));

            SelectedCable = _cableNameRepository.Get(selectedCableId);
         }

         StatusMessage = result.Message;
      }

      private void DeleteSelectedItems(object parameter)
      {
         List<string> selectedCableIds = ConvertToList(parameter)?.Select(x => x.Id).ToList();
         ReturnResult result = _cableNameRepository.DeleteAll(selectedCableIds);

         if (result.IsSuccess)
         {
            RaisePropertyChanged(nameof(Cables));
         }

         StatusMessage = result.Message;
      }

      private List<CableModel> ConvertToList(object parameter)
      {
         System.Collections.IList cableModels = (System.Collections.IList)parameter;
         List<CableModel> cables = cableModels.OfType<CableModel>().ToList();

         return cables;
      }

      #endregion
   }
}

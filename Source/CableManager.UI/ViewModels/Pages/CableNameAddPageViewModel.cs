using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Command;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.CableName;
using CableManager.Repository.CablePrice;
using CableManager.Repository.Models;
using CableManager.Services.Offer;

namespace CableManager.UI.ViewModels.Pages
{
   public class CableNameAddPageViewModel : RootViewModel
   {
      private readonly ICableNameRepository _cableNameRepository;

      private readonly ICablePriceRepository _cablePriceRepository;

      private readonly IOfferService _offerService;

      public CableNameAddPageViewModel(LabelProvider labelProvider, ICableNameRepository cableNameRepository, ICablePriceRepository cablePriceRepository,
         IOfferService offerService) : base(labelProvider)
      {
         _cableNameRepository = cableNameRepository;
         _cablePriceRepository = cablePriceRepository;
         _offerService = offerService;

         SaveCableNameCommand = new RelayCommand<object>(SaveCableName);
         LoadCableNamesCommand = new RelayCommand<object>(LoadCableNames);
      }

      public RelayCommand<object> SaveCableNameCommand { get; set; }

      public RelayCommand<object> LoadCableNamesCommand { get; set; }

      #region Private methods

      private void SaveCableName(object parameter)
      {
         CableModel cable = ConvertToModel(parameter);
         ReturnResult result = _cableNameRepository.Save(cable);

         StatusMessage = result.Message;
      }

      private void LoadCableNames(object parameter)
      {
         var openFileDialog = new OpenFileDialog
         {
            Title = "Select file with cable name definitions",
            Filter = "Cable name files (*.XLSX)|*.XLSX"
         };

         bool? isSuccess = openFileDialog.ShowDialog();

         if (isSuccess != null && isSuccess.Value)
         {
            List<CableModel> cableNames = _offerService.LoadCableNames(openFileDialog.FileName);
            ReturnResult result = _cableNameRepository.SaveAll(cableNames);

            List<CablePriceDbModel> cablePriceDbModels = _cablePriceRepository.GetAll();
            List<List<string>> searchCriteriaList = CreateSearchCriteria(cableNames);

            UpdatePriceModels(cablePriceDbModels, searchCriteriaList);

            _cablePriceRepository.SaveAll(cablePriceDbModels);

            StatusMessage = result.Message;
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

      private void UpdatePriceModels(List<CablePriceDbModel> cablePriceDbModels, List<List<string>> searchCriteriaList)
      {
         foreach (CablePriceDbModel priceModel in cablePriceDbModels)
         {
            foreach (List<string> searchCriteria in searchCriteriaList)
            {
               var intersect = priceModel.CableNames.Intersect(searchCriteria);

               if (intersect.Any())
               {
                  priceModel.CableNames.AddRange(searchCriteria);
                  priceModel.CableNames = priceModel.CableNames.Distinct().ToList();
                  break;
               }
            }
         }
      }

      private CableModel ConvertToModel(object parameter)
      {
         var cableParameters = (object[])parameter;
         var name = cableParameters[0] as string;
         var synonyms = cableParameters[1] as string;

         var cableModel = new CableModel(name, synonyms);

         return cableModel;
      }

      #endregion

   }
}

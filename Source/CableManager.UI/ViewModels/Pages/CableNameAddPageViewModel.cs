﻿using System.Collections.Generic;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Command;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.CableName;
using CableManager.Repository.Models;
using CableManager.Services.Offer;

namespace CableManager.UI.ViewModels.Pages
{
   public class CableNameAddPageViewModel : RootViewModel
   {
      private readonly ICableNameRepository _cableNameRepository;

      private readonly IOfferService _offerService;

      public CableNameAddPageViewModel(LabelProvider labelProvider, ICableNameRepository cableNameRepository, IOfferService offerService)
         : base(labelProvider)
      {
         _cableNameRepository = cableNameRepository;
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

            StatusMessage = result.Message;
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

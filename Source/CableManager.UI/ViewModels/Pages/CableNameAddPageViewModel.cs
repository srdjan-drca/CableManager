﻿using System.Collections.Generic;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Command;
using CableManager.Common.Result;
using CableManager.Localization;
using CableManager.Repository.Cable;
using CableManager.Repository.Models;
using CableManager.Services.DocumentLoaders.CableName;

namespace CableManager.UI.ViewModels.Pages
{
   public class CableNameAddPageViewModel : RootViewModel
   {
      private readonly ICableRepository _cableRepository;

      private readonly ICableNameLoader _cableNameLoader;

      public CableNameAddPageViewModel(LabelProvider labelProvider, ICableRepository cableRepository, ICableNameLoader cableNameLoader) : base(labelProvider)
      {
         _cableRepository = cableRepository;
         _cableNameLoader = cableNameLoader;

         SaveCableNameCommand = new RelayCommand<object>(SaveCableName);
         LoadCableNamesCommand = new RelayCommand<object>(LoadCableNames);
      }

      public RelayCommand<object> SaveCableNameCommand { get; set; }

      public RelayCommand<object> LoadCableNamesCommand { get; set; }

      #region Private methods

      private void SaveCableName(object parameter)
      {
         CableModel cable = ConvertToModel(parameter);
         ReturnResult result = _cableRepository.Save(cable);

         StatusMessage = result.Message;
      }

      private void LoadCableNames(object parameter)
      {
         var openFileDialog = new OpenFileDialog
         {
            Title = "Select file with cable name definitions",
            Filter = "Cable name files (*.XLS;*.XLSX)|*.XLS;*.XLSX"
         };

         bool? isSuccess = openFileDialog.ShowDialog();

         if (isSuccess != null && isSuccess.Value)
         {
            List<CableModel> cables = _cableNameLoader.Load(openFileDialog.FileName);
            ReturnResult result = _cableRepository.SaveAll(cables);

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
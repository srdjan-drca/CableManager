using QLicense;
using GalaSoft.MvvmLight.Command;
using CableManager.Localization;
using CableManager.Common.Result;
using CableManager.Services.License;
using CableManager.UI.Notification;

namespace CableManager.UI.ViewModels.Windows
{
   public class LicenseWindowViewModel : RootViewModel
   {
      private readonly ILicenseService _licenseService;

      private string _licenseKey = string.Empty;

      public LicenseWindowViewModel(LabelProvider labelProvider, ILicenseService licenseService) : base(labelProvider)
      {
         _licenseService = licenseService;

         SubmitLicenseKeyCommand = new RelayCommand<object>(SubmitLicenseKey);
      }

      public string UniqueId => HardwareInfo.GenerateUID("CableManager");

      public string LicenseKey
      {
         get { return _licenseKey; }
         set
         {
            _licenseKey = value;
            RaisePropertyChanged(nameof(LicenseKey));
         }
      }

      public RelayCommand<object> SubmitLicenseKeyCommand { get; set; }

      #region Private methods

      private void SubmitLicenseKey(object parameter)
      {
         ReturnResult result = _licenseService.ValidateLicenseKey(LicenseKey);
         var message = new Message(result.IsSuccess.ToString(), MessageType.LicenseActivation);

         StatusMessage = result.Message;

         MessengerInstance.Send(message);
      }

      #endregion
   }
}

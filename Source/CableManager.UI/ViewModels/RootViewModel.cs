using CableManager.Localization;
using CableManager.Repository.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CableManager.UI.ViewModels
{
   public abstract class RootViewModel : ViewModelBase
   {
      private string _statusMessage;

      private UserModel _currentUser;

      protected RootViewModel(LabelProvider labelProvider)
      {
         LabelProvider = labelProvider;

         ClearStatusMessageCommand = new RelayCommand<object>(ClearStatusMessage);
      }

      public string StatusMessage
      {
         get { return _statusMessage; }
         set
         {
            _statusMessage = value;
            RaisePropertyChanged(nameof(StatusMessage));
         }
      }

      public LabelProvider LabelProvider { get; }

      public UserModel CurrentUser
      {
         get
         {
            return _currentUser;
         }
         set
         {
            _currentUser = value;
            RaisePropertyChanged(nameof(CurrentUser));
         }
      }

      public RelayCommand<object> ClearStatusMessageCommand { get; set; }

      private void ClearStatusMessage(object parameter)
      {
         StatusMessage = string.Empty;
      }
   }
}
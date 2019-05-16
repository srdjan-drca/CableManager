using System.Linq;
using System.Collections.Generic;
using CableManager.Localization;
using CableManager.UI.Helpers.DragDrop;
using CableManager.UI.Notification;

namespace CableManager.UI.ViewModels.Controls
{
   public class DragDropBoxViewModel : RootViewModel, IFileDragDropTarget
   {
      public DragDropBoxViewModel(LabelProvider labelProvider) : base(labelProvider)
      {
         CustomerRequestFiles = new List<string>();
      }

      public List<string> CustomerRequestFiles { get; set; }

      public void OnFileDrop(string[] filePaths)
      {
         CustomerRequestFiles.AddRange(filePaths);

         var message = new Message(filePaths.FirstOrDefault(), MessageType.CustomerRequest);

         MessengerInstance.Send(message);
      }
   }
}
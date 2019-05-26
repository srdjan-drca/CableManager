using System.IO;
using System.Linq;
using CableManager.Localization;
using CableManager.UI.Helpers.DragDrop;
using CableManager.UI.Notification;

namespace CableManager.UI.ViewModels.Controls
{
   public class DragDropBoxViewModel : RootViewModel, IFileDragDropTarget
   {
      public DragDropBoxViewModel(LabelProvider labelProvider) : base(labelProvider)
      {
      }

      public void OnFileDrop(string[] filePaths)
      {
         string customerRequestFile = filePaths.FirstOrDefault();
         string extension = Path.GetExtension(customerRequestFile);
         string recordId = extension == ".xlsx" ? customerRequestFile : string.Empty;
         Message message = new Message(recordId, MessageType.CustomerRequest);

         MessengerInstance.Send(message);
      }
   }
}
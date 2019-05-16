namespace CableManager.UI.Notification
{
   public class Message
   {
      public string RecordId { get; private set; }

      public MessageType Type { get; private set; }

      public Message(string recordId, MessageType messageType)
      {
         RecordId = recordId;
         Type = messageType;
      }
   }
}

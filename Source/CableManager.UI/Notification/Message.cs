namespace CableManager.UI.Notification
{
   public class Message
   {
      public object RecordId { get; private set; }

      public MessageType Type { get; private set; }

      public Message(object recordId, MessageType messageType)
      {
         RecordId = recordId;
         Type = messageType;
      }
   }
}

namespace CableManager.Repository.Models
{
   public class OfferDocumentModel
   {
      public string Id { get; set; }

      public string Name { get; set; }

      public string Path { get; set; }

      public string Date { get; set; }

      public OfferDocumentModel()
      {
      }

      public OfferDocumentModel(string name, string path, string date)
      {
         Name = name;
         Path = path;
         Date = date;
      }
   }
}
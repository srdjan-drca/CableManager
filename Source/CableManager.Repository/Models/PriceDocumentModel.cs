namespace CableManager.Repository.Models
{
   public class PriceDocumentModel
   {
      public string Id { get; set; }

      public string Name { get; set; }

      public string Path { get; set; }

      public string Date { get; set; }

      public bool IsSelected { get; set; }

      public PriceDocumentModel(string name, string path, string date)
      {
         Name = name;
         Path = path;
         Date = date;
      }

      public PriceDocumentModel(string id, string name, string path, string date, bool isSelected)
         : this(name, path, date)
      {
         Id = id;
         IsSelected = isSelected;
      }
   }
}
namespace CableManager.Repository.Models
{
   public class CableModel
   {
      public string Id;

      public string Name { get; set; }

      public string Synonyms { get; set; }

      public CableModel(string name, string synonyms)
      {
         Name = name;
         Synonyms = synonyms;
      }

      public CableModel(string id, string name, string synonyms) : this(name, synonyms)
      {
         Id = id;
      }
   }
}

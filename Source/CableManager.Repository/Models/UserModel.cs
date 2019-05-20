namespace CableManager.Repository.Models
{
   public class UserModel
   {
      public string Id { get; set; }

      public string Number { get; set; }

      public string Name { get; set; }

      public string Password { get; set; }

      public string FirstName { get; set; }

      public string LastName { get; set; }

      public string LastOfferNumber { get; set; }

      public UserModel()
      {
      }

      public UserModel(string name)
      {
         Name = name;
      }

      public UserModel(string name, string password)
      {
         Name = name;
         Password = password;
      }
   }
}
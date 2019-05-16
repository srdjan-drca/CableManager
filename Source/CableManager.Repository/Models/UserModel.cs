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

      public UserModel(string name)
      {
         Name = name;
      }

      public UserModel(string name, string password)
      {
         Name = name;
         Password = password;
      }

      public UserModel(string id, string number, string name, string password, string firstName, string lastName) : this(name, password)
      {
         Id = id;
         Number = number;
         FirstName = firstName;
         LastName = lastName;
      }
   }
}
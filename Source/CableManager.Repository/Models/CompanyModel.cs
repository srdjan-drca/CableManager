﻿using System.Collections.Generic;

namespace CableManager.Repository.Models
{
   public class CompanyModel
   {
      public string Id { get; set; }

      public string Name { get; set; }

      public string Street { get; set; }

      public string City { get; set; }

      public string TaxNumber { get; set; }

      public string Phone1 { get; set; }

      public string Phone2 { get; set; }

      public string Fax { get; set; }

      public string Mobile { get; set; }

      public string Email { get; set; }

      public string LogoPath { get; set; }

      public List<string> BankAccounts { get; set; }

      public CompanyModel()
      {
         BankAccounts = new List<string>();
      }
   }
}

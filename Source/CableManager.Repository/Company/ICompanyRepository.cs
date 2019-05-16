using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Repository.Models;

namespace CableManager.Repository.Company
{
   public interface ICompanyRepository
   {
      ReturnResult Save(CompanyModel company);

      List<CompanyModel> GetAll();
   }
}

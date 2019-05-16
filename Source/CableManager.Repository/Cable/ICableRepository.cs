﻿using System.Collections.Generic;
using CableManager.Common.Result;
using CableManager.Repository.Models;

namespace CableManager.Repository.Cable
{
   public interface ICableRepository
   {
      ReturnResult Save(CableModel cable);

      ReturnResult SaveAll(List<CableModel> cables);

      CableModel Get(string cableId);

      List<CableModel> GetAll();

      ReturnResult DeleteAll(List<string> cableIds);
   }
}

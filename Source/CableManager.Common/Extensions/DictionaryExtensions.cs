using System;
using System.Collections.Generic;

namespace CableManager.Common.Extensions
{
   public static class DictionaryExtensions
   {
      public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> collection)
      {
         if (collection == null)
         {
            throw new ArgumentNullException("Collection is null");
         }

         foreach (var item in collection)
         {
            if (!source.ContainsKey(item.Key))
            {
               source.Add(item.Key, item.Value);
            }
            else
            {
               // handle duplicate key issue here
            }
         }
      }
   }
}

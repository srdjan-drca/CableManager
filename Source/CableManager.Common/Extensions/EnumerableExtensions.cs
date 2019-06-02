using System;
using System.Collections.Generic;
using System.Linq;

namespace CableManager.Common.Extensions
{
   public static class EnumerableExtensions
   {
      public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
      {
         return new HashSet<T>(source);
      }

      public static HashSet<TResult> ToHashSet<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
      {
         return new HashSet<TResult>(source.Select(selector));
      }

      public static IEnumerable<List<T>> BatchBy<T>(this IEnumerable<T> enumerable, int batchSize)
      {
         using (var enumerator = enumerable.GetEnumerator())
         {
            List<T> list = null;
            while (enumerator.MoveNext())
            {
               if (list == null)
               {
                  list = new List<T> { enumerator.Current };
               }
               else if (list.Count < batchSize)
               {
                  list.Add(enumerator.Current);
               }
               else
               {
                  yield return list;
                  list = new List<T> { enumerator.Current };
               }
            }

            if (list?.Count > 0)
            {
               yield return list;
            }
         }
      }
   }
}

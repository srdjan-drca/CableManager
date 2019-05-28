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
   }
}

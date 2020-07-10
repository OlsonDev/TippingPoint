using System;
using System.Collections.Generic;

namespace TippingPoint.Extensions {
  public static class ListExtensions {
		public static IEnumerable<T> EnumerateRange<T>(this IList<T> list, Range range) {
      var count = list.Count;
      if (count == 0) yield break;
      var start = range.Start.GetOffset(count);
      var end = range.End.GetOffset(count);
      for (var i = start; i < end; i++)
        yield return list[i];
		}
	}
}
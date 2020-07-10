using System;
using System.Collections.Generic;
using System.Linq;

namespace TippingPoint.Extensions {
	public static class EnumerableExtensions {
		/// <summary>
		/// Efficiently converts an enumerable to a read-only list. Use this to avoid warnings
		/// about multiple enumerations; however, use IEnumerable<T> to communicate your API
		/// only wants to enumerate an argument/property.
		/// </summary>
		public static IReadOnlyList<T> ToReadOnlyListFast<T>(this IEnumerable<T> enumerable)
			=> enumerable as IReadOnlyList<T> ?? enumerable?.ToList() ?? (IReadOnlyList<T>)Array.Empty<T>();

		/// <summary>Groups an enumerable into batches of a given size.</summary>
		public static IEnumerable<IReadOnlyList<T>> InBatchesOf<T>(this IEnumerable<T> enumerable, int batchSize) {
			var batch = new List<T>(batchSize);
			foreach (var item in enumerable) {
				batch.Add(item);
				if (batch.Count < batchSize) continue;
				yield return batch;
				batch = new List<T>(batchSize);
			}

			if (batch.Count > 0) yield return batch;
		}
	}
}
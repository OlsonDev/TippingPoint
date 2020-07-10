using System;
using System.Collections.Generic;

namespace TippingPoint.Extensions {
  public static class RangeExtensions {
    /// <summary>Groups a range into batches of a given size.</summary>
    public static IEnumerable<Range> InBatchesOf(this Range range, int batchSize, int collectionSize) {
      var start = range.Start.GetOffset(collectionSize);
      var end = range.End.GetOffset(collectionSize);
      var batchEnd = Math.Min(start + batchSize, end);
      while (batchEnd < end) {
        yield return new Range(start, batchEnd);
        start = batchEnd;
        batchEnd = Math.Min(batchEnd + batchSize, end);
      }
      yield return new Range(start, batchEnd);
    }
  }
}
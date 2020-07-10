using System;

namespace TippingPoint.Extensions {
  public static class GuidExtensions {
    private static readonly int[] ByteOrder = new[] { 15, 14, 13, 12, 11, 10, 9, 8, 6, 7, 4, 5, 0, 1, 2, 3 };
    public static Guid Increment(this Guid guid) {
      var bytes = guid.ToByteArray();
      var carry = true;
      for (var i = 0; i < ByteOrder.Length && carry; i++) {
        var index = ByteOrder[i];
        var oldValue = bytes[index]++;
        carry = oldValue > bytes[index];
      }
      return new Guid(bytes);
    }
  }
}
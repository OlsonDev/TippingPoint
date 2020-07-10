using System;
using System.Collections.Generic;

namespace TippingPoint.Extensions {
  public static class RandomExtensions {
    private static readonly string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    private static readonly string Numeric = "1234567890";
    private static readonly string SpecialCharacters = "_-";

    public static string NextUsername(this Random random) {
      var chars = new List<char>();
      InsertPasswordCharacters(random, chars, 1, 3, Uppercase);
      InsertPasswordCharacters(random, chars, 6, 10, Lowercase);
      InsertPasswordCharacters(random, chars, 0, 2, Numeric);
      InsertPasswordCharacters(random, chars, 0, 1, SpecialCharacters);
      return new string(chars.ToArray());
    }

    private static void InsertPasswordCharacters(
      Random random,
      IList<char> chars,
      int countMin,
      int countMax,
      string alphabet) {
      var count = random.Next(countMin, countMax);
      for (var i = 0; i < count; i++)
        chars.Insert(random.Next(0, chars.Count), alphabet[random.Next(0, alphabet.Length)]);
    }
  }
}
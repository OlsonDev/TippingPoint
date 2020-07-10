using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Dapper;
using TippingPoint.Attributes;

namespace TippingPoint.Extensions {
  public static class DapperExtensions {
    private static readonly Type TvpColumnAttributeType = typeof(TvpColumnAttribute);
    private static readonly Type IntType = typeof(int);
    private static readonly Dictionary<(Type, string), TvpTypeCache> Cache = new Dictionary<(Type, string), TvpTypeCache>(new CacheComparer());
    private static readonly BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

    public static SqlMapper.ICustomQueryParameter AsTableValuedParameter<T>(this IEnumerable<T> enumerable, string tvpName) {
      var cache = GetTvpTypeCache<T>(tvpName);
      var count = cache.ColumnCount;
      var names = cache.ColumnNames;
      var types = cache.ColumnTypes;
      var getters = cache.Getters;

      var table = new DataTable();
      var columns = table.Columns;
      for (var i = 0; i < count; i++) {
        columns.Add(names[i], types[i]);
      }
      var rows = table.Rows;
      foreach (var obj in enumerable) {
        rows.Add(getters.Select(g => g(obj) ?? DBNull.Value).ToArray());
      }

      return table.AsTableValuedParameter(tvpName);
    }

    public static void PrepTvpTypeCache<T>(string tvpName)
      => GetTvpTypeCache<T>(tvpName);

    private static TvpTypeCache GetTvpTypeCache<T>(string tvpName) {
      var type = typeof(T);
      var key = (type, tvpName);
      if (Cache.TryGetValue(key, out var value)) return value;
      var props = GetOrderedProperties<T>(tvpName);
      var names = new List<string>();
      var types = new List<Type>();
      var getters = new List<Func<object?, object?>>();
      foreach (var prop in props) {
        names.Add(prop.Name);
        var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        types.Add(propType.IsEnum ? IntType : propType);
        getters.Add(prop.GetValue);
      }

      value = new TvpTypeCache(names.AsReadOnly(), types.AsReadOnly(), getters.AsReadOnly());
      Cache[key] = value;
      return value;
    }

    private static IEnumerable<PropertyInfo> GetOrderedProperties<T>(string tvpName) {
      var type = typeof(T);
      // Cannot put attributes on anonymous types; so hopefully the order matches the TVP.
      if (type.IsAnonymousType()) return type.GetProperties(PublicInstance);
      // Otherwise, return attributed properties in the desired order.
      return type
        .GetProperties(PublicInstance)
        .Select(prop => new {
          prop,
          tvpColumn = prop
            .GetCustomAttributes(TvpColumnAttributeType)
            .Cast<TvpColumnAttribute>()
            .Where(tvpColumn => string.Equals(tvpName, tvpColumn.TvpName))
            .SingleOrDefault()
        })
        .Where(o => !(o.tvpColumn is null))
        .OrderBy(o => o.tvpColumn!.Order)
        .ThenBy(o => o.prop.Name)
        .Select(o => o.prop);
    }

    private class CacheComparer : IEqualityComparer<(Type, string)> {
      public bool Equals([AllowNull] (Type, string) x, [AllowNull] (Type, string) y) {
        if (x.Item1 != y.Item1) return false;
        return string.Equals(x.Item2, y.Item2, StringComparison.OrdinalIgnoreCase);
      }

      public int GetHashCode([DisallowNull] (Type, string) obj) {
        var hash1 = obj.Item1.GetHashCode();
        var hash2 = obj.Item2.GetHashCode();
        unchecked { return (hash1 * 397) ^ hash2; }
      }
    }

    private class TvpTypeCache {
      public int ColumnCount { get; }
      public IList<string> ColumnNames { get; }
      public IList<Type> ColumnTypes { get; }
      public IList<Func<object?, object?>> Getters { get; }

      public TvpTypeCache(
        IList<string> columnNames,
        IList<Type> columnTypes,
        IList<Func<object?, object?>> getters) {
        ColumnCount = columnNames.Count;
        ColumnNames = columnNames;
        ColumnTypes = columnTypes;
        Getters = getters;
      }
    }
  }
}
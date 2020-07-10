using System;

namespace TippingPoint.Attributes {
  [AttributeUsage(AttributeTargets.Property)]
  public class TvpColumnAttribute : Attribute {
    public string TvpName { get; }
    public int Order { get; }

    public TvpColumnAttribute(string tvpName, int order)
    {
        TvpName = tvpName;
        Order = order;
    }
  }
}
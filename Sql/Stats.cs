namespace TippingPoint.Sql {
  public static class Stats {
    public static readonly string On = @"
SET STATISTICS IO   ON;
SET STATISTICS TIME ON;
    ";
    public static readonly string Off = @"
SET STATISTICS IO   OFF;
SET STATISTICS TIME OFF;
    ";
  }
}
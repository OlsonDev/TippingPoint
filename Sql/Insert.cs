namespace TippingPoint.Sql {
  public static class Insert {
    private static readonly string Audit = @"
  , DateCreated
  , DateModified
  , UserCreated
  , UserModified";

    public static readonly string FoosFromTvp = $@"
INSERT dbo.Foo (
    FooID
  , FooDatum1
  , FooDatum2
  , FooDatum3
{Audit}
)
SELECT
    FooID
  , FooDatum1
  , FooDatum2
  , FooDatum3
{Audit}
FROM @Tvp;
    ";

    public static readonly string BarsFromTvp = $@"
INSERT dbo.Bar (
    BarID
  , FooID
  , BarDatum1
  , BarDatum2
  , BarDatum3
{Audit}
)
SELECT
    BarID
  , FooID
  , BarDatum1
  , BarDatum2
  , BarDatum3
{Audit}
FROM @Tvp;
    ";

    public static readonly string QuxsFromTvp = $@"
INSERT dbo.Qux (
    QuxID
  , FooID
  , BarID
  , QuxDatum1
  , QuxDatum2
  , QuxDatum3
{Audit}
)
SELECT
    QuxID
  , FooID
  , BarID
  , QuxDatum1
  , QuxDatum2
  , QuxDatum3
{Audit}
FROM @Tvp;
    ";
  }
}
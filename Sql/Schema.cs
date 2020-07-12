namespace TippingPoint.Sql {
  public static class Schema {
    public static readonly string CreateDb = @"
IF DB_ID('TippingPoint') IS NULL
  CREATE DATABASE TippingPoint;
    ";
    public static readonly string DropAndCreateBase = $@"
DROP TYPE IF EXISTS dbo.QuxTvp;
DROP TYPE IF EXISTS dbo.BarTvp;
DROP TYPE IF EXISTS dbo.FooTvp;

CREATE TYPE dbo.FooTvp AS TABLE (
{Columns.Foo}
);

CREATE TYPE dbo.BarTvp AS TABLE (
{Columns.Bar}
);

CREATE TYPE dbo.QuxTvp AS TABLE (
{Columns.Qux}
);

DROP TABLE IF EXISTS dbo.Qux;
DROP TABLE IF EXISTS dbo.Bar;
DROP TABLE IF EXISTS dbo.Foo;

CREATE TABLE dbo.Foo ( -- IUsers
{Columns.Foo}
  , CONSTRAINT PK_dbo_Foo          PRIMARY KEY (FooID)
);

CREATE TABLE dbo.Bar ( -- Orgs
{Columns.Bar}
  , CONSTRAINT PK_dbo_Bar          PRIMARY KEY (BarID)
  , CONSTRAINT FK_dbo_Bar_dbo_Foo  FOREIGN KEY (FooID)  REFERENCES dbo.Foo (FooID)
);

CREATE TABLE dbo.Qux ( -- NotificationSettings
{Columns.Qux}
  , CONSTRAINT PK_dbo_Qux          PRIMARY KEY (QuxID)
  , CONSTRAINT FK_dbo_Qux_dbo_Foo  FOREIGN KEY (FooID)  REFERENCES dbo.Foo (FooID)
  , CONSTRAINT FK_dbo_Qux_dbo_Bar  FOREIGN KEY (BarID)  REFERENCES dbo.Bar (BarID)
);

CREATE INDEX IX_dbo_Bar_FooID
  ON dbo.Bar (FooID) INCLUDE (BarDatum1, BarDatum2, BarDatum3);
    ";
  }
}
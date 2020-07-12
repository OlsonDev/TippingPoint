namespace TippingPoint.Sql {
  public static class Columns {
    public static readonly string Foo = @"
	  FooID               UNIQUEIDENTIFIER  NOT NULL -- ID
	, FooDatum1           INT               NOT NULL
	, FooDatum2           INT               NOT NULL
	, FooDatum3           INT                   NULL
	, DateCreated         DATETIME2(7)      NOT NULL
	, DateModified        DATETIME2(7)      NOT NULL
	, UserCreated         NVARCHAR(256)     NOT NULL
	, UserModified        NVARCHAR(256)         NULL
    ";

    public static readonly string Bar = @"
	  BarID               INT               NOT NULL
	, FooID               UNIQUEIDENTIFIER  NOT NULL -- OwnerID
	, BarDatum1           INT               NOT NULL
	, BarDatum2           INT               NOT NULL
	, BarDatum3           INT                   NULL
	, DateCreated         DATETIME2(7)      NOT NULL
	, DateModified        DATETIME2(7)      NOT NULL
	, UserCreated         NVARCHAR(256)     NOT NULL
	, UserModified        NVARCHAR(256)         NULL
    ";

    public static readonly string Qux = @"
	  QuxID               UNIQUEIDENTIFIER  NOT NULL
	, FooID               UNIQUEIDENTIFIER  NOT NULL -- UserID
	, BarID               INT                   NULL -- OrgID
	, QuxDatum1           INT               NOT NULL
	, QuxDatum2           INT               NOT NULL
	, QuxDatum3           INT                   NULL
	, DateCreated         DATETIME2(7)      NOT NULL
	, DateModified        DATETIME2(7)      NOT NULL
	, UserCreated         NVARCHAR(256)     NOT NULL
	, UserModified        NVARCHAR(256)         NULL
    ";
  }
}
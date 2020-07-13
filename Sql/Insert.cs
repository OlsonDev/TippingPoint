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

    public static readonly string ResultsFromTvps = @"
INSERT Benchmark.TableIteration (
    IterationNumber
  , TableName
  , [RowCount]
)
SELECT
    TI.IterationNumber
  , TI.TableName
  , TI.[RowCount]
FROM @TableIterationTvp AS TI;


INSERT Benchmark.IndexClass (
    Name
  , Command
)
SELECT DISTINCT
    FR.IndexClassName
  , FR.IndexClassCommand
FROM @FlatResultTvp AS FR;


INSERT Benchmark.QueryClass (
    Name
  , Command
)
SELECT DISTINCT
    FR.QueryClassName
  , FR.QueryClassCommand
FROM @FlatResultTvp AS FR;


INSERT Benchmark.Sample (
    IndexClassID
  , QueryClassID
  , IterationNumber
  , SampleNumber
  , HitOrMiss
  , [RowCount]
  , StopwatchTicks
  , ExecutionCpuMs
  , ExecutionElapsedMs
  , ParseAndCompileCpuMs
  , ParseAndCompileElapsedMs
)
SELECT
    IC.IndexClassID
  , QC.QueryClassID
  , FR.IterationNumber
  , FR.SampleNumber
  , FR.HitOrMiss
  , FR.[RowCount]
  , FR.StopwatchTicks
  , FR.ExecutionCpuMs
  , FR.ExecutionElapsedMs
  , FR.ParseAndCompileCpuMs
  , FR.ParseAndCompileElapsedMs
FROM @FlatResultTvp        AS FR
JOIN Benchmark.IndexClass  AS IC  ON IC.Name = FR.IndexClassName
JOIN Benchmark.QueryClass  AS QC  ON QC.Name = FR.QueryClassName;


INSERT Benchmark.SampleTableIo (
    SampleID
  , TableName
  , ScanCount
  , LogicalReads
  , PhysicalReads
  , PageServerReads
  , ReadAheadReads
  , PageServerReadAheadReads
  , LobLogicalReads
  , LobPhysicalReads
  , LobPageServerReads
  , LobReadAheadReads
  , LobPageServerReadAheadReads
)
SELECT
    S.SampleID
  , IO.TableName
  , IO.ScanCount
  , IO.LogicalReads
  , IO.PhysicalReads
  , IO.PageServerReads
  , IO.ReadAheadReads
  , IO.PageServerReadAheadReads
  , IO.LobLogicalReads
  , IO.LobPhysicalReads
  , IO.LobPageServerReads
  , IO.LobReadAheadReads
  , IO.LobPageServerReadAheadReads
FROM @TableIoTvp           AS IO
JOIN Benchmark.IndexClass  AS IC  ON IC.Name            = IO.IndexClassName
JOIN Benchmark.QueryClass  AS QC  ON QC.Name            = IO.QueryClassName
JOIN Benchmark.Sample      AS S   ON S.IndexClassID     = IC.IndexClassID
                                 AND S.QueryClassID     = QC.QueryClassID
                                 AND S.IterationNumber  = IO.IterationNumber
                                 AND S.SampleNumber     = IO.SampleNumber
                                 AND S.HitOrMiss        = IO.HitOrMiss;
    ";
  }
}
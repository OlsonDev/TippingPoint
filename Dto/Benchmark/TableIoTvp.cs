using TippingPoint.Attributes;
using TippingPoint.Benchmark.Telemetry;
using TippingPoint.SqlStatistics;

namespace TippingPoint.Dto.Benchmark {
  public class TableIoTvp {
    [TvpColumn("Benchmark.TableIoTvp", 1)]
    public string IndexClassName { get; }

    [TvpColumn("Benchmark.TableIoTvp", 2)]
    public string QueryClassName { get; }

    [TvpColumn("Benchmark.TableIoTvp", 3)]
    public int IterationNumber { get; }

    [TvpColumn("Benchmark.TableIoTvp", 4)]
    public int SampleNumber { get; }

    [TvpColumn("Benchmark.TableIoTvp", 5)]
    public string HitOrMiss { get; }

    [TvpColumn("Benchmark.TableIoTvp", 6)]
    public string TableName { get; }

    [TvpColumn("Benchmark.TableIoTvp", 7)]
    public int? ScanCount { get; }

    [TvpColumn("Benchmark.TableIoTvp", 8)]
    public int? LogicalReads { get; }

    [TvpColumn("Benchmark.TableIoTvp", 9)]
    public int? PhysicalReads { get; }

    [TvpColumn("Benchmark.TableIoTvp", 10)]
    public int? PageServerReads { get; }

    [TvpColumn("Benchmark.TableIoTvp", 11)]
    public int? ReadAheadReads { get; }

    [TvpColumn("Benchmark.TableIoTvp", 12)]
    public int? PageServerReadAheadReads { get; }

    [TvpColumn("Benchmark.TableIoTvp", 13)]
    public int? LobLogicalReads { get; }

    [TvpColumn("Benchmark.TableIoTvp", 14)]
    public int? LobPhysicalReads { get; }

    [TvpColumn("Benchmark.TableIoTvp", 15)]
    public int? LobPageServerReads { get; }

    [TvpColumn("Benchmark.TableIoTvp", 16)]
    public int? LobReadAheadReads { get; }

    [TvpColumn("Benchmark.TableIoTvp", 17)]
    public int? LobPageServerReadAheadReads { get; }

    public TableIoTvp(
      IndexBenchmarkBase indexBenchmark,
      QueryBenchmarkBase queryBenchmark,
      int iterationNumber,
      int sampleNumber,
      TableIo tableIo,
      string hitOrMiss) {
      IndexClassName = indexBenchmark.Name;
      QueryClassName = queryBenchmark.Name;
      IterationNumber = iterationNumber;
      SampleNumber = sampleNumber;
      HitOrMiss = hitOrMiss;
      TableName = tableIo.TableName;
      ScanCount = tableIo.ScanCount;
      LogicalReads = tableIo.LogicalReads;
      PhysicalReads = tableIo.PhysicalReads;
      PageServerReads = tableIo.PageServerReads;
      ReadAheadReads = tableIo.ReadAheadReads;
      PageServerReadAheadReads = tableIo.PageServerReadAheadReads;
      LobLogicalReads = tableIo.LobLogicalReads;
      LobPhysicalReads = tableIo.LobPhysicalReads;
      LobPageServerReads = tableIo.LobPageServerReads;
      LobReadAheadReads = tableIo.LobReadAheadReads;
      LobPageServerReadAheadReads = tableIo.LobPageServerReadAheadReads;
    }
  }
}
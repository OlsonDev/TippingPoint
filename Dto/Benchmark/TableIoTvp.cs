using TippingPoint.Attributes;

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
      string indexClassName,
      string queryClassName,
      int iterationNumber,
      int sampleNumber,
      string hitOrMiss,
      string tableName,
      int? scanCount,
      int? logicalReads,
      int? physicalReads,
      int? pageServerReads,
      int? readAheadReads,
      int? pageServerReadAheadReads,
      int? lobLogicalReads,
      int? lobPhysicalReads,
      int? lobPageServerReads,
      int? lobReadAheadReads,
      int? lobPageServerReadAheadReads) {
      IndexClassName = indexClassName;
      QueryClassName = queryClassName;
      IterationNumber = iterationNumber;
      SampleNumber = sampleNumber;
      HitOrMiss = hitOrMiss;
      TableName = tableName;
      ScanCount = scanCount;
      LogicalReads = logicalReads;
      PhysicalReads = physicalReads;
      PageServerReads = pageServerReads;
      ReadAheadReads = readAheadReads;
      PageServerReadAheadReads = pageServerReadAheadReads;
      LobLogicalReads = lobLogicalReads;
      LobPhysicalReads = lobPhysicalReads;
      LobPageServerReads = lobPageServerReads;
      LobReadAheadReads = lobReadAheadReads;
      LobPageServerReadAheadReads = lobPageServerReadAheadReads;
    }
  }
}
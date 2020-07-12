using TippingPoint.Attributes;

namespace TippingPoint.Dto.Benchmark {
  public class FlatResultTvp {
    [TvpColumn("Benchmark.FlatResultTvp", 1)]
    public string IndexClassName { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 2)]
    public string QueryClassName { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 3)]
    public int IterationNumber { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 4)]
    public int SampleNumber { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 5)]
    public string HitOrMiss { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 6)]
    public long RowCount { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 7)]
    public long ExecutionCpuMs { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 8)]
    public long ExecutionElapsedMs { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 9)]
    public long ParseAndCompileCpuMs { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 10)]
    public long ParseAndCompileElapsedMs { get; }

    public FlatResultTvp(
      string indexClassName,
      string queryClassName,
      int iterationNumber,
      int sampleNumber,
      string hitOrMiss,
      long rowCount,
      long executionCpuMs,
      long executionElapsedMs,
      long parseAndCompileCpuMs) {
      IndexClassName = indexClassName;
      QueryClassName = queryClassName;
      IterationNumber = iterationNumber;
      SampleNumber = sampleNumber;
      HitOrMiss = hitOrMiss;
      RowCount = rowCount;
      ExecutionCpuMs = executionCpuMs;
      ExecutionElapsedMs = executionElapsedMs;
      ParseAndCompileCpuMs = parseAndCompileCpuMs;
    }
  }
}
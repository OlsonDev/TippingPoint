using TippingPoint.Attributes;
using TippingPoint.Benchmark.Telemetry;

namespace TippingPoint.Dto.Benchmark {
  public class FlatResultTvp {
    [TvpColumn("Benchmark.FlatResultTvp", 1)]
    public string IndexClassName { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 2)]
    public string IndexClassCommand { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 3)]
    public string QueryClassName { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 4)]
    public string QueryClassCommand { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 5)]
    public int IterationNumber { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 6)]
    public int SampleNumber { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 7)]
    public string HitOrMiss { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 8)]
    public long RowCount { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 9)]
    public long StopwatchTicks { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 10)]
    public long ExecutionCpuMs { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 11)]
    public long ExecutionElapsedMs { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 12)]
    public long ParseAndCompileCpuMs { get; }

    [TvpColumn("Benchmark.FlatResultTvp", 13)]
    public long ParseAndCompileElapsedMs { get; }

    public FlatResultTvp(
      IndexBenchmarkBase indexBenchmark,
      QueryBenchmarkBase queryBenchmark,
      SampleBenchmark sampleBenchmark,
      int iterationNumber,
      int sampleNumber,
      string hitOrMiss) {
      var timeStats = sampleBenchmark.Statistics.Time;
      IndexClassName = indexBenchmark.Name;
      IndexClassCommand = indexBenchmark.Command;
      QueryClassName = queryBenchmark.Name;
      QueryClassCommand = queryBenchmark.Command;
      IterationNumber = iterationNumber;
      SampleNumber = sampleNumber;
      HitOrMiss = hitOrMiss;
      RowCount = sampleBenchmark.RowCount;
      StopwatchTicks = sampleBenchmark.StopwatchTicks;
      ExecutionCpuMs = timeStats!.ExecutionTime!.CpuMs;
      ExecutionElapsedMs = timeStats!.ExecutionTime!.ElapsedMs;
      ParseAndCompileCpuMs = timeStats!.ParseAndCompileTime!.CpuMs;
      ParseAndCompileElapsedMs = timeStats!.ParseAndCompileTime!.ElapsedMs;
    }
  }
}
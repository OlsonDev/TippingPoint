using TippingPoint.SqlStatistics;

namespace TippingPoint.Benchmark.Telemetry {
  // Represents a single query execution.
  public class SampleBenchmark {
    public long StopwatchTicks { get; }
    public int RowCount { get; }
    public Statistics Statistics { get; }
    public SampleBenchmark(long stopwatchTicks, int rowCount, Statistics statistics) {
      StopwatchTicks = stopwatchTicks;
      RowCount = rowCount;
      Statistics = statistics;
    }
  }
}
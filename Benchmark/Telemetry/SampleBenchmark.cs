using TippingPoint.SqlStatistics;

namespace TippingPoint.Benchmark.Telemetry {
  // Represents a single query execution.
  public class SampleBenchmark {
    public long StopwatchTicks { get; }
    public Statistics Statistics { get; }
    public SampleBenchmark(long stopwatchTicks, Statistics statistics) {
      StopwatchTicks = stopwatchTicks;
      Statistics = statistics;
    }
  }
}
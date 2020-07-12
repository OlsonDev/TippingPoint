using System.Collections.Generic;

namespace TippingPoint.Benchmark.Telemetry {
  // Represents several executions of the same query, both hits and misses,
  // with the same amount of seeded data. A hit represents a query which
  // produces rows, whereas a miss represents a query which doesn't.
  public class IterationBenchmark {
    public IList<SampleBenchmark> HitSamples { get; } = new List<SampleBenchmark>(Program.SamplesPerIteration);
    public IList<SampleBenchmark> MissSamples { get; } = new List<SampleBenchmark>(Program.SamplesPerIteration);
  }
}
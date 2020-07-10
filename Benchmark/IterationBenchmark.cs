using System.Collections.Generic;

namespace TippingPoint.Benchmark {
  public class IterationBenchmark {
    public List<SampleBenchmark> HitSamples { get; } = new List<SampleBenchmark>(Program.SamplesPerIteration);
    public List<SampleBenchmark> MissSamples { get; } = new List<SampleBenchmark>(Program.SamplesPerIteration);
  }
}
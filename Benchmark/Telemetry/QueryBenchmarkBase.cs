using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TippingPoint.Sql;
using TippingPoint.SqlStatistics;

namespace TippingPoint.Benchmark.Telemetry {
  // Represents all iterations and their hit-and-miss samples of the same query.
  public abstract class QueryBenchmarkBase {
    // Not thread safe but expect this to be used single-threadedly
    private static readonly Stopwatch Stopwatch = new Stopwatch();
    public string Name => GetType().Name;
    public abstract string Command { get; }
    public IList<IterationBenchmark> IterationBenchmarks { get; } = new List<IterationBenchmark>(Program.Iterations);

    protected abstract Task<int> ExecuteQueryToBenchmarkAsync(SqlConnection connection, DynamicParameters dynamicParameters);

    public async Task ExecuteIterationAsync(
      SqlConnection connection,
      IReadOnlyList<DynamicParameters> hitSampleParameters,
      IReadOnlyList<DynamicParameters> missSampleParameters) {
      var iterationBenchmark = new IterationBenchmark();
      for (var i = 0; i < Program.SamplesPerIteration; i++) {
        var hitSample = await ExecuteSampleAsync(connection, hitSampleParameters[i]);
        var missSample = await ExecuteSampleAsync(connection, missSampleParameters[i]);

        iterationBenchmark.HitSamples.Add(hitSample);
        iterationBenchmark.MissSamples.Add(missSample);
      }
      IterationBenchmarks.Add(iterationBenchmark);
    }

    private async Task<SampleBenchmark> ExecuteSampleAsync(SqlConnection connection, DynamicParameters parameters) {
      await connection.ExecuteAsync(Stats.On);
      var statisticsSource = new TaskCompletionSource<Statistics>();
      var infoMessageHandler = BuildInfoMessageHandler(statisticsSource);
      connection.InfoMessage += infoMessageHandler;
      GC.Collect(1, GCCollectionMode.Forced, true);
      GCSettings.LatencyMode = GCLatencyMode.LowLatency;
      Stopwatch.Restart();
      var rowCount = await ExecuteQueryToBenchmarkAsync(connection, parameters);
      Stopwatch.Stop();
      GCSettings.LatencyMode = GCLatencyMode.Batch;
      var statistics = await statisticsSource.Task;
      connection.InfoMessage -= infoMessageHandler;
      await connection.ExecuteAsync(Stats.Off);
      return new SampleBenchmark(Stopwatch.ElapsedTicks, rowCount, statistics);
    }

    private static SqlInfoMessageEventHandler BuildInfoMessageHandler(TaskCompletionSource<Statistics> source) {
      var statistics = new Statistics();
      return (sender, e) => {
        statistics.HandleInfoMessageEvent(e);
        if (statistics.IsComplete) source.SetResult(statistics);
      };
    }
  }
}
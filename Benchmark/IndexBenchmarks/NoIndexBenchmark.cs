using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TippingPoint.Benchmark.Telemetry;

namespace TippingPoint.Benchmark.IndexBenchmarks {
  public class NoIndexBenchmark : IndexBenchmarkBase {
    public override string Command => @"/* No index here */";
    protected override Task CreateIndexesAsync(SqlConnection connection, SqlTransaction transaction)
      => Task.CompletedTask;
  }
}
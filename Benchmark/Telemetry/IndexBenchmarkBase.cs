using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TippingPoint.Benchmark.QueryBenchmarks;
using TippingPoint.Sql;

namespace TippingPoint.Benchmark.Telemetry {
  public abstract class IndexBenchmarkBase {
    public IReadOnlyList<QueryBenchmarkBase> QueriesToBenchmark { get; } = new List<QueryBenchmarkBase> {
      new FilterBy2ColumnsBenchmark(),
      new FilterBy3ColumnsBenchmark(),
    };

    public string Name => GetType().Name;
    protected abstract Task CreateIndexesAsync(SqlConnection connection, SqlTransaction transaction);

    public async Task TryInitSchemaAsync(string connectionString) {
      using var connection = new SqlConnection(connectionString);
      Console.Write("Connecting to TippingPoint database... ");
      await connection.OpenAsync();
      Console.WriteLine("done!");
      using var transaction = connection.BeginTransaction();
      Console.Write("Initializing TippingPoint schema... ");
      await connection.ExecuteAsync(Schema.DropAndCreateBase, null, transaction);
      await CreateIndexesAsync(connection, transaction);
      await transaction.CommitAsync();
      Console.WriteLine("done!");
    }

    public async Task RunQueriesToBenchmarkAsync(
      SqlConnection connection,
      IReadOnlyList<DynamicParameters> hitSampleParameters,
      IReadOnlyList<DynamicParameters> missSampleParameters) {
      foreach (var queryToBenchmark in QueriesToBenchmark) {
        await queryToBenchmark.ExecuteIterationAsync(connection, hitSampleParameters, missSampleParameters);
      }
    }
  }
}
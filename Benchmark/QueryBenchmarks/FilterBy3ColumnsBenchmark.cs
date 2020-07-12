using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TippingPoint.Benchmark.Telemetry;
using TippingPoint.Dto;

namespace TippingPoint.Benchmark.QueryBenchmarks {
  public class FilterBy3ColumnsBenchmark : QueryBenchmarkBase {
    private const string Command = @"
      SELECT
          Q.QuxID
        , Q.FooID
        , Q.BarID
        , Q.QuxDatum1
        , Q.QuxDatum2
        , Q.QuxDatum3
      FROM (SELECT _ = NULL)  AS _
      JOIN dbo.Qux            AS Q  ON Q.FooID      = @FooID
                                   AND Q.QuxDatum1  = @QuxDatum1
                                   AND Q.BarID      = @BarID;
    ";
    protected override Task ExecuteQueryToBenchmarkAsync(SqlConnection connection, DynamicParameters parameters)
      => connection.QueryAsync<QuxIndexDto>(Command, parameters);
  }
}
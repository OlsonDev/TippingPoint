using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TippingPoint.Benchmark.Telemetry;
using TippingPoint.Dto;

namespace TippingPoint.Benchmark.QueryBenchmarks {
  public class FilterBy2ColumnsBenchmark : QueryBenchmarkBase {
    private const string CommandFast = @"
      SELECT
          Q.QuxID
        , Q.FooID
        , Q.BarID
        , Q.QuxDatum1
        , Q.QuxDatum2
        , Q.QuxDatum3
      FROM (SELECT _ = NULL)  AS _
      JOIN dbo.Qux            AS Q  ON Q.FooID      = @FooID
                                   AND Q.QuxDatum1  = @QuxDatum1;
    ";

    public override string Command => CommandFast;

    protected override async Task<int> ExecuteQueryToBenchmarkAsync(SqlConnection connection, DynamicParameters parameters)
      =>  (await connection.QueryAsync<QuxIndexDto>(CommandFast, parameters)).AsList().Count();
  }
}
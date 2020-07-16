using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TippingPoint.Benchmark.Telemetry;
using TippingPoint.Dto;

namespace TippingPoint.Benchmark.QueryBenchmarks {
  public class FilterByFilteredIndexAnd3ColumnsBenchmark : QueryBenchmarkBase {
    private const string CommandFast = @"
      SELECT
          Q.QuxID
        , Q.FooID
        , Q.BarID
        , Q.QuxDatum1
        , Q.QuxDatum2
        , Q.QuxDatum3
      FROM (SELECT _ = NULL)  AS _
      JOIN dbo.Qux            AS Q  ON Q.QuxDatum3 != 50
                                   AND Q.QuxDatum3 != 60
                                   AND Q.FooID      = @FooID
                                   AND Q.QuxDatum1  = @QuxDatum1
                                   AND Q.BarID      = @BarID;
    ";

    public override string Command => CommandFast;

    protected override async Task<int> ExecuteQueryToBenchmarkAsync(SqlConnection connection, DynamicParameters parameters)
      =>  (await connection.QueryAsync<QuxIndexDto>(CommandFast, parameters)).AsList().Count();
  }
}
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TippingPoint.Benchmark.Telemetry;

namespace TippingPoint.Benchmark.IndexBenchmarks {
  public class Index3Include2Benchmark : IndexBenchmarkBase {
    public override string Command => @"
CREATE INDEX IX_dbo_Qux_FooID_QuxDatum1
  ON dbo.Qux (FooID, QuxDatum1, BarID) INCLUDE (QuxDatum2, QuxDatum3);
    ";
    protected override Task CreateIndexesAsync(SqlConnection connection, SqlTransaction transaction)
      => connection.ExecuteAsync(Command, transaction: transaction);
  }
}
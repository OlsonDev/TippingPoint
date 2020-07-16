using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TippingPoint.Benchmark.Telemetry;

namespace TippingPoint.Benchmark.IndexBenchmarks {
  public class FilteredIndex3Include2Benchmark : IndexBenchmarkBase {
    public override string Command => @"
CREATE INDEX IX_dbo_Qux_FooID_QuxDatum1_BarID
  ON dbo.Qux (FooID, QuxDatum1, BarID) INCLUDE (QuxDatum2, QuxDatum3) WHERE QuxDatum3 != 50 AND QuxDatum3 != 60;
    ";
    protected override Task CreateIndexesAsync(SqlConnection connection, SqlTransaction transaction)
      => connection.ExecuteAsync(Command, transaction: transaction);
  }
}
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TippingPoint.Benchmark.Telemetry;

namespace TippingPoint.Benchmark.IndexBenchmarks {
  public class Index2Include3Benchmark : IndexBenchmarkBase {
    protected override Task CreateIndexesAsync(SqlConnection connection, SqlTransaction transaction) {
      return connection.ExecuteAsync(@"
CREATE INDEX IX_dbo_Qux_FooID_QuxDatum1
  ON dbo.Qux (FooID, QuxDatum1) INCLUDE (BarID, QuxDatum2, QuxDatum3);
      ", transaction: transaction);
    }
  }
}
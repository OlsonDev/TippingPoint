using TippingPoint.Attributes;

namespace TippingPoint.Dto.Benchmark {
  public class TableIterationTvp {
    [TvpColumn("Benchmark.TableIterationTvp", 1)]
    public int IterationNumber { get; }

    [TvpColumn("Benchmark.TableIterationTvp", 2)]
    public string TableName { get; }

    [TvpColumn("Benchmark.TableIterationTvp", 3)]
    public long RowCount { get; }

    public TableIterationTvp(
      int iterationNumber,
      string tableName,
      long rowCount) {
      IterationNumber = iterationNumber;
      TableName = tableName;
      RowCount = rowCount;
    }
  }
}
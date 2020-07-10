using TippingPoint.SqlStatistics;

namespace TippingPoint.Benchmark {
  public class SampleBenchmark {
    /*
      SQL Server parse and compile time:
        CPU time = 0 ms, elapsed time = 2 ms.

      (6 rows affected)
      Table 'Worktable'. Scan count 0, logical reads 0, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.
      Table 'Workfile'. Scan count 0, logical reads 0, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.
      Table 'NotificationSettings'. Scan count 2, logical reads 971, physical reads 0, page server reads 0, read-ahead reads 0, page server read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob page server reads 0, lob read-ahead reads 0, lob page server read-ahead reads 0.

      (10 rows affected)

      SQL Server Execution Times:
        CPU time = 32 ms,  elapsed time = 27 ms.

      Completion time: 2020-07-09T18:24:51.2891326-05:00
    */
    public long StopwatchTicks { get; }
    public Statistics Statistics { get; }
    public SampleBenchmark(long stopwatchTicks, Statistics statistics) {
      StopwatchTicks = stopwatchTicks;
      Statistics = statistics;
    }
  }
}
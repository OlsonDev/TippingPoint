using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Diagnostics;
using System.Collections.Generic;
using TippingPoint.Extensions;
using TippingPoint.Dto;
using TippingPoint.Sql;
using TippingPoint.Benchmark.Telemetry;
using TippingPoint.Benchmark.IndexBenchmarks;
using TippingPoint.Dto.Benchmark;
using System.Runtime;

namespace TippingPoint {
  internal class Program {
    public const int Iterations = 70;
    public const int SamplesPerIteration = 20;
    private static readonly Random Random = new Random();
    private static readonly string MasterConnectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=True";
    private static readonly string ConnectionString = "Data Source=localhost;Initial Catalog=TippingPoint;Integrated Security=True";
    private static readonly MemoryDb MemoryDb = new MemoryDb();
    private static readonly List<List<DynamicParameters>> HitQueryParametersPerIteration = new List<List<DynamicParameters>>(Iterations);
    private static readonly List<List<DynamicParameters>> MissQueryParametersPerIteration = new List<List<DynamicParameters>>(Iterations);
    private static readonly List<IndexBenchmarkBase> IndexesToBenchmark = new List<IndexBenchmarkBase> {
      new FilteredIndex2Include3Benchmark(),
      new FilteredIndex3Include2Benchmark(),
      new Index2Include3Benchmark(),
      new Index3Include2Benchmark(),
      new NoIndexBenchmark(),
    };

    // TODO: Play with
    // dbo.NotificationSettings (UserID, NotificationType, OrgID) INCLUDE (NotificationMethod, Role)
    // vs.
    // dbo.NotificationSettings (UserID, NotificationType) INCLUDE (OrgID, NotificationMethod, Role)
    // which correlates to...
    // dbo.Qux (FooID, QuxDatum1, BarID) INCLUDE (QuxDatum2, QuxDatum3)
    // vs.
    // dbo.Qux (FooID, QuxDatum1) INCLUDE (BarID, QuxDatum2, QuxDatum3)
    internal static async Task Main() {
      PrepTvpCache();
      await GenerateMemoryDbDataAsync();
      await TryInitResultsSchemaAsync();
      var i = 0;
      foreach (var indexBenchmark in IndexesToBenchmark) {
        i++;

        await InitSchemaAsync(i, indexBenchmark);
        await RunBenchmarkAsync(i, indexBenchmark);
      }
      await InsertBenchmarkResultsAsync();
    }

    private static void PrepTvpCache() {
      DapperExtensions.PrepTvpTypeCache<FooDto>("dbo.FooTvp");
      DapperExtensions.PrepTvpTypeCache<BarDto>("dbo.BarTvp");
      DapperExtensions.PrepTvpTypeCache<QuxDto>("dbo.QuxTvp");
    }

    private static async Task RunBenchmarkAsync(int indexBenchmarkNumber, IndexBenchmarkBase indexBenchmark) {
      Console.WriteLine($"Running index benchmark... #{indexBenchmarkNumber,2}/{IndexesToBenchmark.Count,2} | {indexBenchmark.Name}");
      var connection = new SqlConnection(ConnectionString);
      await connection.OpenAsync();

      var stopwatch = new Stopwatch();
      for (var i = 0; i < Iterations; i++) {
        Console.Write($"Inserting data  for iteration #{i + 1,3}... ");
        stopwatch.Restart();
        await InsertFoosAsync(connection, i);
        await InsertBarsAsync(connection, i);
        await InsertQuxsAsync(connection, i);
        stopwatch.Stop();
        Console.WriteLine($"done! -- Elapsed: {stopwatch.Elapsed.ToHumanFormat()}");

        Console.Write($"Running queries for iteration #{i + 1,3}... ");
        stopwatch.Restart();
        var hitSampleParameters = HitQueryParametersPerIteration[i];
        var missSampleParameters = MissQueryParametersPerIteration[i];
        await indexBenchmark.RunQueriesToBenchmarkAsync(connection, hitSampleParameters, missSampleParameters);
        stopwatch.Stop();
        Console.WriteLine($"done! -- Elapsed: {stopwatch.Elapsed.ToHumanFormat()}");
      }
      await connection.CloseAsync();
    }

    private static async Task InsertBenchmarkResultsAsync() {
      Console.Write("Building benchmark results TVPs... ");
      var tableIterationTvp = BuildTableIterationTvp();
      var (flatResultTvp, tableIoTvp) = BuildBenchmarkResultsTvps();
      Console.WriteLine($"done! {flatResultTvp.Count} flat result records and {tableIoTvp.Count} table IO records.");
      Console.Write("Inserting benchmark results... ");
      var connection = new SqlConnection(ConnectionString);
      await connection.OpenAsync();
      await connection.ExecuteAsync(Insert.ResultsFromTvps, new {
        tableIterationTvp = tableIterationTvp.AsTableValuedParameter("Benchmark.TableIterationTvp"),
        flatResultTvp = flatResultTvp.AsTableValuedParameter("Benchmark.FlatResultTvp"),
        tableIoTvp = tableIoTvp.AsTableValuedParameter("Benchmark.TableIoTvp")
      });
      await connection.CloseAsync();
      Console.WriteLine("done!");
    }

    private static List<TableIterationTvp> BuildTableIterationTvp() {
      var tableIterationTvp = new List<TableIterationTvp>();
      var fooRanges = MemoryDb.Foo.IterationRanges;
      var barRanges = MemoryDb.Bar.IterationRanges;
      var quxRanges = MemoryDb.Qux.IterationRanges;
      for (var i = 0; i < Iterations; i++) {
        var iterationNumber = i + 1;
        tableIterationTvp.Add(new TableIterationTvp(iterationNumber, "Foo", fooRanges[i].End.GetOffset(int.MaxValue)));
        tableIterationTvp.Add(new TableIterationTvp(iterationNumber, "Bar", barRanges[i].End.GetOffset(int.MaxValue)));
        tableIterationTvp.Add(new TableIterationTvp(iterationNumber, "Qux", quxRanges[i].End.GetOffset(int.MaxValue)));
      }
      return tableIterationTvp;
    }

    private static (List<FlatResultTvp>, List<TableIoTvp>) BuildBenchmarkResultsTvps() {
      var flatResultTvp = new List<FlatResultTvp>();
      var tableIoTvp = new List<TableIoTvp>();
      foreach (var indexBenchmark in IndexesToBenchmark) {
        foreach (var queryBenchmark in indexBenchmark.QueriesToBenchmark) {
          var iterationNumber = 0;
          foreach (var iterationBenchmark in queryBenchmark.IterationBenchmarks) {
            iterationNumber++;
            BuildAndAddBenchmarkResultTvps(flatResultTvp, tableIoTvp, indexBenchmark, queryBenchmark, iterationBenchmark, iterationNumber, "Hit");
            BuildAndAddBenchmarkResultTvps(flatResultTvp, tableIoTvp, indexBenchmark, queryBenchmark, iterationBenchmark, iterationNumber, "Miss");
          }
        }
      }
      return (flatResultTvp, tableIoTvp);
    }

    private static void BuildAndAddBenchmarkResultTvps(
      IList<FlatResultTvp> flatResultTvp,
      IList<TableIoTvp> tableIoTvp,
      IndexBenchmarkBase indexBenchmark,
      QueryBenchmarkBase queryBenchmark,
      IterationBenchmark iterationBenchmark,
      int iterationNumber,
      string hitOrMiss) {
      var sampleNumber = 0;
      var samples = hitOrMiss == "Hit" ? iterationBenchmark.HitSamples : iterationBenchmark.MissSamples;
      foreach (var sampleBenchmark in samples) {
        sampleNumber++;
        var timeStats = sampleBenchmark.Statistics.Time;
        flatResultTvp.Add(new FlatResultTvp(indexBenchmark, queryBenchmark, sampleBenchmark, iterationNumber, sampleNumber, hitOrMiss));
        foreach (var tableIo in sampleBenchmark.Statistics.Io.TableIos) {
          tableIoTvp.Add(new TableIoTvp(indexBenchmark, queryBenchmark, iterationNumber, sampleNumber, tableIo, hitOrMiss));
        }
      }
    }

    private static async Task InsertAsync<TDto, TId>(
      SqlConnection connection,
      int iteration,
      string cmd,
      string tvpName,
      MemoryTable<TDto, TId> table) {
      // We're going to insert in batches so we don't have to transmit everything across the
      // network before SQL Server acts. Let's go with an arbitrary size of 5000 row batches.
      var ranges = table.IterationRanges[iteration].InBatchesOf(5000, table.Rows.Count);
      foreach (var range in ranges) {
        var tvp = table.Rows.EnumerateRange(range).AsTableValuedParameter(tvpName);
        await connection.ExecuteAsync(cmd, new { tvp });
      }
    }

    private static Task InsertFoosAsync(SqlConnection connection, int iteration)
      => InsertAsync(connection, iteration, Insert.FoosFromTvp, "dbo.FooTvp", MemoryDb.Foo);

    private static Task InsertBarsAsync(SqlConnection connection, int iteration)
      => InsertAsync(connection, iteration, Insert.BarsFromTvp, "dbo.BarTvp", MemoryDb.Bar);

    private static Task InsertQuxsAsync(SqlConnection connection, int iteration)
      => InsertAsync(connection, iteration, Insert.QuxsFromTvp, "dbo.QuxTvp", MemoryDb.Qux);

    private static async Task GenerateMemoryDbDataAsync() {
      Console.WriteLine($"Generating data which'll span {Iterations} iterations... ");
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      for (var i = 0; i < Iterations; i++) {
        var shouldLogProgress = (i + 1) % 10 == 0 || i >= 49;
        if (shouldLogProgress) {
          Console.Write($"Generating iteration #{i + 1,3}... ");
        }
        var result = await MemoryDb.ScaleUpAsync();

        // Grab some query parameters for our benchmarks.
        var quxs = result.AddedQuxs;
        var count = quxs.Count;
        var hitIterationParameters = new List<DynamicParameters>();
        var missIterationParameters = new List<DynamicParameters>();
        for (var j = 0; j < SamplesPerIteration; j++) {
          // Add parameters for these columns so they can take part in various queries...
          // (FooID, QuxDatum1, BarID) INCLUDE (QuxDatum2, QuxDatum3)
          var sampleHitParameters = new DynamicParameters();
          var sampleMissParameters = new DynamicParameters();
          var qux = quxs[Random.Next(count)];
          sampleHitParameters.Add("FooID", qux.FooId);
          sampleHitParameters.Add("BarID", qux.BarId);
          sampleHitParameters.Add("QuxDatum1", qux.QuxDatum1);
          sampleHitParameters.Add("QuxDatum2", qux.QuxDatum2);
          sampleHitParameters.Add("QuxDatum3", qux.QuxDatum3);

          sampleMissParameters.Add("FooID", qux.FooId!.Value.Increment()); // Highly unlikely to have two sequential GUIDs.
          sampleMissParameters.Add("BarID", MemoryDb.Bar.Rows.Count + Random.Next(100, 1000)); // Higher BarID than what's in the DB at this point.
          sampleMissParameters.Add("QuxDatum1", Random.Next(40, 51)); // This range is present in GetRandomQuxDatum1().
          sampleMissParameters.Add("QuxDatum2", Random.Next(3, 100)); // QuxDatum2 is either 0 or 1.
          sampleMissParameters.Add("QuxDatum3", Random.Next(5, 100)); // This range is not present in GetRandomQuxDatum3().

          hitIterationParameters.Add(sampleHitParameters);
          missIterationParameters.Add(sampleMissParameters);
        }
        HitQueryParametersPerIteration.Add(hitIterationParameters);
        MissQueryParametersPerIteration.Add(missIterationParameters);

        if (shouldLogProgress) {
          stopwatch.Stop();
          Console.WriteLine($"done! CLR memory usage: {FormatBytes(GC.GetTotalMemory(false))}; Elapsed: {stopwatch.Elapsed.ToHumanFormat()}");
          stopwatch.Restart();
        }
      }

      Console.Write("Collecting garbage... ");
      stopwatch.Restart();
      GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
      GC.Collect(2, GCCollectionMode.Forced, true);
      stopwatch.Stop();
      Console.WriteLine($"done! CLR memory usage: {FormatBytes(GC.GetTotalMemory(false))}; Elapsed: {stopwatch.Elapsed.ToHumanFormat()}");

      // Print some pretty stuff.
      Console.WriteLine("| Iteration | # Foos    | # Bars    | # Quxs    |");
      Console.WriteLine("|===========|===========|===========|===========|");
      Console.WriteLine($"|    (Last) | {MemoryDb.Foo.Count,9} | {MemoryDb.Bar.Count,9} | {MemoryDb.Qux.Count,9} |");
      Console.WriteLine("|===========|===========|===========|===========|");
      var fooRanges = MemoryDb.Foo.IterationRanges;
      var barRanges = MemoryDb.Bar.IterationRanges;
      var quxRanges = MemoryDb.Qux.IterationRanges;
      for (var i = 0; i < Iterations; i++) {
        var line = $"| {i + 1,9} | {fooRanges[i].End,9} | {barRanges[i].End,9} | {quxRanges[i].End,9} |";
        Console.WriteLine(line);
      }
    }

    private static string FormatBytes(long bytes) {
      var suffix = new[] { "B", "KB", "MB", "GB", "TB" };
      double num = bytes;
      var i = 0;
      for (; i < suffix.Length && bytes >= 1024; i++, bytes /= 1024) {
        num = bytes / 1024.0;
      }
      return $"{num:0.00} {suffix[i]}";
    }

    private static async Task TryInitResultsSchemaAsync() {
      try {
        await InitResultsSchemaAsync();
      } catch (SqlException) {
        await TryCreateDbAsync();
        await WaitForDatabaseReadyAsync();
        await InitResultsSchemaAsync();
      }
    }

    private static async Task InitResultsSchemaAsync() {
        Console.Write("Connecting to master database... ");
        var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        Console.WriteLine("done!");
        Console.Write("Creating benchmark results schema... ");
        await connection.ExecuteAsync(ResultsSchema.DropAndCreateBase);
        await connection.CloseAsync();
        Console.WriteLine("done!");
    }

    private static async Task InitSchemaAsync(int indexBenchmarkNumber, IndexBenchmarkBase indexBenchmark) {
      try {
        Console.WriteLine($"Initializing schema for index benchmark... #{indexBenchmarkNumber,2}/{IndexesToBenchmark.Count,2} | {indexBenchmark.Name}");
        await indexBenchmark.TryInitSchemaAsync(ConnectionString);
      } catch (SqlException) {
        await TryCreateDbAsync();
        await WaitForDatabaseReadyAsync();
        await indexBenchmark.TryInitSchemaAsync(ConnectionString);
      }
    }

    private static async Task TryCreateDbAsync() {
      using var connection = new SqlConnection(MasterConnectionString);

      Console.Write("Connecting to master database... ");
      await connection.OpenAsync();
      Console.WriteLine("done!");

      Console.Write("Creating TippingPoint database... ");
      await connection.ExecuteAsync(Schema.CreateDb);
      Console.WriteLine("done!");
    }

    private static async Task WaitForDatabaseReadyAsync() {
      using var connection = new SqlConnection(ConnectionString);
      var stopwatch = new Stopwatch();
      var timeout = TimeSpan.FromSeconds(30);
      var delay = 100;
      while (true) {
        try {
          if (stopwatch.Elapsed >= timeout) throw new TimeoutException("Could not login to database after creating it.");
          Console.Write("Connecting to TippingPoint database... ");
          await connection.OpenAsync();
          await connection.CloseAsync();
          Console.WriteLine("done! TippingPoint database is ready for use.");
          break;
        } catch (SqlException ex) {
          // https://docs.microsoft.com/en-us/previous-versions/sql/sql-server-2008-r2/cc645613%28v%3dsql.105%29
          // Cannot open database "%.*ls" requested by the login. The login failed.
          if (ex.Number != 4060) throw;
          if (stopwatch.Elapsed >= timeout) throw;
          Console.WriteLine($"Failed to connect; database is not ready. Exception message: {ex.Message}");
          await Task.Delay(delay);
          delay = Math.Min(400, delay + 100);
        }
      }
    }
  }
}
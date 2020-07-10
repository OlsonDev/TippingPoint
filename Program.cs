using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Diagnostics;
using System.Collections.Generic;
using TippingPoint.Extensions;
using TippingPoint.Dto;
using TippingPoint.SqlStatistics;
using System.Text.Json;
using TippingPoint.Benchmark;
using TippingPoint.Sql;

namespace TippingPoint {
  internal class Program {

    public const int Iterations = 3; // 50
    public const int SamplesPerIteration = 100;
    private static readonly Random Random = new Random();
    private static readonly string MasterConnectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=True";
    private static readonly string ConnectionString = "Data Source=localhost;Initial Catalog=TippingPoint;Integrated Security=True";
    private static readonly MemoryDb MemoryDb = new MemoryDb();
    private static readonly List<List<DynamicParameters>> HitQueryParametersPerIteration = new List<List<DynamicParameters>>(Iterations);
    private static readonly List<List<DynamicParameters>> MissQueryParametersPerIteration = new List<List<DynamicParameters>>(Iterations);

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
      await InitSchemaAsync(2);
      var twoIndexBenchmark = await RunBenchmarkAsync();
      await InitSchemaAsync(3);
      var threeIndexBenchmark = await RunBenchmarkAsync();
    }

    private static void PrepTvpCache() {
      DapperExtensions.PrepTvpTypeCache<FooDto>("dbo.FooTvp");
      DapperExtensions.PrepTvpTypeCache<BarDto>("dbo.BarTvp");
      DapperExtensions.PrepTvpTypeCache<QuxDto>("dbo.QuxTvp");
    }

    private static async Task<IEnumerable<IterationBenchmark>> RunBenchmarkAsync() {
      Console.WriteLine("Running benchmark...");
      var connection = new SqlConnection(ConnectionString);
      await connection.OpenAsync();

      var benchmarks = new List<IterationBenchmark>(Iterations);
      for (var i = 0; i < Iterations; i++) {
        var iterationBenchmark = new IterationBenchmark();
        Console.WriteLine($"Inserting data for iteration #{i + 1,2}");
        await InsertFoosAsync(connection, i);
        await InsertBarsAsync(connection, i);
        await InsertQuxsAsync(connection, i);
        Console.WriteLine($"Running queries for iteration #{i + 1,2}");
        await RunQueriesToBenchmarkAsync(connection, i, iterationBenchmark);
        benchmarks.Add(iterationBenchmark);
      }
      await connection.CloseAsync();
      return benchmarks;
    }

    private static SqlInfoMessageEventHandler BuildInfoMessageHandler(TaskCompletionSource<Statistics> source) {
      var statistics = new Statistics();
      return (sender, e) => {
        statistics.HandleInfoMessageEvent(e);
        if (statistics.IsComplete) source.SetResult(statistics);
      };
    }

    private static async Task RunQueriesToBenchmarkAsync(
      SqlConnection connection,
      int iteration,
      IterationBenchmark iterationBenchmark) {
      await connection.ExecuteAsync(Stats.On);
      var statisticsSource = new TaskCompletionSource<Statistics>();
      var infoMessageHandler = BuildInfoMessageHandler(statisticsSource);
      connection.InfoMessage += infoMessageHandler;
      var dtos = (await connection.QueryAsync<FooDto>("SELECT TOP 10000 * FROM dbo.Foo;")).AsList();
      Console.WriteLine($"Retrieved {dtos.Count} DTOs.");

      // In testing this, I've found it's more likely we'll have the InfoMessage events sent prior
      // to us having DTO data, but we'll await that 2nd because it seems more finnicky with timing.
      Console.Write("Waiting for statistics... ");
      var statistics = await statisticsSource.Task;
      Console.WriteLine($"done!");
      Console.WriteLine(JsonSerializer.Serialize(statistics));
      Console.WriteLine();

      connection.InfoMessage -= infoMessageHandler;
      await connection.ExecuteAsync(Stats.Off);
    }

    private static async Task InsertAsync<TDto, TId>(SqlConnection connection, int iteration, string cmd, string tvpName, MemoryTable<TDto, TId> table) {
      // We're going to insert in batches so we don't have to transmit everything across the
      // network before SQL Server acts. Let's go with an arbitrary size of 1000 row batches.
      var ranges = table.IterationRanges[iteration].InBatchesOf(1000, table.Rows.Count);
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
      Console.Write($"Generating data which'll span {Iterations} iterations... ");
      for (var i = 0; i < Iterations; i++) {
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
      }

      // Print some pretty stuff.
      Console.WriteLine($"done! CLR memory usage: {FormatBytes(GC.GetTotalMemory(false))}");
      Console.WriteLine("| Iteration | # Foos  | # Bars  | # Quxs  |");
      Console.WriteLine("|===========|=========|=========|=========|");
      Console.WriteLine($"|    (Last) | {MemoryDb.Foo.Count,7} | {MemoryDb.Bar.Count,7} | {MemoryDb.Qux.Count,7} |");
      Console.WriteLine("|===========|=========|=========|=========|");
      var fooRanges = MemoryDb.Foo.IterationRanges;
      var barRanges = MemoryDb.Bar.IterationRanges;
      var quxRanges = MemoryDb.Qux.IterationRanges;
      for (var i = 0; i < Iterations; i++) {
        var line = $"| {i + 1,9} | {fooRanges[i].End,7} | {barRanges[i].End,7} | {quxRanges[i].End,7} |";
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
      return $"{num:0.##} {suffix[i]}";
    }

    private static async Task InitSchemaAsync(int countIndexed) {
      try {
        await TryInitSchemaAsync(countIndexed);
      } catch (SqlException) {
        await TryCreateDbAsync();
        await WaitForDatabaseReadyAsync();
        await TryInitSchemaAsync(countIndexed);
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

    private static async Task TryInitSchemaAsync(int countIndexed) {
      using var connection = new SqlConnection(ConnectionString);
      Console.Write("Connecting to TippingPoint database... ");
      await connection.OpenAsync();
      Console.WriteLine("done!");
      using var transaction = connection.BeginTransaction();
      var cmd = Schema.DropAndCreateWithNIndexed(countIndexed);
      Console.Write("Initializing TippingPoint schema... ");
      await connection.ExecuteAsync(cmd, null, transaction);
      await transaction.CommitAsync();
      Console.WriteLine("done!");
    }
  }
}
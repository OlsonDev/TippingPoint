using System;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;

namespace TippingPoint.SqlStatistics {
  public class Statistics {
    private static readonly object ConsoleLock = new object();
    public IoStatistics Io { get; } = new IoStatistics();
    public TimeStatistics Time { get; } = new TimeStatistics();

    [JsonIgnore]
    public bool IsComplete
      => Io.IsComplete && Time.IsComplete;

    public void HandleInfoMessageEvent(SqlInfoMessageEventArgs e) {
      if (Io.HandleInfoMessageEvent(e)) return;
      if (Time.HandleInfoMessageEvent(e)) return;
      LogUnhandledInfoMessageEvent(e);
    }

    private void LogUnhandledInfoMessageEvent(SqlInfoMessageEventArgs e) {
      lock (ConsoleLock) {
        var foregroundColor = Console.ForegroundColor;
        Console.WriteLine("Unhandled info message:");
        Console.WriteLine();
        Console.WriteLine("<SqlInfo>");
        if (!string.IsNullOrWhiteSpace(e.Source)) {
          Console.WriteLine($"    <Source>{e.Source}</Source>");
        }
        if (!string.IsNullOrWhiteSpace(e.Message)) {
          Console.WriteLine($"    <Message>{e.Message}</Message>");
        }
        foreach (var error in e.Errors) {
          Console.WriteLine($"    <Error>{error}</Error>");
        }
        Console.WriteLine("</SqlInfo>");
        Console.WriteLine();
        Console.ForegroundColor = foregroundColor;
      }
    }
  }
}
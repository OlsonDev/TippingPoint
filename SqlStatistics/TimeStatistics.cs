using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace TippingPoint.SqlStatistics {
  public class TimeStatistics {
    private static readonly Regex Format = new Regex(
      @"^\s*SQL Server ((?<pnc>parse and compile time)|(?<exec>Execution Times)):\s*CPU time = (?<cpu>\d+) ms,\s+elapsed time = (?<elapsed>\d+) ms.\s*$",
      RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline
    );
    public SqlTime? ParseAndCompileTime { get; private set; } = null;
    public SqlTime? ExecutionTime { get; private set; } = null;

    [JsonIgnore]
    public bool IsComplete
      => !(ParseAndCompileTime is null && ExecutionTime is null);

    public bool HandleInfoMessageEvent(SqlInfoMessageEventArgs e) {
      var match = Format.Match(e.Message);
      if (!match.Success) return false;
      var groups = match.Groups;
      var cpu = groups["cpu"];
      var elapsed = groups["elapsed"];
      var isPnc = groups["pnc"].Success;
      var isExec = groups["exec"].Success;
      if (!cpu.Success || !elapsed.Success || (isPnc == isExec)) return false;
      var sqlTime = new SqlTime(int.Parse(cpu.Value), int.Parse(elapsed.Value));
      if (isPnc) ParseAndCompileTime = sqlTime;
      else ExecutionTime = sqlTime;
      return true;
    }
  }
}
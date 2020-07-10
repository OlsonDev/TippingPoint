using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace TippingPoint.SqlStatistics {
  public class IoStatistics {
    private const RegexOptions FormatRegexOptions =
        RegexOptions.Compiled
      | RegexOptions.ExplicitCapture
      | RegexOptions.IgnoreCase
      | RegexOptions.Multiline
      | RegexOptions.IgnorePatternWhitespace;

    private static readonly Regex Format = new Regex(@"
      ^\s*Table\s+'(?<TableName>[^']+)'
      # Prefix everything with comma or period so it matches the full description
      # instead of just a sub-description (e.g. physical reads vs. lob physical reads).
      ([,.\s]+
        (
            Scan\ Count\s+(?<ScanCount>\d+)
          | Logical\ Reads\s+(?<LogicalReads>\d+)
          | Physical\ Reads\s+(?<PhysicalReads>\d+)
          | Page\ Server\ Reads\s+(?<PageServerReads>\d+)
          | Read-Ahead\ Reads\s+(?<ReadAheadReads>\d+)
          | Page\ Server\ Read-Ahead\ Reads\s+(?<PageServerReadAheadReads>\d+)
          | Lob\ Logical\ Reads\s+(?<LobLogicalReads>\d+)
          | Lob\ Physical\ Reads\s+(?<LobPhysicalReads>\d+)
          | Lob\ Page\ Server\ Reads\s+(?<LobPageServerReads>\d+)
          | Lob\ Read-Ahead\ Reads\s+(?<LobReadAheadReads>\d+)
          | Lob\ Page\ Server\ Read-Ahead\ Reads\s+(?<LobPageServerReadAheadReads>\d+)
          | [^,.\\r\\n]+? # Ignore unsupported statistics
        )
      )+\s*\.\s*$
	", FormatRegexOptions);
    public List<TableIo> TableIos { get; } = new List<TableIo>();

    [JsonIgnore]
    public bool IsComplete => TableIos.Any();

    public bool HandleInfoMessageEvent(SqlInfoMessageEventArgs e) {
      // This method has some weirdness with nullable-reference type checking;
      // it thinks `matches` is `MatchCollection?`.
      // Also annoying: MatchCollection only implements the non-generic IEnumerable so we have to cast it.
      var matches = Format.Matches(e.Message);
      if (!matches.Any()) return false;
      foreach (var match in matches.Cast<Match>()) {
        var groups = match.Groups;
        TableIos.Add(new TableIo(groups["TableName"].Value) {
          ScanCount = GetGroupAsInt(groups, "ScanCount"),
          LogicalReads = GetGroupAsInt(groups, "LogicalReads"),
          PhysicalReads = GetGroupAsInt(groups, "PhysicalReads"),
          PageServerReads = GetGroupAsInt(groups, "PageServerReads"),
          ReadAheadReads = GetGroupAsInt(groups, "ReadAheadReads"),
          PageServerReadAheadReads = GetGroupAsInt(groups, "PageServerReadAheadReads"),
          LobLogicalReads = GetGroupAsInt(groups, "LobLogicalReads"),
          LobPhysicalReads = GetGroupAsInt(groups, "LobPhysicalReads"),
          LobPageServerReads = GetGroupAsInt(groups, "LobPageServerReads"),
          LobReadAheadReads = GetGroupAsInt(groups, "LobReadAheadReads"),
          LobPageServerReadAheadReads = GetGroupAsInt(groups, "LobPageServerReadAheadReads"),
        });
      }
      return true;
    }

    private int? GetGroupAsInt(GroupCollection groups, string key)
      => int.TryParse(groups[key].Value, out var value) ? value : (int?)null;
  }
}
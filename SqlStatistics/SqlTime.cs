namespace TippingPoint.SqlStatistics {
  public class SqlTime {
    public int CpuMs { get; }
    public int ElapsedMs { get; }
    public SqlTime(int cpuMs, int elapsedMs) {
      CpuMs = cpuMs;
      ElapsedMs = elapsedMs;
    }
  }
}
using System;

namespace TippingPoint.Extensions {
  public static class TimeSpanExtensions {
		public static string ToHumanFormat(this TimeSpan timeSpan)
			=> timeSpan == TimeSpan.Zero
				? "0s"
				: timeSpan.TotalMilliseconds < 1d
					? "<1ms"
					: ""
						+ (timeSpan.Days > 0 ? $"{timeSpan.Days}d" : "")
						+ (timeSpan.Hours > 0 ? $"{timeSpan.Hours}h" : "")
						+ (timeSpan.Minutes > 0 ? $"{timeSpan.Minutes}m" : "")
						+ (timeSpan.Seconds > 0 ? $"{timeSpan.Seconds}s" : "")
						+ (timeSpan.Milliseconds > 0 ? $"{timeSpan.Milliseconds}ms" : "");

		public static string ToHumanFormat(this TimeSpan? timeSpan)
			=> timeSpan.HasValue
				? timeSpan.Value.ToHumanFormat()
				: "";
  }
}
using System;

namespace EqualityComparer
{
  /// <summary> Date time extensions.  </summary>
  /// <remarks> 7/19/2011. </remarks>
  public static class DateTimeExtensions
  {
    /// <summary> Truncates DateTime to second, so that JSON values with DateTimes can be roundtripped / compared properly. </summary>
    /// <remarks> 7/19/2011. </remarks>
    /// <param name="value">  Original DateTime value. </param>
    /// <returns> A new DateTime value truncated to the nearest second. </returns>
    public static DateTime TruncateToSecond(this DateTime value)
    {
      return new DateTime((value.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
    }

    /// <summary> A DateTime extension method that rounds DateTimes to the nearest second. </summary>
    /// <remarks> 7/19/2011. </remarks>
    /// <param name="value">  Original DateTime value. </param>
    /// <returns> A new DateTime value rounded and truncated to the nearest second. </returns>
    public static DateTime RoundToNearestSecond(this DateTime value)
    {
      if (value.Millisecond >= 500)
        //account for tiny discrepancies with ms and ticks
        value = value.AddMilliseconds(502);

      return TruncateToSecond(value);
    }
  }
}

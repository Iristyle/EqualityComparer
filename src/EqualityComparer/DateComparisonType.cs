using System;

namespace EqualityComparer
{
  /// <summary> Defines how to compare dates when using DateComaper{T}. </summary>
  /// <remarks> ebrown, 6/18/2011. </remarks>
  public enum DateComparisonType
  {
    /// <summary> An exact comparison by ticks.  </summary>
    Exact,
    /// <summary> A comparison truncated / always rounded down to the nearest second, which can be useful with data stores that do not roundtrip dates properly.  </summary>
    TruncatedToSecond
  }
}

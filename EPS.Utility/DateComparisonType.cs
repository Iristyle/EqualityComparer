using System;

namespace EPS
{
	/// <summary>	Defines how to compare dates when using DateComaper{T}. </summary>
	/// <remarks>	ebrown, 6/18/2011. </remarks>
	public enum DateComparisonType
	{
		/// <summary> An exact comparison by ticks.  </summary>
		Exact,
		/// <summary> A comparison to the nearest second, which can be useful with data stores that do not roundtrip dates properly.  </summary>
		ToSecond
	}
}
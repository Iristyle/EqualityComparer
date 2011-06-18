using System;
using System.Collections.Generic;
using System.Linq;

namespace EPS
{
	/// <summary>	An EqualityComparer implementation for DateTimes that can use a configurable comparison algorithm, 
	/// 			such as comparing exactly and comparing to the second. </summary>
	/// <remarks>	ebrown, 6/18/2011. </remarks>
	public class DateComparer : EqualityComparer<DateTime>
	{
		private DateComparisonType comparisonType;
		private static EqualityComparer<DateTime> _default =
			new DateComparer(DateComparisonType.Exact);

		/// <summary>	Gets the default DateComparer instance, which is by DateComparisonType.Exact. </summary>
		/// <value>	The default. </value>
		public static EqualityComparer<DateTime> Default
		{
			get { return _default; }
		}

		/// <summary>	Initializes a new instance of the DateComparer class. </summary>
		/// <remarks>	ebrown, 6/18/2011. </remarks>
		/// <param name="comparisonType">	The method by which dates should be compared. </param>
		public DateComparer(DateComparisonType comparisonType)
		{
			this.comparisonType = comparisonType;
		}

		/// <summary>	Tests if two DateTime objects are considered equal, given the DateComparisonType. </summary>
		/// <remarks>	ebrown, 6/18/2011. </remarks>
		/// <param name="x">	Date time to be compared. </param>
		/// <param name="y">	Date time to be compared. </param>
		/// <returns>	true if the objects are considered equal, false if they are not. </returns>
		public bool Equals(DateTime x, DateTime y)
		{
			switch (comparisonType)
			{
				case DateComparisonType.ToSecond:
					return new DateTime((x.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond).Equals(new DateTime((y.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond));
				case DateComparisonType.Exact:
				default:
					return x.Equals(y);
			}
		}

		/// <summary>	Calculates the hash code for this object. </summary>
		/// <remarks>	ebrown, 6/18/2011. </remarks>
		/// <param name="obj">	Date/Time of the object. </param>
		/// <returns>	The hash code for this object. </returns>
		public int GetHashCode(DateTime obj)
		{
			switch (comparisonType)
			{
				case DateComparisonType.ToSecond:
					return new DateTime((obj.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond).GetHashCode();
				case DateComparisonType.Exact:
				default:
					return obj.GetHashCode();
			}			
		}
	}
}
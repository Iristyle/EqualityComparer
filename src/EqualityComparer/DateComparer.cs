using System;
using System.Collections.Generic;
using System.Linq;

namespace EqualityComparer
{
  /// <summary> An EqualityComparer implementation for DateTimes that can use a configurable comparison algorithm,
  ///       such as comparing exactly and comparing to the second. </summary>
  /// <remarks> ebrown, 6/18/2011. </remarks>
  public class DateComparer : EqualityComparer<DateTime>
  {
    private DateComparisonType comparisonType;
    private static EqualityComparer<DateTime> _default =
      new DateComparer(DateComparisonType.Exact);

    /// <summary> Gets the default DateComparer instance, which is by DateComparisonType.Exact. </summary>
    /// <value> The default. </value>
    public static new EqualityComparer<DateTime> Default
    {
      get { return _default; }
    }

    /// <summary> Initializes a new instance of the DateComparer class. </summary>
    /// <remarks> ebrown, 6/18/2011. </remarks>
    /// <param name="dateComparisonType"> The method by which dates should be compared. </param>
    public DateComparer(DateComparisonType dateComparisonType)
    {
      this.comparisonType = dateComparisonType;
    }

    /// <summary> Tests if two DateTime objects are considered equal, given the DateComparisonType. </summary>
    /// <remarks> ebrown, 6/18/2011. </remarks>
    /// <param name="x">  Date time to be compared. </param>
    /// <param name="y">  Date time to be compared. </param>
    /// <returns> true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(DateTime x, DateTime y)
    {
      switch (comparisonType)
      {
        case DateComparisonType.TruncatedToSecond:
          return x.TruncateToSecond().Equals(y.TruncateToSecond());
        case DateComparisonType.Exact:
        default:
          return x.Equals(y);
      }
    }

    /// <summary> Calculates the hash code for this object. </summary>
    /// <remarks> ebrown, 6/18/2011. </remarks>
    /// <param name="obj">  Date/Time of the object. </param>
    /// <returns> The hash code for this object. </returns>
    public override int GetHashCode(DateTime obj)
    {
      switch (comparisonType)
      {
        case DateComparisonType.TruncatedToSecond:
          return obj.TruncateToSecond().GetHashCode();
        case DateComparisonType.Exact:
        default:
          return obj.GetHashCode();
      }
    }
  }
}

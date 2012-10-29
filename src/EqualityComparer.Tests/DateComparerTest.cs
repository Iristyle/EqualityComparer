using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions;

namespace EqualityComparer.Tests
{
  public class DateComparerTest
  {
    private static DateTime wellKnownDate = new DateTime(2011, 6, 20, 13, 30, 1, 200);

    public static IEnumerable<object[]> GetEqualToSecondDates
    {
      get
      {
        yield return new object[] { wellKnownDate, wellKnownDate };
        yield return new object[] { wellKnownDate, wellKnownDate.AddMilliseconds(100) };
        yield return new object[] { wellKnownDate, wellKnownDate.AddMilliseconds(799) };
      }
    }

    public static IEnumerable<object[]> GetUnequalToSecondDates
    {
      get
      {
        yield return new object[] { wellKnownDate, wellKnownDate.AddMilliseconds(800) };
        yield return new object[] { wellKnownDate, wellKnownDate.AddMilliseconds(-201) };
      }
    }

    [Theory]
    [PropertyData("GetEqualToSecondDates")]
    public void Equals_TruncatedToSecond_ReturnsTrue_OnComparisonsWithinSameSecond(DateTime testValue1, DateTime testValue2)
    {
      Assert.True(new DateComparer(DateComparisonType.TruncatedToSecond).Equals(testValue1, testValue2));
    }

    [Theory]
    [PropertyData("GetUnequalToSecondDates")]
    public void Equals_TruncatedToSecond_ReturnsFalse_OnComparisonsOutsideSameSecond(DateTime testValue1, DateTime testValue2)
    {
      Assert.False(new DateComparer(DateComparisonType.TruncatedToSecond).Equals(testValue1, testValue2));
    }

    [Fact]
    public void Equals_ExactComparison_ReturnsFalse_DifferentDateTime()
    {
      var now = DateTime.Now;
      Assert.False(new DateComparer(DateComparisonType.Exact).Equals(now, now.AddTicks(1)));
    }

    [Fact]
    public void Equals_ExactComparison_ReturnsTrue_SameTime()
    {
      var now = DateTime.Now;
      Assert.True(new DateComparer(DateComparisonType.Exact).Equals(now, now));
    }
  }
}

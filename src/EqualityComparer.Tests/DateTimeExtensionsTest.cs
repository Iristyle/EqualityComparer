using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace EqualityComparer.Tests
{
  public class DateTimeExtensionsTest
  {
    private static DateTime wellKnownDate = new DateTime(2011, 6, 20, 13, 30, 1, 200);

    public static IEnumerable<object[]> GetTruncatedExpectations
    {
      get
      {
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 499), new DateTime(2011, 6, 20, 13, 30, 1, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 499).AddTicks(TimeSpan.TicksPerSecond / 2), new DateTime(2011, 6, 20, 13, 30, 1, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 500), new DateTime(2011, 6, 20, 13, 30, 1, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 999), new DateTime(2011, 6, 20, 13, 30, 1, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 1), new DateTime(2011, 6, 20, 13, 30, 1, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 2, 1), new DateTime(2011, 6, 20, 13, 30, 2, 0) };
      }
    }

    public static IEnumerable<object[]> GetRoundedExpectations
    {
      get
      {
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 499), new DateTime(2011, 6, 20, 13, 30, 1, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 499).AddTicks(TimeSpan.TicksPerSecond / 2), new DateTime(2011, 6, 20, 13, 30, 2, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 500), new DateTime(2011, 6, 20, 13, 30, 2, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 999), new DateTime(2011, 6, 20, 13, 30, 2, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 1, 1), new DateTime(2011, 6, 20, 13, 30, 1, 0) };
        yield return new object[] { new DateTime(2011, 6, 20, 13, 30, 2, 1), new DateTime(2011, 6, 20, 13, 30, 2, 0) };
      }
    }

    [Theory]
    [PropertyData("GetTruncatedExpectations")]
    public void TruncateToSecond_ReturnsExpected(DateTime input, DateTime expected)
    {
      Assert.Equal(input.TruncateToSecond(), expected.TruncateToSecond());
    }

    [Theory]
    [PropertyData("GetRoundedExpectations")]
    public void RoundToNearestSecond_ReturnsExpected(DateTime input, DateTime expected)
    {
      Assert.Equal(input.RoundToNearestSecond(), expected.RoundToNearestSecond());
    }
  }
}

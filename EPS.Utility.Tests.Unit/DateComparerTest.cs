using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace EPS.Utility.Tests.Unit
{
	public class DateComparerTest
	{

		[Fact]
		public void ExactComparison_SameDateTime_ReturnsTrue()
		{
			DateTime testValue = new DateTime(2011, 6, 20, 13, 30, 1, 200);
			Assert.True(new DateComparer(DateComparisonType.Exact).Equals(testValue, testValue));
		}

		[Fact]
		public void ExactComparison_DifferentDateTime_ReturnsFalse()
		{
			DateTime testValue = new DateTime(2011, 6, 20, 13, 30, 1, 200);
			Assert.False(new DateComparer(DateComparisonType.Exact).Equals(testValue, testValue.AddMilliseconds(100)));
		}

		[Fact]
		public void ToTheSecondComparison_SlightlyDifferentDateTime_ReturnsTrue()
		{
			DateTime testValue = new DateTime(2011, 6, 20, 13, 30, 1, 200);
			Assert.True(new DateComparer(DateComparisonType.ToSecond).Equals(testValue, testValue.AddMilliseconds(200)));
		}

		[Fact]
		public void ToTheSecondComparison_SlightlyDifferentDateTimeWithDifferentSecondValues_ReturnsTrue()
		{
			// note: 900ms + 200ms diff causes the second value to increment.
			DateTime testValue = new DateTime(2011, 6, 20, 13, 30, 1, 900);

			// TODO: should this return true or false?
			Assert.True(new DateComparer(DateComparisonType.ToSecond).Equals(testValue, testValue.AddMilliseconds(200)));
		}

		[Fact]
		public void ToTheSecondComparison_DateTimeWithDifferenceGreaterThanOneSecond_ReturnsFalse()
		{
			DateTime testValue = new DateTime(2011, 6, 20, 13, 30, 1, 900);
			Assert.False(new DateComparer(DateComparisonType.ToSecond).Equals(testValue, testValue.AddMilliseconds(1100)));
		}

	}
}

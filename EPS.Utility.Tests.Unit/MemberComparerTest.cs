using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace EPS.Utility.Tests.Unit
{
	public class MemberComparerTest
	{
		[Fact]
		public void Equal_ThrowsOnNullEqualityComparerDictionary()
		{
			Assert.Throws<ArgumentNullException>(() => MemberComparer.Equal(4, 4, null));
		}

		[Fact]
		public void Equal_ThrowsOnNullTypeInComparerDictionary()
		{
			Assert.Throws<ArgumentNullException>(() => MemberComparer.Equal(4, 4, new Dictionary<Type, IEqualityComparer>()
			{ 
				{ null, GenericEqualityComparer<int>.ByAllMembers() }
			}));
		}

		[Fact]
		public void Equal_ThrowsOnNullComparerInComparerDictionary()
		{
			Assert.Throws<ArgumentNullException>(() => MemberComparer.Equal(4, 4, new Dictionary<Type, IEqualityComparer>()
			{ 
				{ typeof(string), null }
			}));
		}

		[Fact]
		public void Equal_ThrowsOnMismatchedComparerType()
		{
			Assert.Throws<ArgumentException>(() => MemberComparer.Equal(4, 4, new Dictionary<Type, IEqualityComparer>()
			{ 
				{ typeof(string), GenericEqualityComparer<int>.ByAllMembers() }
			}));
		}

		[Fact]
		public void Equal_TrueOnNullXNullY()
		{
			Assert.True(MemberComparer.Equal(null as List<int>, null as List<int>));
		}

		[Fact]
		public void Equal_FalseOnNullXNonNullY()
		{
			Assert.False(MemberComparer.Equal(null as List<int>, new List<int>()));
		}

		[Fact]
		public void Equal_FalseOnNullYNonNullX()
		{
			Assert.False(MemberComparer.Equal(new List<int>(), null as List<int>));
		}

		[Fact]
		public void Equal_TrueOnString()
		{
			Assert.True(MemberComparer.Equal("foo", "foo"));
		}

		[Fact]
		public void Equal_FalseOnMismatchedString()
		{
			Assert.False(MemberComparer.Equal("foo", "bar"));
		}

		[Fact]
		public void Equal_TrueOnPrimitive()
		{
			Assert.True(MemberComparer.Equal(3, 3));
		}

		[Fact]
		public void Equal_FalseOnMismatchedPrimitive()
		{
			Assert.False(MemberComparer.Equal(5, 15));
		}

		[Fact]
		public void Equal_TrueOnSameObject()
		{
			var anonymous = new { PropertyA = "A", Integer = 23, Guid = Guid.NewGuid() };

			Assert.True(MemberComparer.Equal(anonymous, anonymous));
		}

		[Fact]
		public void Equal_TrueOnDifferentObjectsWithSameValues()
		{
			Guid sharedGuid = Guid.NewGuid();
			DateTime now = DateTime.Now;
			Assert.True(MemberComparer.Equal(new { PropertyA = "A", Integer = 23, Guid = sharedGuid, Date = now },
				new { PropertyA = "A", Integer = 23, Guid = sharedGuid, Date = now }));
		}

		[Fact]
		public void Equal_FalseOnDifferentObjectsWithDifferentValues()
		{
			Guid sharedGuid = Guid.NewGuid();
			Assert.False(MemberComparer.Equal(new { PropertyA = "B", Integer = 23, Guid = sharedGuid },
				new { PropertyA = "A", Integer = 23, Guid = sharedGuid }));
		}

		[Fact]
		public void Equal_TrueOnObjectsWithNestedObjects()
		{
			var sub1 = new { PropertyB = "b1" };
			var sub2 = new { PropertyB = "b1" };

			Assert.True(MemberComparer.Equal(sub1, sub2));
			Assert.True(MemberComparer.Equal(new { PropertyA = "A", Integer = 23, Sub = sub1 },
				new { PropertyA = "A", Integer = 23, Sub = sub2 }));
		}

		[Fact]
		public void Equal_FalseOnObjectsWithNestedObjectsWithDifferentValues()
		{
			var sub1 = new { PropertyB = "b1" };
			var sub2 = new { PropertyB = "b2" };

			Assert.False(MemberComparer.Equal(sub1, sub2));
			Assert.False(MemberComparer.Equal(new { PropertyA = "A", Integer = 23, Sub = sub1 },
				new { PropertyA = "A", Integer = 23, Sub = sub2 }));
		}

		[Fact]
		public void Equal_TrueOnObjectsWithNullNestedObjects()
		{
			//anonymous types that look the same like these actually share a static type (as constructed by the compiler)
			var sub1 = new { PropertyB = "b1" };
			sub1 = null;
			var sub2 = new { PropertyB = "b1" };
			sub2 = null;

			Assert.True(MemberComparer.Equal(sub1, sub2));
			Assert.True(MemberComparer.Equal(new { PropertyA = "A", Integer = 23, Sub = sub1 },
				new { PropertyA = "A", Integer = 23, Sub = sub2 }));
		}

		[Fact]
		public void Equal_FalseOnObjectsWithNullNestedObjectsWithDifferentValues()
		{
			var sub1 = new { PropertyB = "b1" };
			sub1 = null;
			var sub2 = new { PropertyB = "b2" };
			sub2 = null;

			Assert.True(MemberComparer.Equal(sub1, sub2));
			Assert.False(MemberComparer.Equal(new { PropertyA = "A", Integer = 24, Sub = sub1 },
				new { PropertyA = "A", Integer = 23, Sub = sub2 }));
		}

		[Fact]
		public void Equal_FalseOnObjectsWithOneNullNestedObjectAndOneNonNullNestedObject()
		{
			var sub1 = new { PropertyB = "b1" };
			var sub2 = new { PropertyB = "b2" };
			sub2 = null;

			Assert.False(MemberComparer.Equal(sub1, sub2));
			Assert.False(MemberComparer.Equal(new { PropertyA = "A", Integer = 23, Sub = sub1 },
				new { PropertyA = "A", Integer = 23, Sub = sub2 }));
		}

		[Fact]
		public void Equal_TrueOnDoubleNestedObjectsWithSameValues()
		{
			//we've nested two levels deep
			var sub2 = new { PropertyC = "b2" };
			var sub1 = new { PropertyB = "b1", Sub = sub2 };

			Assert.True(MemberComparer.Equal(new { PropertyA = "A", Integer = 23, Sub = sub1 },
				new { PropertyA = "A", Integer = 23, Sub = sub1 }));
		}

		[Fact]
		public void Equal_TrueOnObjectsWithEnumProperties()
		{
			Assert.True(MemberComparer.Equal(new { Day = DayOfWeek.Monday }, new { Day = DayOfWeek.Monday }));
		}

		[Fact]
		public void Equal_TrueOnObjectsWithNestedCollections()
		{
			var nestedCollection1 = new { Property = "value", NestedProperties = new List<string>() { "a", "b" } };
			var nestedCollection2 = new { Property = "value", NestedProperties = new List<string>() { "a", "b" } };

			Assert.True(MemberComparer.Equal(nestedCollection1, nestedCollection2));
		}

		[Fact]
		public void Equal_FalseOnObjectsWithNestedCollections()
		{
			var nestedCollection1 = new { Property = "value", NestedProperties = new List<string>() { "a", "b" } };
			var nestedCollection2 = new { Property = "value", NestedProperties = new List<string>() { "b", "a" } };

			Assert.False(MemberComparer.Equal(nestedCollection1, nestedCollection2));
		}

		[Fact]
		public void Equal_TrueOnEqualCollections()
		{
			Assert.True(MemberComparer.Equal(new int[] { 5, 10 }, new int[] { 5, 10 }));
		}

		[Fact]
		public void Equal_FalseOnMismatchedCollections()
		{
			Assert.False(MemberComparer.Equal(new int[] { 5, 10 }, new int[] { 10, 5 }));
		}

		class ClassWithFieldsAndProperties
		{
			public string Foo;
			public string Bar { get; set; }
		}

		[Fact]
		public void Equal_TrueOnClassWithMismatchedPropertiesAndFieldsWithCustomComparer()
		{
			string Bar = "bar";
			Assert.True(MemberComparer.Equal(new ClassWithFieldsAndProperties() { Foo = "456", Bar = Bar }, new ClassWithFieldsAndProperties() { Foo = "4567", Bar = Bar },
				 new Dictionary<Type, System.Collections.IEqualityComparer>()
				{
					{ typeof(ClassWithFieldsAndProperties),  new GenericEqualityComparer<ClassWithFieldsAndProperties>((a, b) => a.Bar == b.Bar) } 
				}));
		}

		[Fact]
		public void Equal_TrueOnClassWithMismatchedPropertiesAndFieldsWithCustomComparerNested()
		{
			string Bar = "bar";
			Assert.True(MemberComparer.Equal(new { Integer = 5, Custom = new ClassWithFieldsAndProperties() { Foo = "456", Bar = Bar }}, 
				new { Integer = 5, Custom = new ClassWithFieldsAndProperties() { Foo = "4567", Bar = Bar }},
				 new Dictionary<Type, System.Collections.IEqualityComparer>()
				{
					{ typeof(ClassWithFieldsAndProperties),  new GenericEqualityComparer<ClassWithFieldsAndProperties>((a, b) => a.Bar == b.Bar) } 
				}));
		}
		
		[Fact]
		public void Equal_TrueOnClasswithPropertiesAndFields()
		{
			string Bar = "123", Foo = "456";
			Assert.True(MemberComparer.Equal(new ClassWithFieldsAndProperties() { Bar = Bar, Foo = Foo }, new ClassWithFieldsAndProperties() { Bar = Bar, Foo = Foo }));
		}

		[Fact]
		public void Equal_FalseOnClassWithMismatchFieldValues()
		{
			string Bar = "bar";
			Assert.False(MemberComparer.Equal(new ClassWithFieldsAndProperties() { Foo = "456", Bar = Bar }, new ClassWithFieldsAndProperties() { Foo = "4567", Bar = Bar }));
		}

		[Fact]
		public void Equal_TrueOnExactDates()
		{
			DateTime now = DateTime.Now;
			Assert.True(MemberComparer.Equal(now, now));
		}

		[Fact]
		public void Equal_TrueToSecondOnEqualDates()
		{
			DateTime now = DateTime.Now;
			Assert.True(MemberComparer.Equal(now, now, new Dictionary<Type, System.Collections.IEqualityComparer>()
			{
				{ typeof(DateTime),  new DateComparer(DateComparisonType.ToSecond) } 
			}));
		}

		[Fact]
		public void Equal_FalseOnDatesDifferingByLessThanASecond()
		{
			DateTime one = DateTime.Parse("07:27:15.01"),
			two = DateTime.Parse("07:27:15.49");

			Assert.False(MemberComparer.Equal(one, two));
		}
		
		[Fact]
		public void Equal_TrueToSecondOnDatesDifferingByLessThanASecondWithCustomComparer()
		{
			DateTime one = DateTime.Parse("07:27:15.01"),
			two = DateTime.Parse("07:27:15.49");

			Assert.True(MemberComparer.Equal(one, two,  new Dictionary<Type, System.Collections.IEqualityComparer>()
			{
				{ typeof(DateTime),  new DateComparer(DateComparisonType.ToSecond) } 
			}));
		}

		[Fact]
		public void Equal_TrueToSecondOnNestedDatesDifferingByLessThanASecondWithCustomComparer()
		{
			DateTime one = DateTime.Parse("07:27:15.01"),
			two = DateTime.Parse("07:27:15.49");

			var a = new { Foo = 5, Bar = new { Now = one } };
			var b = new { Foo = 5, Bar = new { Now = two } };

			Assert.True(MemberComparer.Equal(one, two,  new Dictionary<Type, System.Collections.IEqualityComparer>()
			{
				{ typeof(DateTime),  new DateComparer(DateComparisonType.ToSecond) } 
			}));
		}

		[Fact]
		public void Equal_TrueToSecondOnNestedCollectionOfDatesDifferingByLessThanASecondWithCustomComparer()
		{
			var dates = new [] { DateTime.Parse("07:27:15.01"), DateTime.Parse("07:27:15.49") };
			Assert.True(MemberComparer.Equal(new { A = 1, Dates = dates }, new { A = 1, Dates = dates },  new Dictionary<Type, System.Collections.IEqualityComparer>()
			{
				{ typeof(DateTime),  new DateComparer(DateComparisonType.ToSecond) } 
			}));
		}
	}
}
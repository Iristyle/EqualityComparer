using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace EqualityComparer.Tests
{
	public class GenericEqualityComparerTest
	{
		class A
		{
			public A(int integer, string @string)
			{
				Integer = integer;
				String = @string;
			}

			public int Integer { get; set; }
			public string String { get; set; }

			public static GenericEqualityComparer<A> IntegerOnlyComparer = new GenericEqualityComparer<A>((a1, a2) => a1.Integer == a2.Integer);
			public static GenericEqualityComparer<A> AllPropertiesComparer = new GenericEqualityComparer<A>((a1, a2) => a1.Integer == a2.Integer && a1.String == a2.String);
		}

		class B
		{
			public B(int integer, A a)
			{
				Integer = integer;
				A = @a;
			}

			public int Integer { get; set; }
			public A A { get; set; }
		}

    class DictionaryObj
    {
      public Dictionary<int, A> ObjectDictionary { get; set; }
    }

		[Fact]
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "We're testing the constructor!")]
		public void Constructor_ThrowsOnNullFunc()
		{
			Assert.Throws<ArgumentNullException>(() => { var comparer = new GenericEqualityComparer<A>(null); });
		}

		[Fact]
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "We're testing the constructor!")]
		public void Constructor_ThrowsOnNullHashGenerator()
		{
			Assert.Throws<ArgumentNullException>(() => { var comparer = new GenericEqualityComparer<A>((a, b) => a.Integer == b.Integer, null); });
		}

		[Fact]
		public void Compare_TrueOnMatchingObjectsWithSpecifiedPropertiesOnly()
		{
			A a = new A(1, "foo"),
				b = new A(1, "bar");
			Assert.Equal(a, b, A.IntegerOnlyComparer);
		}

		[Fact]
		public void Compare_TrueOnMatchingObjectsWithMultipleProperties()
		{
			A a = new A(1, "string"),
				b = new A(a.Integer, a.String);
			Assert.Equal(a, b, A.AllPropertiesComparer);
		}

		[Fact]
		public void Compare_FalseOnMismatchedObjects()
		{
			A a = new A(1, string.Empty),
				b = new A(2, string.Empty);
			Assert.NotEqual(a, b, A.IntegerOnlyComparer);
		}

		[Fact]
		public void ByAllProperties_TrueOnMatchedObjectInstances()
		{
			A a = new A(3, "Foo"),
			b = new A(3, "Foo");

			Assert.Equal(a, b, GenericEqualityComparer<A>.ByAllMembers());
		}

		[Fact]
		public void ByAllProperties_FalseOnUnMatchedObjectInstances()
		{
			A a = new A(5, "Foo"),
			b = new A(3, "Bar");

			Assert.NotEqual(a, b, GenericEqualityComparer<A>.ByAllMembers());
		}

		[Fact]
		public void ByAllProperties_TrueOnMatchedObjectInstancesWithCustomComparer()
		{
			B b = new B(6, new A(5, "Foo")),
			b2 = new B(6, new A(5, "Bar"));

			Assert.Equal(b, b2, GenericEqualityComparer<B>.ByAllMembers(new[] { A.IntegerOnlyComparer }));
		}

    [Fact]
    public void ByAllProperties_TrueOnMatchedDictionaryObjectInstances()
    {
      DictionaryObj dobj1 = new DictionaryObj { ObjectDictionary = new Dictionary<int, A> { { 1, new A(1, "one") }, { 2, new A(2, "two") } } };
      DictionaryObj dobj2 = new DictionaryObj { ObjectDictionary = new Dictionary<int, A> { { 1, new A(1, "one") }, { 2, new A(2, "two") } } };

      Assert.Equal(dobj1, dobj2, GenericEqualityComparer<DictionaryObj>.ByAllMembers());
    }
	}
}
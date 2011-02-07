using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace EPS.Utility.Tests.Unit
{
    public class GenericEqualityComparerTest
    {
        class A
        {
            public int Integer { get; set; }
            public string String { get; set; }

            public static GenericEqualityComparer<A> IntegerOnlyComparer = new GenericEqualityComparer<A>((a1, a2) => a1.Integer == a2.Integer);
            public static GenericEqualityComparer<A> AllPropertiesComparer = new GenericEqualityComparer<A>((a1, a2) => a1.Integer == a2.Integer && a1.String == a2.String);
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
            Assert.Throws<ArgumentNullException>(() => { var comparer = new GenericEqualityComparer<A>((a,b) => a.Integer == b.Integer, null); });
        }

        [Fact]
        public void Compare_TrueOnMatchingObjectsWithSpecifiedPropertiesOnly()
        {
            A a = new A() { Integer = 1, String = "foo" }, b = new A() { Integer = 1, String = "bar" };
            Assert.Equal(a, a, A.IntegerOnlyComparer);
        }

        [Fact]
        public void Compare_TrueOnMatchingObjectsWithMultipleProperties()
        {
            A a = new A() { Integer = 1, String = "string" }, b = new A() { Integer = 1, String = "string" };
            Assert.Equal(a, a, A.AllPropertiesComparer);
        }

        [Fact]
        public void Compare_FalseOnMismatchedObjects()
        {
            A a = new A() { Integer = 1 }, b = new A() { Integer = 2 };
            Assert.NotEqual(a, b, A.IntegerOnlyComparer);
        }
    }
}

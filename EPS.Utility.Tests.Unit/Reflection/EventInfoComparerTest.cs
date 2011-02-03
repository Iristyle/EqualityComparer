using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace EPS.Reflection.Tests.Unit
{
    public class EventInfoComparerTest
    {
        class A
        {
            public event EventHandler Test;
            public event EventHandler<UnhandledExceptionEventArgs> Test2;
        }

        class B
        {
            public event EventHandler Test;
            public event EventHandler<UnhandledExceptionEventArgs> Test2;
        }

        [Fact]
        public void Equals_True_OnTypesOfSameSignature()
        {
            Assert.True(typeof(A).GetEvents().SequenceEqual(typeof(B).GetEvents(), EventInfoComparer.Default));
        }

        [Fact]
        public void Equals_False_OnNullFirstParameter()
        {
            Assert.False(EventInfoComparer.Default.Equals(null, (EventInfo)typeof(A).GetMember("Test")[0]));
        }

        [Fact]
        public void Equals_False_OnNullSecondParameter()
        {
            Assert.False(EventInfoComparer.Default.Equals((EventInfo)typeof(A).GetMember("Test")[0], null));
        }
    }
}

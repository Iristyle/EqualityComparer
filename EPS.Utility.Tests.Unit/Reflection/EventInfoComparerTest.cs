using System;
using System.Linq;
using Xunit;

namespace EPS.Reflection.Tests.Unit
{
    public class EventInfoComparerTest
    {
        class TypeA
        {
            public event EventHandler Test;
        }

        class TypeB
        {
            public event EventHandler Test;
        }

        [Fact]
        public void Equals_True_OnTypesOfSameSignature()
        {
            Assert.True(typeof(TypeA).GetEvents().SequenceEqual(typeof(TypeB).GetEvents(), EventInfoComparer.Default));
        }
    }
}

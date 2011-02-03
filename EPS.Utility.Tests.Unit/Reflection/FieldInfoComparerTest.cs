using System;
using System.Linq;
using Xunit;

namespace EPS.Reflection.Tests.Unit
{
    public class FieldInfoComparerTest
    {
        class TypeA
        {
            public int Test = 0;
        }

        class TypeB
        {
            public int Test = 0;
        }

        [Fact]
        public void Equals_True_OnTypesOfSameSignature()
        {
            Assert.True(typeof(TypeA).GetFields().SequenceEqual(typeof(TypeB).GetFields(), FieldInfoComparer.Default));
        }
    }
}

using System;
using System.Linq;
using Xunit;

namespace EPS.Reflection.Tests.Unit
{
    public class ConstructorInfoComparerTest
    {
        class TypeA
        {
            public TypeA(int test) { }
        }

        class TypeB
        {
            public TypeB(int test) { }
        }

        [Fact]
        public void Equals_True_OnTypesOfSameSignature()
        {
            Assert.True(typeof(TypeA).GetConstructors().SequenceEqual(typeof(TypeB).GetConstructors(), ConstructorInfoComparer.Default));
        }

    }
}

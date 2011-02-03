using System;
using System.Linq;
using Xunit;

namespace EPS.Reflection.Tests.Unit
{
    public class MemberInfoComparerTest
    {
        class TypeA
        {
            public int Test { get; set; }
        }

        class TypeB
        {
            public int Test { get; set; }
        }

        [Fact]
        public void Equals_True_ForIdenticalTypes()
        {
            Assert.True(typeof(TypeA).GetMembers().SequenceEqual(typeof(TypeB).GetMembers(), MemberInfoComparer.Default));
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace EqualityComparer.Reflection.Tests
{
  public class FieldInfoComparerTest
  {
    class TypeA
    {
      [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Test Code")]
      public int Test = 0;
    }

    class TypeB
    {
      [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Test Code")]
      public int Test = 0;
    }

    [Fact]
    public void Equals_True_OnTypesOfSameSignature()
    {
      Assert.True(typeof(TypeA).GetFields().SequenceEqual(typeof(TypeB).GetFields(), FieldInfoComparer.Default));
    }
  }
}

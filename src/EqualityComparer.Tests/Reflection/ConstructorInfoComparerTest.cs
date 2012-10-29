using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace EqualityComparer.Reflection.Tests
{
  public class ConstructorInfoComparerTest
  {
    class TypeA
    {
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public TypeA(int test) { }
    }

    class TypeB
    {
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public TypeB(int test) { }
    }

    [Fact]
    public void Equals_True_OnTypesOfSameSignature()
    {
      Assert.True(typeof(TypeA).GetConstructors().SequenceEqual(typeof(TypeB).GetConstructors(), ConstructorInfoComparer.Default));
    }
  }
}

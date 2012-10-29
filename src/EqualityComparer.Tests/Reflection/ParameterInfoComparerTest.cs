using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace EqualityComparer.Reflection.Tests
{
  public class ParameterInfoComparerTest
  {
    class A
    {
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public void Test(int input, string input2) { }
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public void TestCopy(int input, string input2) { }
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public void TestOverload(string input, string input2) { }
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public void TestRearranged(string input, int input2) { }
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public void TestWithOptional(int input, string input2 = "optional") { }
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public void TestWithRef(int input, ref string input2) { }
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public void TestWithOut(int input, out string input2) { input2 = "out"; }
    }

    [Fact]
    public void Equals_True_OnIdenticalParameter()
    {
      Assert.True(ParameterInfoComparer.Default.Equals(typeof(A).GetMethod("Test").GetParameters()[0],
        typeof(A).GetMethod("Test").GetParameters()[0]));
    }

    [Fact]
    public void Equals_True_OnIdenticallyTypedParameters()
    {
      Assert.True(typeof(A).GetMethod("Test").GetParameters()
        .SequenceEqual(typeof(A).GetMethod("TestCopy").GetParameters(), ParameterInfoComparer.Default));
    }

    [Fact]
    public void Equals_False_OnSameParameterNamesAndOrderButDifferentTypes()
    {
      Assert.False(typeof(A).GetMethod("Test").GetParameters()
        .SequenceEqual(typeof(A).GetMethod("TestOverload").GetParameters(), ParameterInfoComparer.Default));
    }

    [Fact]
    public void Equals_False_OnReorderedParameters()
    {
      Assert.False(typeof(A).GetMethod("Test").GetParameters()
        .SequenceEqual(typeof(A).GetMethod("TestRearranged").GetParameters(), ParameterInfoComparer.Default));
    }

    [Fact]
    public void Equals_False_OnParametersDifferingByOptional()
    {
      Assert.False(typeof(A).GetMethod("Test").GetParameters()
        .SequenceEqual(typeof(A).GetMethod("TestWithOptional").GetParameters(), ParameterInfoComparer.Default));
    }

    [Fact]
    public void Equals_False_OnParametersDifferingByRef()
    {
      Assert.False(typeof(A).GetMethod("Test").GetParameters()
        .SequenceEqual(typeof(A).GetMethod("TestWithRef").GetParameters(), ParameterInfoComparer.Default));
    }

    [Fact]
    public void Equals_False_OnParametersDifferingByOut()
    {
      Assert.False(typeof(A).GetMethod("Test").GetParameters()
        .SequenceEqual(typeof(A).GetMethod("TestWithOut").GetParameters(), ParameterInfoComparer.Default));
    }

    [Fact]
    public void Equals_False_OnNullFirstParameter()
    {
      Assert.False(ParameterInfoComparer.Default.Equals(null, typeof(A).GetMethod("Test").GetParameters()[0]));
    }

    [Fact]
    public void Equals_False_OnNullSecondParameter()
    {
      Assert.False(ParameterInfoComparer.Default.Equals(typeof(A).GetMethod("Test").GetParameters()[0]), null);
    }
  }
}

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace EqualityComparer.Reflection.Tests
{
  public class MethodInfoComparerTest
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

    class B
    {
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public void Test(int input, string input2) { }
    }

    class C<T> : A
    {
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Test Code")]
      public void TestGeneric(T input) { }
    }

    [Fact]
    public void Equals_True_OnIdenticalMethod()
    {
      Assert.True(MethodInfoComparer.Default.Equals(typeof(A).GetMethod("Test"),
        typeof(A).GetMethod("Test")));
    }

    [Fact]
    public void Equals_True_OnIdenticalMethodSignaturesOnDifferentTypes()
    {
      Assert.True(MethodInfoComparer.Default.Equals(typeof(A).GetMethod("Test"),
        typeof(B).GetMethod("Test")));
    }

    [Fact]
    public void Equals_False_OnIdenticalMethodSignaturesWithDifferentNames()
    {
      Assert.False(MethodInfoComparer.Default.Equals(typeof(A).GetMethod("Test"),
        typeof(A).GetMethod("TestCopy")));
    }

    [Fact]
    public void Equals_False_OnSameParameterNamesAndOrderButDifferentTypes()
    {
      Assert.False(MethodInfoComparer.Default.Equals(typeof(A).GetMethod("Test"),
        typeof(A).GetMethod("TestOverload")));
    }

    [Fact]
    public void Equals_False_OnReorderedParameters()
    {
      Assert.False(MethodInfoComparer.Default.Equals(typeof(A).GetMethod("Test"),
        typeof(A).GetMethod("TestRearranged")));
    }

    [Fact]
    public void Equals_False_OnParametersDifferingByOptional()
    {
      Assert.False(MethodInfoComparer.Default.Equals(typeof(A).GetMethod("Test"),
        typeof(A).GetMethod("TestWithOptional")));
    }

    [Fact]
    public void Equals_False_OnParametersDifferingByRef()
    {
      Assert.False(MethodInfoComparer.Default.Equals(typeof(A).GetMethod("Test"),
        typeof(A).GetMethod("TestWithRef")));
    }

    [Fact]
    public void Equals_False_OnParametersDifferingByOut()
    {
      Assert.False(MethodInfoComparer.Default.Equals(typeof(A).GetMethod("Test"),
        typeof(A).GetMethod("TestWithOut")));
    }

    [Fact]
    public void Equals_False_OnNullFirstParameter()
    {
      Assert.False(MethodInfoComparer.Default.Equals(null, typeof(A).GetMethod("Test")));
    }

    [Fact]
    public void Equals_False_OnNullSecondParameter()
    {
      Assert.False(MethodInfoComparer.Default.Equals(typeof(A).GetMethod("Test"), null));
    }
  }
}

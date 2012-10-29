using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace EqualityComparer.Reflection.Tests
{
  public class EventInfoComparerTest
  {
    class A
    {
      public event EventHandler Test;
      public event EventHandler<UnhandledExceptionEventArgs> Test2;

      //placeholders to remove compiler warnings
      private void CallTest() { var TestCopy = Test; if (null != TestCopy) { TestCopy(this, EventArgs.Empty); } }
      private void CallTest2() { var TestCopy2 = Test2; if (null != TestCopy2) { TestCopy2(this, new UnhandledExceptionEventArgs(new DivideByZeroException(), false)); } }
    }

    class B
    {
      public event EventHandler Test;
      public event EventHandler<UnhandledExceptionEventArgs> Test2;

      //placeholders to remove compiler warnings
      private void CallTest() { var TestCopy = Test; if (null != TestCopy) { TestCopy(this, EventArgs.Empty); } }
      private void CallTest2() { var TestCopy2 = Test2; if (null != TestCopy2) { TestCopy2(this, new UnhandledExceptionEventArgs(new DivideByZeroException(), false)); } }
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

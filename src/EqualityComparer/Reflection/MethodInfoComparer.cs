using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace EqualityComparer.Reflection
{
  /// <summary>   This is a *very* rudimentary comparison routine that examines two MethodInfo definitions for signature compatibility. </summary>
  /// <remarks>   ebrown, 2/3/2011. </remarks>
  public class MethodInfoComparer : EqualityComparer<MethodInfo>
  {
    private static Lazy<MethodInfoComparer> _default = new Lazy<MethodInfoComparer>(() => new MethodInfoComparer());

    /// <summary>   Gets the default MethodInfoComparer instance, rather than continually constructing new instances. </summary>
    /// <value> The default. </value>
    public static new MethodInfoComparer Default { get { return _default.Value; } }

    /// <summary>   Tests if two MethodInfo objects are considered equal. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="x">    MethodInfo instance to be compared. </param>
    /// <param name="y">    MethodInfo instance to be compared. </param>
    /// <returns>   true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(MethodInfo x, MethodInfo y)
    {
      if (x == y) { return true; }
      if ((x == null) || (y == null)) { return false; }

      if (x.Name != y.Name)
      {
        return false;
      }

      Type xReturnType = x.ReturnType,
        yReturnType = y.ReturnType;

      //comparing ReturnType doesn't work on generic methods -- so we have to do things a little different
      if (x.IsGenericMethod && y.IsGenericMethod)
      {
        if (xReturnType.IsGenericType && yReturnType.IsGenericType)
          return (xReturnType.GetGenericTypeDefinition() == yReturnType.GetGenericTypeDefinition())
            && x.GetParameters().SequenceEqual(y.GetParameters(), ParameterInfoComparer.Default);

        //match type names
        if (xReturnType.IsGenericParameter && yReturnType.IsGenericParameter)
          return (xReturnType.Name == yReturnType.Name
          && ((!x.GetParameters().Any() && !y.GetParameters().Any())
          || x.GetParameters().SequenceEqual(y.GetParameters(), ParameterInfoComparer.Default)));
      }

      //return types match and there are 0 params or param list sequences match
      return (xReturnType == yReturnType
        && ((!x.GetParameters().Any() && !y.GetParameters().Any())
        || x.GetParameters().SequenceEqual(y.GetParameters(), ParameterInfoComparer.Default)));
    }

    /// <summary>   Calculates the hash code for this object. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="obj">  The object. </param>
    /// <returns>   The hash code for this object. </returns>
    public override int GetHashCode(MethodInfo obj)
    {
      if (null == obj) { return 0; }
      return string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", obj.MemberType, obj.Name, obj.ReturnType).GetHashCode();
    }
  }
}

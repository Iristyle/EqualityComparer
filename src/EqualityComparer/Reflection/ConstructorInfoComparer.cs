using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace EqualityComparer.Reflection
{
  /// <summary>   This is a *very* rudimentary comparison routine that examines two ConstructorInfo definitions for signature compatibility. </summary>
  /// <remarks>   ebrown, 2/3/2011. </remarks>
  public class ConstructorInfoComparer : EqualityComparer<ConstructorInfo>
  {
    private static Lazy<ConstructorInfoComparer> _default = new Lazy<ConstructorInfoComparer>(() => new ConstructorInfoComparer());

    /// <summary>   Gets the default ConstructorInfoComparer instance, rather than continually constructing new instances. </summary>
    /// <value> The default. </value>
    public static new ConstructorInfoComparer Default { get { return _default.Value; } }

    /// <summary>
    /// Tests if two ConstructorInfo objects are considered equal by our definition
    ///  -- same Name, CallingConvention, Abstract, Final, Private, Public, Static, Virtual, Attributes and matching parameters order as defined by
    ///  ParameterInfoComparer.
    /// </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="x">    ConstructorInfo to be compared. </param>
    /// <param name="y">    ConstructorInfo to be compared. </param>
    /// <returns>   true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(ConstructorInfo x, ConstructorInfo y)
    {
      if (x == y) { return true; }
      if ((x == null) || (y == null)) { return false; }

      var basicRules = (x.Name == y.Name &&
        x.CallingConvention == y.CallingConvention &&
        x.IsAbstract == y.IsAbstract &&
        x.IsFinal == y.IsFinal &&
        x.IsPrivate == y.IsPrivate &&
        x.IsPublic == y.IsPublic &&
        x.IsStatic == y.IsStatic &&
        x.IsVirtual == y.IsVirtual &&
        x.Attributes == y.Attributes);

      if (!basicRules) { return false; }

      //generic constructors do things a little different
      if (x.IsGenericMethod && y.IsGenericMethod)
      {
        var xArgs = x.GetGenericArguments();
        var yArgs = y.GetGenericArguments();

        bool genericsMatch = ((!xArgs.Any() && !yArgs.Any()) || xArgs.SequenceEqual(yArgs));
        if (!genericsMatch) { return false; }
      }

      ParameterInfo[] xParameters = x.GetParameters(),
        yParameters = y.GetParameters();

      //return types match and there are 0 params or param list sequences match
      return ((!xParameters.Any() && !yParameters.Any())
        || xParameters.SequenceEqual(yParameters, ParameterInfoComparer.Default));
    }

    /// <summary>   Calculates the hash code for this object. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="obj">  The object. </param>
    /// <returns>   The hash code for this object. </returns>
    public override int GetHashCode(ConstructorInfo obj)
    {
      if (null == obj) { return 0; }
      return string.Format(CultureInfo.CurrentCulture, "{0}{1}", obj.MemberType, obj.Name).GetHashCode();
    }
  }
}

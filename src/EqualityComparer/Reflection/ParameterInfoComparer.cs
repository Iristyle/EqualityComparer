using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace EqualityComparer.Reflection
{
  /// <summary>   This is a *very* rudimentary comparison routine that examines two ParameterInfo definitions for signature compatibility. </summary>
  /// <remarks>   ebrown, 2/3/2011. </remarks>
  public class ParameterInfoComparer : EqualityComparer<ParameterInfo>
  {
    private static Lazy<ParameterInfoComparer> _default = new Lazy<ParameterInfoComparer>(() => new ParameterInfoComparer());

    /// <summary>   Gets the default ParameterInfoComparer instance, rather than continually constructing new instances. </summary>
    /// <value> The default. </value>
    public static new ParameterInfoComparer Default { get { return _default.Value; } }

    /// <summary>
    /// Tests if two ParameterInfo objects are considered equal
    /// -- same DefaultValue, IsIn, IsOptional, IsRetval, ParameterType, Position.
    /// </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="x">    ParameterInfo instance to be compared. </param>
    /// <param name="y">    ParameterInfo instance to be compared. </param>
    /// <returns>   true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(ParameterInfo x, ParameterInfo y)
    {
      if (x == y) { return true; }
      if ((x == null) || (y == null)) { return false; }

      return (x.Name == y.Name &&
        x.DefaultValue == y.DefaultValue &&
        x.IsIn == y.IsIn &&
        x.IsOptional == y.IsOptional &&
        x.IsOut == y.IsOut &&
        x.IsRetval == y.IsRetval &&
        x.ParameterType == y.ParameterType &&
        x.Position == y.Position);
    }

    /// <summary>   Calculates the hash code for this object. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="obj">  The object. </param>
    /// <returns>   The hash code for this object. </returns>
    public override int GetHashCode(ParameterInfo obj)
    {
      if (null == obj) { return 0; }
      return (String.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}", obj.Name, obj.DefaultValue, obj.ParameterType, obj.Position)).GetHashCode();
    }
  }
}

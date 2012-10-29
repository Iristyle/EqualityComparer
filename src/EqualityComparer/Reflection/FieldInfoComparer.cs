using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace EqualityComparer.Reflection
{
  /// <summary>   A rudimentary FieldInfo comparer that checks for equivalency. </summary>
  /// <remarks>   ebrown, 2/3/2011. </remarks>
  public class FieldInfoComparer : EqualityComparer<FieldInfo>
  {
    private static Lazy<FieldInfoComparer> _default = new Lazy<FieldInfoComparer>(() => new FieldInfoComparer());

    /// <summary>   Gets the default FieldInfoComparer instance, rather than continually constructing new instances. </summary>
    /// <value> The default. </value>
    public static new FieldInfoComparer Default { get { return _default.Value; } }

    /// <summary>
    /// Tests if two FieldInfo objects are considered equal by our definition -- same Name, FieldType, InitOnly, Literal, Pinvoke, Private /
    /// Public, NotSerialized, Static, Attributes.
    /// </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="x">    Field information to be compared. </param>
    /// <param name="y">    Field information to be compared. </param>
    /// <returns>   true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(FieldInfo x, FieldInfo y)
    {
      if (x == y) { return true; }
      if ((x == null) || (y == null)) { return false; }

      return (x.Name == y.Name &&
        x.FieldType == y.FieldType &&
        x.IsInitOnly == y.IsInitOnly &&
        x.IsLiteral == y.IsLiteral &&
        x.IsPinvokeImpl == y.IsPinvokeImpl &&
        x.IsPrivate == y.IsPrivate &&
        x.IsPublic == y.IsPublic &&
        x.IsNotSerialized == y.IsNotSerialized &&
        x.IsStatic == y.IsStatic &&
        x.Attributes == y.Attributes);
    }

    /// <summary>   Calculates the hash code for this object. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="obj">  The object. </param>
    /// <returns>   The hash code for this object. </returns>
    public override int GetHashCode(FieldInfo obj)
    {
      if (null == obj) { return 0; }
      return string.Format(CultureInfo.CurrentCulture, "Member:{0}Name:{1}FieldType:{2}", obj.MemberType, obj.Name, obj.FieldType).GetHashCode();
    }
  }
}

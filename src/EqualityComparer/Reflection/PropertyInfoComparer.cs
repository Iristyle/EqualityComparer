using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace EqualityComparer.Reflection
{
  /// <summary>   A class that performs a rudimentary comparison of PropertyInfo instances. </summary>
  /// <remarks>   ebrown, 2/3/2011. </remarks>
  public class PropertyInfoComparer : EqualityComparer<PropertyInfo>
  {
    private static Lazy<PropertyInfoComparer> _default = new Lazy<PropertyInfoComparer>(() => new PropertyInfoComparer());

    /// <summary>   Gets the default PropertyInfoComparer instance, rather than continually constructing new instances. </summary>
    /// <value> The default. </value>
    public static new PropertyInfoComparer Default { get { return _default.Value; } }

    /// <summary>
    /// Tests if two PropertyInfo objects are considered equal by our definition -- same Name, PropertyType, CanRead / CanWrite, Attributes.
    /// </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="x">    PropertyInfo to be compared. </param>
    /// <param name="y">    PropertyInfo to be compared. </param>
    /// <returns>   true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(PropertyInfo x, PropertyInfo y)
    {
      if (x == y) { return true; }
      if ((x == null) || (y == null)) { return false; }

      return (x.Name == y.Name &&
        x.PropertyType == y.PropertyType &&
        x.CanRead == y.CanRead &&
        x.CanWrite == y.CanWrite &&
        x.Attributes == y.Attributes);
    }

    /// <summary>   Calculates the hash code for this object. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="obj">  The object. </param>
    /// <returns>   The hash code for this object. </returns>
    public override int GetHashCode(PropertyInfo obj)
    {
      if (null == obj) { return 0; }
      return string.Format(CultureInfo.CurrentCulture, "Member:{0}Name:{1}PropertyType:{2}", obj.MemberType, obj.Name, obj.PropertyType).GetHashCode();
    }
  }
}

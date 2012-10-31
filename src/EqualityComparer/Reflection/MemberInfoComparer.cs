using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace EqualityComparer.Reflection
{
  /// <summary>   A rudimentary comparison routine that Member information comparer. </summary>
  /// <remarks>   ebrown, 2/3/2011. </remarks>
  public class MemberInfoComparer : EqualityComparer<MemberInfo>
  {
    private static Lazy<MemberInfoComparer> _default = new Lazy<MemberInfoComparer>(() => new MemberInfoComparer());
    private static Lazy<MemberInfoComparer> _ignoreCustom = new Lazy<MemberInfoComparer>(() => new MemberInfoComparer(MemberTypes.Custom, MemberTypes.NestedType, MemberTypes.TypeInfo));

    /// <summary>   Gets the default MemberInfoComparer instance, rather than continually constructing new instances. </summary>
    /// <value> The default. </value>
    public static new MemberInfoComparer Default { get { return _default.Value; } }

    /// <summary>
    /// Gets the MemberInfoComparer instance that will ignore MemberTypes.Custom, MemberTypes.NestedType and MemberTypes.TypeInfo, rather
    /// than continually constructing new instances.
    /// </summary>
    /// <value> A MemberInfoComparer following the specified rules. </value>
    public static MemberInfoComparer IgnoreNestedTypes { get { return _ignoreCustom.Value; } }

    private MemberTypes[] _memberTypeIgnores;

    /// <summary>
    /// Initializes a new instance of the MemberInfoComparer class.
    /// </summary>
    public MemberInfoComparer()
    { }

    /// <summary>   Initializes a new instance of the MemberInfoComparer class. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="ignores">  A variable-length parameters list containing MemberTypes to ignores. </param>
    public MemberInfoComparer(params MemberTypes[] ignores)
    {
      if (null == ignores) { throw new ArgumentNullException("ignores"); }

      this._memberTypeIgnores = ignores;
      if (null != ignores)
      {
        if (ignores.Contains(MemberTypes.All))
        {
          throw new ArgumentException("MemberTypes.All makes no sense within this context", "ignores");
        }
      }
    }

    /// <summary>   Tests if two MemberInfo objects are considered equal. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="x">    MemberInfo instance to be compared. </param>
    /// <param name="y">    MemberInfo instance to be compared. </param>
    /// <returns>   true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(MemberInfo x, MemberInfo y)
    {
      if (x == y) { return true; }
      if ((x == null) || (y == null)) { return false; }
      MemberTypes xMemberType = x.MemberType,
        yMemberType = y.MemberType;
      if (xMemberType != yMemberType) { return false; }

      //check against our ignore list -- return true if we're ignoring these types
      if (null != this._memberTypeIgnores && this._memberTypeIgnores.Contains(xMemberType)) { return true; }

      if (x.Name != y.Name) { return false; }

      switch (xMemberType)
      {
        case MemberTypes.Constructor:
          return ConstructorInfoComparer.Default.Equals((ConstructorInfo)x, (ConstructorInfo)y);
        case MemberTypes.Event:
          return EventInfoComparer.Default.Equals((EventInfo)x, (EventInfo)y);
        case MemberTypes.Field:
          return FieldInfoComparer.Default.Equals((FieldInfo)x, (FieldInfo)y);
        case MemberTypes.Method:
          return MethodInfoComparer.Default.Equals((MethodInfo)x, (MethodInfo)y);
        case MemberTypes.Property:
          return PropertyInfoComparer.Default.Equals((PropertyInfo)x, (PropertyInfo)y);

        //compare the NestedTypes for compatibility based on their Members (effectively making this a recursive call)
        case MemberTypes.NestedType:
          var xMembers = ((Type)x).GetMembers();
          var yMembers = ((Type)y).GetMembers();

          //empty list of members
          return ((!xMembers.Any() && !yMembers.Any())
            //or the same list of members (using the same criteria that we're currently using)
            || (!xMembers.Except(yMembers, this).Any() && !yMembers.Except(xMembers, this).Any()));

        default:
        case MemberTypes.Custom:
        case MemberTypes.TypeInfo:
          return false;
      }
    }

    /// <summary>   Calculates the hash code for this object. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="obj">  The object. </param>
    /// <returns>   The hash code for this object. </returns>
    public override int GetHashCode(MemberInfo obj)
    {
      if (null == obj) { return 0; }
      return string.Format(CultureInfo.CurrentCulture, "{0}{1}", obj.MemberType, obj.Name).GetHashCode();
    }
  }
}

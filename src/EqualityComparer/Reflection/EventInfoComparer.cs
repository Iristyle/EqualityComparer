using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace EqualityComparer.Reflection
{
  /// <summary>   Event information comparer. </summary>
  /// <remarks>   ebrown, 2/3/2011. </remarks>
  public class EventInfoComparer : EqualityComparer<EventInfo>
  {
    private static Lazy<EventInfoComparer> _default = new Lazy<EventInfoComparer>(() => new EventInfoComparer());

    /// <summary>   Gets the default EventInfoComparer instance, rather than continually constructing new instances. </summary>
    /// <value> The default. </value>
    public static new EventInfoComparer Default { get { return _default.Value; } }

    /// <summary>   Tests if two EventInfo objects are considered equal by our definition -- same Name, EventHandlerType, IsMulticast, Attributes. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="x">    EventInfo instance to be compared. </param>
    /// <param name="y">    EventInfo instance to be compared. </param>
    /// <returns>   true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(EventInfo x, EventInfo y)
    {
      if (x == y) { return true; }
      if ((x == null) || (y == null)) { return false; }

      return (x.Name == y.Name &&
        x.EventHandlerType == y.EventHandlerType &&
        x.IsMulticast == y.IsMulticast &&
        x.Attributes == y.Attributes);
    }

    /// <summary>   Calculates the hash code for this object. </summary>
    /// <remarks>   ebrown, 2/3/2011. </remarks>
    /// <param name="obj">  The object. </param>
    /// <returns>   The hash code for this object. </returns>
    public override int GetHashCode(EventInfo obj)
    {
      if (null == obj) { return 0; }
      return string.Format(CultureInfo.CurrentCulture, "Member:{0}Name:{1}EventHandlerType:{2}", obj.MemberType, obj.Name, obj.EventHandlerType).GetHashCode();
    }
  }
}

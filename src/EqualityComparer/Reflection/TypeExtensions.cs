using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EqualityComparer.Reflection
{
  /// <summary>   A set of useful extension methods built on top of <see cref="T:System.Type"/> </summary>
  /// <remarks>   ebrown, 11/9/2010. </remarks>
  public static class TypeExtensions
  {
    /// <summary>   Determines whether a given type implements a specified interface, where the interface *must* be generic. </summary>
    /// <remarks>   ebrown, 11/9/2010. </remarks>
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
    /// <exception cref="ArgumentException">        Thrown when one or more arguments have unsupported or illegal values. </exception>
    /// <param name="interfaceType">    The Type of the interface to search for. </param>
    /// <param name="concreteType">     The concrete object Type to examine. </param>
    /// <returns>   <c>true</c> if the concrete type specified implements the interface type specified; otherwise, <c>false</c>. </returns>
    public static bool IsGenericInterfaceAssignableFrom(this Type interfaceType, Type concreteType)
    {
      return GetGenericInterfaces(interfaceType, concreteType).Any();
    }

    /// <summary>
    /// For a given type implementing a specified interface, where the interface *must* be generic, this returns the parameters passed to the
    /// generic interface.
    /// </summary>
    /// <remarks>   ebrown, 11/9/2010. </remarks>
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
    /// <exception cref="ArgumentException">        Thrown when one or more arguments have unsupported or illegal values. </exception>
    /// <param name="interfaceType">    The Type of the interface to search for. </param>
    /// <param name="concreteType">     The concrete object Type to examine. </param>
    /// <returns>   An enumeration of the Types being used in the generic interface declaration. </returns>
    public static IEnumerable<Type> GetGenericInterfaceTypeParameters(this Type interfaceType, Type concreteType)
    {
      var implementedInterface = GetGenericInterfaces(interfaceType, concreteType).FirstOrDefault();
      if (implementedInterface == default(Type))
      {
        throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Interface {0} not implemented by {1}", interfaceType, concreteType), "concreteType");
      }

      return implementedInterface.GetGenericArguments();
    }

    private static IEnumerable<Type> GetGenericInterfaces(Type interfaceType, Type concreteType)
    {
      if (null == interfaceType) { throw new ArgumentNullException("interfaceType"); }
      if (null == concreteType) { throw new ArgumentNullException("concreteType"); }

      if (!interfaceType.IsGenericType || !interfaceType.IsInterface)
      {
        throw new ArgumentException("interfaceType must be a generic interface such as IInterface<T>");
      }

      return concreteType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
    }
    /// <summary>   Determines whether the specified type is anonymous. </summary>
    /// <remarks>   ebrown, 11/9/2010. </remarks>
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
    /// <param name="type"> The type. </param>
    /// <returns>   <c>true</c> if the specified type is anonymous; otherwise, <c>false</c>. </returns>
    public static bool IsAnonymous(this Type type)
    {
      if (type == null) { throw new ArgumentNullException("type"); }

      string typeName = type.Name;
      // HACK: The only way to detect anonymous types right now.
      return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
             && type.IsGenericType && typeName.Contains("AnonymousType")
             && (typeName.StartsWith("<>", StringComparison.OrdinalIgnoreCase) || typeName.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
             && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
    }

    /// <summary>   Determines whether the specified object is anonymous. </summary>
    /// <remarks>   ebrown, 11/9/2010. </remarks>
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
    /// <param name="value">    The object to inspect. </param>
    /// <returns>   <c>true</c> if the specified object is based on an anonymous type; otherwise, <c>false</c>. </returns>
    public static bool IsAnonymous<T>(this T value)
    {
      if (null == value) { throw new ArgumentNullException("value"); }

      return IsAnonymous(typeof(T));
    }


    /// <summary>   A Type extension method that gets all base types and interfaces for a given type by recursing the type hierarchy. </summary>
    /// <remarks>
    /// Given Type is explored for all derived types and interfaces. Types are returned in the following depth order:
    /// - The type itself at depth 0
    /// - Implemented interfaces at depth 1
    /// - All based types, in order of derivation with an appropriate depth of 2+.
    /// </remarks>
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
    /// <param name="type"> The type. </param>
    /// <returns>   all base types and interfaces in order of depth. </returns>
    public static IDictionary<Type, int> GetAllBaseTypesAndInterfaces(this Type type)
    {
      if (null == type) { throw new ArgumentNullException("type"); }

      //start with interfaces at depth of 1
      var types = type.GetInterfaces().ToDictionary(i => i, i => 1);
      //original type at depth of 0
      types.Add(type, 0);

      //base types at depth of 2+ up the tree -- if we're object / have no BaseType
      RecurseTypeHierarchy(type.BaseType, types, 2);

      return types;
    }

    private static void RecurseTypeHierarchy(Type derivedType, IDictionary<Type, int> discoveredTypes, int depth)
    {
      if (null == derivedType) { return; }

      if (derivedType.BaseType != null)
      {
        //plow all the way to the bottom -- largest depth further down
        RecurseTypeHierarchy(derivedType.BaseType, discoveredTypes, depth + 1);
      }
      discoveredTypes.Add(derivedType, depth);
    }
  }
}

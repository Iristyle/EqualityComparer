using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EPS.Reflection
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
            if (null == interfaceType) { throw new ArgumentNullException("interfaceType"); }
            if (null == concreteType) { throw new ArgumentNullException("concreteType"); }
            
            if (!interfaceType.IsGenericType || !interfaceType.IsInterface)
                throw new ArgumentException("interfaceType must be a generic interface such as IInterface<T>");

            return concreteType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
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
            if (null == interfaceType) { throw new ArgumentNullException("interfaceType"); }
            if (null == concreteType) { throw new ArgumentNullException("concreteType"); }

            if (!interfaceType.IsGenericType || !interfaceType.IsInterface)
            {
                throw new ArgumentException("interfaceType must be a generic interface such as IInterface<T>");
            }

            var interfaces = concreteType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType).ToList();
            if (interfaces.Count == 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Interface {0} not implemented by {1}", interfaceType, concreteType), "concreteType");
            }

            return interfaces[0].GetGenericArguments();
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
        [SuppressMessage("Gendarme.Rules.Design.Linq", "AvoidExtensionMethodOnSystemObjectRule", Justification = "Someone cares about VB.Net?")]
        public static bool IsAnonymous(this object value)
        {
            if (null == value) { throw new ArgumentNullException("value"); }

            return IsAnonymous(value.GetType());
        }
    }
}

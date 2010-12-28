using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EPS.Reflection
{
    /// <summary>
    /// Provides a way to find all extensions methods in assemblies, extension methods for a given type, or
    /// a way to filter the list of extensions methods based on MethodInfo.
    /// </summary>
    public static class ExtensionMethodSearcher
    {
        /// <summary>   Gets the extension methods available in all currently loaded assemblies within the AppDomain that apply to a given type. </summary>
        /// <remarks>   Collection is not cached. ebrown, 11/9/2010. </remarks>
        /// <typeparam name="T">    The Type to inspect. </typeparam>
        /// <returns>   A <see cref="System.Collections.Generic.List{MethodInfo}"/> with all the extension methods. </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Convenience Overload / Acceptable usage since we're dealing with Types")]
        [SuppressMessage("Gendarme.Rules.Design.Generic", "AvoidMethodWithUnusedGenericTypeRule", Justification = "Convenience Overload / Acceptable usage since we're dealing with Types")]        
        public static IList<MethodInfo> GetExtensionMethodsInCurrentAssemblies<T>()
        {
            return GetExtensionMethodsInCurrentAssemblies(typeof(T));
        }

        /// <summary>   Gets the extension methods available in all currently loaded assemblies within the AppDomain that apply to a given type. </summary>
        /// <remarks>   Collection is not cached.  ebrown, 11/9/2010. </remarks>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="extendedType"> The Type being extended with extension methods. </param>
        /// <returns>   A <see cref="System.Collections.Generic.List{MethodInfo}"/> with all the extension methods. </returns>
        public static IList<MethodInfo> GetExtensionMethodsInCurrentAssemblies(this Type extendedType)
        {
            if (null == extendedType)
                throw new ArgumentNullException("extendedType");

            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel())
            {
                lock (methods)
                    methods.AddRange(GetExtensionMethods(assembly, extendedType));
            }

            return methods;
        }

        /// <summary>   Gets the extension methods available in all currently loaded assemblies within the AppDomain that apply to a given type. </summary>
        /// <remarks>   Collection is not cached.  ebrown, 11/9/2010. </remarks>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="extendedType"> The Type being extended with extension methods. </param>
        /// <param name="selector">     The filtering function used to inspect the methods when building the list. </param>
        /// <returns>   A <see cref="System.Collections.Generic.List{MethodInfo}"/> with all the extension methods. </returns>
        public static IList<MethodInfo> GetExtensionMethodsInCurrentAssemblies(this Type extendedType, Func<MethodInfo, bool> selector)
        {
            if (null == extendedType)
                throw new ArgumentNullException("extendedType");
            if (null == selector)
                throw new ArgumentNullException("selector");

            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel())
            {
                lock (methods)
                    methods.AddRange(GetExtensionMethods(assembly, extendedType, selector));
            }

            return methods;
        }

        /// <summary>   Gets the extension methods available in a given assembly that apply to a given type. </summary>
        /// <remarks>   IEnumerable is not cached.  ebrown, 11/9/2010. </remarks>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="assembly">     The given assembly. </param>
        /// <param name="extendedType"> The Type being extended with extension methods. </param>
        /// <returns>   A <see cref="System.Collections.Generic.List{MethodInfo}"/> with all the extension methods. </returns>
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Assembly assembly, Type extendedType)
        {
            if (null == assembly)
                throw new ArgumentNullException("assembly");
            if (null == extendedType)
                throw new ArgumentNullException("extendedType");

            var query = from type in assembly.GetTypes().AsParallel()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        select method;
            return query;
        }

        /// <summary>   Gets the extension methods available in a given assembly that apply to a given type. </summary>
        /// <remarks>   IEnumerable is not cached.  ebrown, 11/9/2010. </remarks>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="assembly">     The given assembly. </param>
        /// <param name="extendedType"> The Type being extended with extension methods. </param>
        /// <param name="selector">     The filtering function used to inspect the methods when building the list. </param>
        /// <returns>   A <see cref="System.Collections.Generic.List{MethodInfo}"/> with all the extension methods. </returns>
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Assembly assembly, Type extendedType, Func<MethodInfo, bool> selector)
        {
            if (null == assembly)
                throw new ArgumentNullException("assembly");
            if (null == extendedType)
                throw new ArgumentNullException("extendedType");
            if (null == selector)
                throw new ArgumentNullException("selector");

            var query = from type in assembly.GetTypes().AsParallel()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        where selector.Invoke(method)
                        select method;
            return query;
        } 
    }
}

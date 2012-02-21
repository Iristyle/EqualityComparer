using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

//TODO: seriously consider doing some caching here so that we're not constantly performing reflection
namespace EqualityComparer.Reflection
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
        public static IEnumerable<MethodInfo> GetExtensionMethodsForCurrentAssemblies<T>()
        {
            return GetExtensionMethodsForCurrentAssemblies(typeof(T));
        }

        /// <summary>   Gets the extension methods available in all currently loaded assemblies within the AppDomain that apply to a given type. </summary>
        /// <remarks>   
        /// IEnumerable is not cached.  Given Type is explored for all derived types and interfaces. Items are returned in alphabetical order
        /// after the following order:
        /// - Extensions that apply to the specified type
        /// - Extensions that apply to interfaces implemented by the specified type
        /// - Extensions apply to each derived type, in order of derivation. 
        /// </remarks>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="extendedType"> The Type being extended with extension methods. </param>
        /// <returns>   A <see cref="System.Collections.Generic.IEnumerable{MethodInfo}"/> with all the extension methods. </returns>
        public static IEnumerable<MethodInfo> GetExtensionMethodsForCurrentAssemblies(this Type extendedType)
        {
            return GetExtensionMethods(AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).AsParallel(), extendedType, m => true);
        }

        /// <summary>   Gets the extension methods available in all currently loaded assemblies within the AppDomain that apply to a given type. </summary>
        /// <remarks>   
        /// IEnumerable is not cached.  Given Type is explored for all derived types and interfaces. Items are returned in alphabetical order
        /// after the following order:
        /// - Extensions that apply to the specified type
        /// - Extensions that apply to interfaces implemented by the specified type
        /// - Extensions apply to each derived type, in order of derivation. 
        /// </remarks>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="extendedType"> The Type being extended with extension methods. </param>
        /// <param name="selector">     The filtering function used to inspect the methods when building the list. </param>
        /// <returns>   A <see cref="System.Collections.Generic.IEnumerable{MethodInfo}"/> with all the extension methods. </returns>
        public static IEnumerable<MethodInfo> GetExtensionMethodsForCurrentAssemblies(this Type extendedType, Func<MethodInfo, bool> selector)
        {
            if (null == extendedType) { throw new ArgumentNullException("extendedType"); }
            if (null == selector) { throw new ArgumentNullException("selector"); }

            return GetExtensionMethods(AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).AsParallel(), extendedType, selector);
        }

        /// <summary>   Gets the extension methods available in a given assembly that apply to a given type. </summary>
        /// <remarks>   
        /// IEnumerable is not cached.  Given Type is explored for all derived types and interfaces. Items are returned in alphabetical order
        /// after the following order:
        /// - Extensions that apply to the specified type
        /// - Extensions that apply to interfaces implemented by the specified type
        /// - Extensions apply to each derived type, in order of derivation. 
        /// </remarks>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="assembly">     The given assembly. </param>
        /// <param name="extendedType"> The Type being extended with extension methods. </param>
        /// <returns>   A <see cref="System.Collections.Generic.IEnumerable{MethodInfo}"/> with all the extension methods. </returns>
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Assembly assembly, Type extendedType)
        {
            return GetExtensionMethods(assembly, extendedType, m => true);
        }

        /// <summary>   Gets the extension methods available in a given assembly that apply to a given type. </summary>
        /// <remarks>   
        /// IEnumerable is not cached.  Given Type is explored for all derived types and interfaces. Items are returned in alphabetical order
        /// after the following order:
        /// - Extensions that apply to the specified type
        /// - Extensions that apply to interfaces implemented by the specified type
        /// - Extensions apply to each derived type, in order of derivation. 
        /// </remarks>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="assembly">     The given assembly. </param>
        /// <param name="extendedType"> The Type being extended with extension methods. </param>
        /// <param name="selector">     The filtering function used to inspect the methods when building the list. </param>
        /// <returns>   A <see cref="System.Collections.Generic.IEnumerable{MethodInfo}"/> with all the extension methods. </returns>
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Assembly assembly, Type extendedType, Func<MethodInfo, bool> selector)
        {
            if (null == assembly) { throw new ArgumentNullException("assembly"); }
            if (null == extendedType) { throw new ArgumentNullException("extendedType"); }
            if (null == selector) { throw new ArgumentNullException("selector"); }

            return GetExtensionMethods(assembly.GetTypes().AsParallel(), extendedType, selector);
        }

        private static IEnumerable<MethodInfo> GetExtensionMethods(ParallelQuery<Type> types, Type extendedType, Func<MethodInfo, bool> selector)
        {
            //TODO: this works in very general cases with generics -- but won't properly handle generics with types specified
            var baseTypes = extendedType.GetAllBaseTypesAndInterfaces();
            var query = from type in types
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)                                                
                        where method.IsDefined(typeof(ExtensionAttribute), false)                        
                        let rawParameterType = method.GetParameters()[0].ParameterType
                        //if we're not a generic parameter type, use the types
                        let parameterType = !rawParameterType.IsGenericType ? rawParameterType :
                        //if we're asking for something like IEnumerable<> then we can use GetGenericTypeDefinition
                            rawParameterType.ContainsGenericParameters ? rawParameterType.GetGenericTypeDefinition() :
                        //otherwise, just use the IEnumerable<int> or what have you
                            rawParameterType
                        //TODO : what needs to happen is we have to check for type compatibility here -- we have have been passed a IEnumerable<int>
                        //but our baseTypes includes IEnumerable<> -- these two are compatible -- we have to check rules of contravariance, etc
                        where baseTypes.Any(pair => pair.Key == parameterType)
                        where selector.Invoke(method)
                        //TODO: keep in mind if we're dealing with a compatbility situation as described above, this parameterType indexer will be wrong
                        orderby baseTypes[parameterType] ascending, method.Name ascending
                        select method;
            return query;
        }
    }
}
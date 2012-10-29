using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EqualityComparer.Reflection;

namespace EqualityComparer
{
  /// <summary>   A class that performs a public property by property and field by field comparison of two object instances.  Useful for testing. </summary>
  /// <remarks>   http://stackoverflow.com/questions/986572/hows-to-quick-check-if-data-transfer-two-objects-have-equal-properties-in-c </remarks>
  [SuppressMessage("Gendarme.Rules.Smells", "AvoidSpeculativeGeneralityRule", Justification = "This is a class useful to testing and does not represent speculation")]
  public static class MemberComparer
  {
    /// <summary>   Does a public property by property and field by field comparison of the two objects. </summary>
    /// <remarks>   ebrown, 1/19/2011. </remarks>
    /// <typeparam name="T">    Generic type parameter - inferred by compiler. </typeparam>
    /// <param name="instanceX">    The first instance. </param>
    /// <param name="instanceY">    The second instance. </param>
    /// <returns>   true if the objects are equivalent by comparison of properties OR both instances are NULL, false if not. </returns>
    public static bool Equal<T>(T instanceX, T instanceY)
    {
      return Equal(instanceX, instanceY, new IEqualityComparer[] { });
    }

    /// <summary> Does a public property by property and field by field comparison of the two objects. </summary>
    /// <remarks> ebrown, 1/19/2011. </remarks>
    /// <exception cref="ArgumentNullException">  Thrown when the list of comparers is null, the comparers are not also instances of IEqualityComparer{} or any of the comparers are null. </exception>
    /// <exception cref="ArgumentException">    Thrown when there is more than one comparer for a given type. </exception>
    /// <typeparam name="T">  Generic type parameter - inferred by compiler. </typeparam>
    /// <param name="instanceX">    The first instance. </param>
    /// <param name="instanceY">    The second instance. </param>
    /// <param name="customComparers">  A variable-length parameters list containing custom comparers. </param>
    /// <returns> true if the objects are equivalent by comparison of properties OR both instances are NULL, false if not. </returns>
    public static bool Equal<T>(T instanceX, T instanceY, IEnumerable<IEqualityComparer> customComparers)
    {
      if (null == instanceX && null == instanceY) { return true; }
      if (null == instanceX || null == instanceY) { return false; }
      if (null == customComparers)
      {
        throw new ArgumentNullException("customComparers");
      }

      if (customComparers.Any(comparer => null == comparer))
      {
        throw new ArgumentNullException("customComparers", "List of comparers contains a null IEqualityComparer");
      }

      Type genericEqualityComparer = typeof(IEqualityComparer<>);
      if (customComparers.Any(comparer => !genericEqualityComparer.IsGenericInterfaceAssignableFrom(comparer.GetType())))
      {
        throw new ArgumentException("All comparer instances must implement IEqualityComparer<>", "customComparers");
      }

      var comparerPairs = customComparers.Select(comparer =>
        new KeyValuePair<Type, IEqualityComparer>(genericEqualityComparer.GetGenericInterfaceTypeParameters(comparer.GetType()).First(), comparer));

      if (comparerPairs.Select(pair => pair.Key).Distinct().Count() != comparerPairs.Count())
      {
        throw new ArgumentException("Only one IEqualityComparer<> instance per Type is allowed in the list");
      }

      var customComparerDictionary = comparerPairs.ToDictionary(pair => pair.Key, pair => pair.Value);

      return Cache<T>.Compare(instanceX, instanceY, customComparerDictionary);
    }

    static bool ImplementsItsOwnEqualsMethod(this Type type)
    {
      var equalsMethod = type.GetMethod("Equals", new Type[] { type });
      return (null != equalsMethod && equalsMethod.DeclaringType == type);
    }

    static class Cache<T>
    {
      //instance, instance,
      internal static readonly Func<T, T, IDictionary<Type, IEqualityComparer>, bool> Compare = delegate { return true; };

      [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "More readable with static constructor / no easy way to write without")]
      static Cache()
      {
        Type t = typeof(T);

        if (typeof(string) == t)
        {
          Compare = (stringX, stringY, comparers) => comparers.ContainsKey(t) ?
            comparers[t].Equals(stringX, stringY) :
            StringComparer.Ordinal.Equals(stringX, stringY);
          return;
        }
        //for now, do a ref check, since Exception is a bit of an oddball type
        else if (typeof(Exception).IsAssignableFrom(t))
        {
          Compare = (exceptionX, exceptionY, comparers) => comparers.ContainsKey(t) ?
            comparers[t].Equals(exceptionX, exceptionY) :
            object.ReferenceEquals(exceptionX, exceptionY);
          return;
        }
        else if ((t.IsValueType || t.IsPrimitive) &&
          !(t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))))
        {
          Compare = (valueX, valueY, comparers) => comparers.ContainsKey(t) ?
            comparers[t].Equals(valueX, valueY) :
            valueX.Equals(valueY);
          return;
        }

        var x = Expression.Parameter(t, "x");
        var y = Expression.Parameter(t, "y");
        var customComparers = Expression.Parameter(typeof(IDictionary<Type, IEqualityComparer>), "customComparers");

        if (typeof(IEnumerable<>).IsGenericInterfaceAssignableFrom(t))
        {
          Type genericTypeParam = (typeof(IEnumerable<>)).GetGenericInterfaceTypeParameters(t).First();
          //Type genericType = typeof(IEnumerable<>).MakeGenericType(genericTypeParam);
          Compare = Expression.Lambda<Func<T, T, IDictionary<Type, IEqualityComparer>, bool>>(SequencesOfTypeAreEqual(x, y, genericTypeParam, customComparers), x, y, customComparers)
            .Compile();
          return;
        };

        var members = t.GetProperties(BindingFlags.Public | BindingFlags.Instance).OfType<MemberInfo>().Union(t.GetFields(BindingFlags.Public | BindingFlags.Instance)).ToArray();
        if (members.Length == 0) { return; }

        Expression body = CallExpressionIfNoComparer(BuildRecursiveComparison(members, x, y, customComparers, null, null),
          customComparers, t, x, y);
        Compare = Expression.Lambda<Func<T, T, IDictionary<Type, IEqualityComparer>, bool>>(body, x, y, customComparers)
          .Compile();
      }
      private static MethodCallExpression SequencesOfTypeAreEqual(Expression xProperty, Expression yProperty, Type genericTypeParam, Expression comparers)
      {
        return Expression.Call(typeof(System.Linq.Enumerable), "SequenceEqual", new Type[] { genericTypeParam },
          new Expression[] { xProperty, yProperty, Expression.Call(typeof(GenericEqualityComparer<>).MakeGenericType(genericTypeParam), "ByAllMembers", null, Expression.Property(comparers, "Values")) });
      }

      private static BinaryExpression CallComparerIfAvailable(Expression comparers, Type memberType, Expression xPropertyOrField, Expression yPropertyOrField)
      {
        //(comparers.ContainsKey(memberType) && ((IEqualityComparer<memberType>)comparers[memberType]).Equals(x, y))
        return BinaryExpression.AndAlso(
          Expression.Call(comparers, "ContainsKey", null, Expression.Constant(memberType, typeof(Type))),
          Expression.Call(Expression.TypeAs(
            Expression.MakeIndex(comparers, typeof(IDictionary<Type, IEqualityComparer>).GetProperty("Item", typeof(IEqualityComparer), new[] { typeof(Type) }), new[] { Expression.Constant(memberType, typeof(Type)) }),
            typeof(IEqualityComparer<>).MakeGenericType(memberType)), "Equals", null, xPropertyOrField, yPropertyOrField));
      }

      private static BinaryExpression CallExpressionIfNoComparer(Expression comparison, ParameterExpression comparers, Type memberType, Expression xPropertyOrField, Expression yPropertyOrField)
      {
        return BinaryExpression.Or(
          CallComparerIfAvailable(comparers, memberType, xPropertyOrField, yPropertyOrField),
            Expression.AndAlso(
              Expression.IsFalse(Expression.Call(comparers, "ContainsKey", null, Expression.Constant(memberType, typeof(Type)))),
              comparison));
      }

      private static BinaryExpression CustomPropertyComparison(MemberExpression xPropertyOrField, MemberExpression yPropertyOrField, BinaryExpression parentNullChecks, BinaryExpression recursiveProperties,
        Func<BinaryExpression, Expression> customCheckToThisLevel)
      {
        var nullExpression = Expression.Constant(null, typeof(object));
        //x.Property != null && y.Property != null
        var propertyNotNullCheck = Expression.AndAlso(Expression.NotEqual(xPropertyOrField, nullExpression), Expression.NotEqual(yPropertyOrField, nullExpression));
        //combine with any parent null checking to this depth -- i.e. (x.Property != null && y.Property != null && x.Property.Property != null && y.Property.Property != null)
        var nullChecktoThisDepth = null == parentNullChecks ? propertyNotNullCheck : Expression.AndAlso(parentNullChecks, propertyNotNullCheck);

        //now combine that with a null for OKs -- i.e. x.Property != null && y.Property != null
        var propertiesAreBothNull = Expression.AndAlso(Expression.Equal(xPropertyOrField, nullExpression), Expression.Equal(yPropertyOrField, nullExpression));
        var nullAllowedToThisDepth = null == parentNullChecks ? propertiesAreBothNull : Expression.AndAlso(parentNullChecks, propertiesAreBothNull);

        //prefixing with the null check to this point + our custom comparison on this property
        var completedExpression = Expression.Or(nullAllowedToThisDepth, Expression.AndAlso(nullChecktoThisDepth, customCheckToThisLevel(nullChecktoThisDepth)));

        //combine the running list of recursive properties for this parent
        return recursiveProperties == null ? completedExpression : Expression.AndAlso(recursiveProperties, completedExpression);
      }

      private static Expression BuildRecursiveComparison(MemberInfo[] members, Expression x, Expression y, ParameterExpression comparers, BinaryExpression parentNullChecks, Expression body)
      {
        //nothing to see here -- move on
        if (members.Length == 0)
          return body;

        BinaryExpression propertyEqualities = null;
        BinaryExpression recursiveProperties = null;

        for (int i = 0; i < members.Length; i++)
        {
          Type memberType = members[i] is PropertyInfo ? ((PropertyInfo)members[i]).PropertyType : ((FieldInfo)members[i]).FieldType;
          MemberExpression xPropertyOrField = Expression.PropertyOrField(x, members[i].Name),
            yPropertyOrField = Expression.PropertyOrField(y, members[i].Name);

          if (memberType.IsValueType || (memberType.ImplementsItsOwnEqualsMethod() && !memberType.IsAnonymous()))
          {
            var propEqual = CallExpressionIfNoComparer(Expression.Equal(xPropertyOrField, yPropertyOrField), comparers, memberType, xPropertyOrField, yPropertyOrField);

            // this type supports value comparison so we can just compare it.
            propertyEqualities = propertyEqualities == null ? propEqual : Expression.AndAlso(propertyEqualities, propEqual);
            //i.e. (x.Property == y.Property) && (x.Property2 == y.Property2) ....
          }
          //for now, do a ref check, since Exception is a bit of an oddball type
          else if (typeof(Exception).IsAssignableFrom(memberType))
          {
            //the refs are the same OR
            var propEqual = CallExpressionIfNoComparer(Expression.ReferenceEqual(xPropertyOrField, yPropertyOrField),
              comparers, memberType, xPropertyOrField, yPropertyOrField);

            propertyEqualities = propertyEqualities == null ? propEqual : Expression.AndAlso(propertyEqualities, propEqual);
            //i.e. (x.Property == y.Property) && (x.Property2 == y.Property2) ....
          }
          else if (typeof(IEnumerable<>).IsGenericInterfaceAssignableFrom(memberType))
          {
            Type genericTypeParam = (typeof(IEnumerable<>)).GetGenericInterfaceTypeParameters(memberType).First();

            recursiveProperties = CustomPropertyComparison(xPropertyOrField, yPropertyOrField, parentNullChecks, recursiveProperties, (nullCheckToThisDepth) =>
              //and call SequenceEqual on the two sequences, passing a custom IEqualityComparer
              SequencesOfTypeAreEqual(xPropertyOrField, yPropertyOrField, genericTypeParam, comparers));
          }
          // this type does not support value comparison, so we must recurse and find it's properties.
          else
          {
            recursiveProperties = CustomPropertyComparison(xPropertyOrField, yPropertyOrField, parentNullChecks, recursiveProperties, (nullCheckToThisDepth) =>
              //and either use a passed comparer or recurse the property and all it's nested properties
              CallExpressionIfNoComparer(
                BuildRecursiveComparison(memberType.GetProperties(BindingFlags.Public | BindingFlags.Instance).OfType<MemberInfo>().Union(memberType.GetFields(BindingFlags.Public | BindingFlags.Instance)).ToArray(),
                  xPropertyOrField, yPropertyOrField, comparers, nullCheckToThisDepth, recursiveProperties),
                comparers, memberType, xPropertyOrField, yPropertyOrField));
          }
        }

        //to make it this far we either had simple properties or nested recursive stuff
        BinaryExpression depthCheck = null != propertyEqualities && null != recursiveProperties ? Expression.AndAlso(propertyEqualities, recursiveProperties)
          : propertyEqualities ?? recursiveProperties;

        //combine that with our main running expression if there is one
        return body == null ? depthCheck : Expression.AndAlso(body, depthCheck);
      }
    }
  }
}

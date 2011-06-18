using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EPS.Reflection;
using EPS.Utility;

namespace EPS
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
			if (null == instanceX && null == instanceY) { return true; }
			if (null == instanceX || null == instanceY) { return false; }

			return Cache<T>.Compare(instanceX, instanceY, DateComparisonType.Exact);
		}

		/// <summary>   Does a public property by property and field by field comparison of the two objects. </summary>
		/// <remarks>   ebrown, 1/19/2011. </remarks>
		/// <typeparam name="T">    Generic type parameter - inferred by compiler. </typeparam>
		/// <param name="instanceX">    The first instance. </param>
		/// <param name="instanceY">    The second instance. </param>
		/// <returns>   true if the objects are equivalent by comparison of properties OR both instances are NULL, false if not. </returns>
		public static bool Equal<T>(T instanceX, T instanceY, DateComparisonType dateComparisonType)
		{
			if (null == instanceX && null == instanceY) { return true; }
			if (null == instanceX || null == instanceY) { return false; }

			return Cache<T>.Compare(instanceX, instanceY, dateComparisonType);
		}

		static bool ImplementsItsOwnEqualsMethod(this Type type)
		{
			var equalsMethod = type.GetMethod("Equals", new Type[] { type });
			return (equalsMethod.DeclaringType == type);
		}		

		static class Cache<T>
		{
			//instance, instance, 
			internal static readonly Func<T, T, DateComparisonType, bool> Compare = delegate { return true; };

			[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "More readable with static constructor / no easy way to write without")]
			static Cache()
			{
				Type t = typeof(T);
				if (typeof(string) == t)
				{
					Compare = (stringX, stringY, dateComparisonType) => StringComparer.Ordinal.Equals(stringX, stringY);
					return;
				}
				else if (typeof(DateTime) == t || typeof(DateTime?) == t)
				{
					Compare = (dateX, dateY, dateComparisonType) => dateComparisonType == DateComparisonType.Exact ? dateX.Equals(dateY) : 
						new DateComparer(dateComparisonType).Equals(Convert.ToDateTime(dateX), Convert.ToDateTime(dateY));
						return;
				}
				else if (t.IsPrimitive)
				{
					Compare = (primitiveX, primitiveY, dateComparisonType) => primitiveX.Equals(primitiveY);
					return;
				}

				var x = Expression.Parameter(t, "x");
				var y = Expression.Parameter(t, "y");
				var dateComparisonTypeParameter = Expression.Parameter(typeof(DateComparisonType), "dateComparisonType");

				if (typeof(IEnumerable<>).IsGenericInterfaceAssignableFrom(t))
				{
					Type genericTypeParam = (typeof(IEnumerable<>)).GetGenericInterfaceTypeParameters(t).First();
					//Type genericType = typeof(IEnumerable<>).MakeGenericType(genericTypeParam);
					Compare = Expression.Lambda<Func<T, T, DateComparisonType, bool>>(SequencesOfTypeAreEqual(x, y, genericTypeParam, dateComparisonTypeParameter), x, y, dateComparisonTypeParameter)
						.Compile();
					return;
				};

				var members = t.GetProperties().OfType<MemberInfo>().Union(t.GetFields()).ToArray();
				if (members.Length == 0) { return; }

				Expression body = BuildRecursiveComparison(members, x, y, dateComparisonTypeParameter, null, null);
				Compare = Expression.Lambda<Func<T, T, DateComparisonType, bool>>(body, x, y, dateComparisonTypeParameter)
					.Compile();
			}
			private static MethodCallExpression SequencesOfTypeAreEqual(Expression xProperty, Expression yProperty, Type genericTypeParam, Expression dateComparisonProperty)
			{
				return Expression.Call(typeof(System.Linq.Enumerable), "SequenceEqual", new Type[] { genericTypeParam },
					new Expression[] { xProperty, yProperty, Expression.Call(typeof(GenericEqualityComparer<>).MakeGenericType(genericTypeParam), "ByAllMembers", null, dateComparisonProperty) });
			}

			private static Expression BuildRecursiveComparison(MemberInfo[] members, Expression x, Expression y, Expression dateComparisonType, Expression parentNullChecks, Expression body)
			{
				//nothing to see here -- move on
				if (members.Length == 0)
					return body;

				Expression propertyEqualities = null,
					recursiveProperties = null;

				for (int i = 0; i < members.Length; i++)
				{
					Type memberType = members[i] is PropertyInfo ? ((PropertyInfo)members[i]).PropertyType : ((FieldInfo)members[i]).FieldType;
					Expression xPropertyOrField = Expression.PropertyOrField(x, members[i].Name),
						yPropertyOrField = Expression.PropertyOrField(y, members[i].Name);

					if (typeof(DateTime) == memberType || typeof(DateTime?) == memberType)
					{
						Expression castToDateTimeX = Expression.TypeAs(xPropertyOrField, typeof(DateTime?));
						Expression castToDateTimeY = Expression.TypeAs(yPropertyOrField, typeof(DateTime?));

						Expression toSecond = Expression.AndAlso(Expression.Equal(Expression.Constant(DateComparisonType.ToSecond), dateComparisonType),
							Expression.Call(Expression.Assign(Expression.Variable(typeof(DateComparer), "comparer"), 
								Expression.New(typeof(DateComparer))), 
								"Equals", null, castToDateTimeX, castToDateTimeY));
						
						Expression exact = Expression.Equal(xPropertyOrField, yPropertyOrField);

						Expression datesEqual = Expression.Or(toSecond, exact);

						propertyEqualities = propertyEqualities == null ? datesEqual : Expression.AndAlso(propertyEqualities, datesEqual);
					}
					if (memberType.IsValueType || (memberType.ImplementsItsOwnEqualsMethod() && !memberType.IsAnonymous()))
					{
						// this type supports value comparison so we can just compare it.       
						var propEqual = Expression.Equal(xPropertyOrField, yPropertyOrField); //x.Property == y.Property
						propertyEqualities = propertyEqualities == null ? propEqual : Expression.AndAlso(propertyEqualities, propEqual);
						//i.e. (x.Property == y.Property) && (x.Property2 == y.Property2) .... 
					}
					else if (typeof(IEnumerable<>).IsGenericInterfaceAssignableFrom(memberType))
					{
						Type genericTypeParam = (typeof(IEnumerable<>)).GetGenericInterfaceTypeParameters(memberType).First();

						var nullExpression = Expression.Constant(null);
						//x.Property != null && y.Property != null
						var propertyNotNullCheck = Expression.AndAlso(Expression.NotEqual(xPropertyOrField, nullExpression), Expression.NotEqual(yPropertyOrField, nullExpression));
						//combine with any parent null checking to this depth -- i.e. (x.Property != null && y.Property != null && x.Property.Property != null && y.Property.Property != null)
						var nullChecktoThisDepth = null == parentNullChecks ? propertyNotNullCheck : Expression.AndAlso(parentNullChecks, propertyNotNullCheck);
						var recursiveProperty =
							//prefixing with the null check to this point
							Expression.AndAlso(nullChecktoThisDepth,
							//and call SequenceEqual on the two sequences, passing a custom IEqualityComparer
							SequencesOfTypeAreEqual(xPropertyOrField, yPropertyOrField, genericTypeParam, dateComparisonType));

						//now combine that with a null for OKs -- i.e. x.Property != null && y.Property != null
						var propertiesAreBothNull = Expression.AndAlso(Expression.Equal(xPropertyOrField, nullExpression), Expression.Equal(yPropertyOrField, nullExpression));
						var nullAllowedToThisDepth = null == parentNullChecks ? propertiesAreBothNull : Expression.AndAlso(parentNullChecks, propertiesAreBothNull);

						var completedExpression = Expression.Or(nullAllowedToThisDepth, recursiveProperty);

						//combine the running list of recursive properties for this parent
						recursiveProperties = recursiveProperties == null ? completedExpression : Expression.AndAlso(recursiveProperties, completedExpression);
					}
					// this type does not support value comparison, so we must recurse and find it's properties.
					else
					{
						var nullExpression = Expression.Constant(null);
						//x.Property != null && y.Property != null
						var propertyNotNullCheck = Expression.AndAlso(Expression.NotEqual(xPropertyOrField, nullExpression), Expression.NotEqual(yPropertyOrField, nullExpression));
						//combine with any parent null checking to this depth -- i.e. (x.Property != null && y.Property != null && x.Property.Property != null && y.Property.Property != null)
						var nullChecktoThisDepth = null == parentNullChecks ? propertyNotNullCheck : Expression.AndAlso(parentNullChecks, propertyNotNullCheck);
						var recursiveProperty =
							//prefixing with the null check to this point
							Expression.AndAlso(nullChecktoThisDepth,
							//and recurse the property and all it's nested properties
							BuildRecursiveComparison(memberType.GetProperties(), xPropertyOrField, yPropertyOrField, dateComparisonType, nullChecktoThisDepth, recursiveProperties));

						//now combine that with a null for OKs -- i.e. x.Property != null && y.Property != null
						var propertiesAreBothNull = Expression.AndAlso(Expression.Equal(xPropertyOrField, nullExpression), Expression.Equal(yPropertyOrField, nullExpression));
						var nullAllowedToThisDepth = null == parentNullChecks ? propertiesAreBothNull : Expression.AndAlso(parentNullChecks, propertiesAreBothNull);

						var completedExpression = Expression.Or(nullAllowedToThisDepth, recursiveProperty);

						//combine the running list of recursive properties for this parent
						recursiveProperties = recursiveProperties == null ? completedExpression : Expression.AndAlso(recursiveProperties, completedExpression);
					}
				}

				//to make it this far we either had simple properties or nested recursive stuff
				Expression depthCheck = null != propertyEqualities && null != recursiveProperties ? Expression.AndAlso(propertyEqualities, recursiveProperties)
					: propertyEqualities ?? recursiveProperties;

				//combine that with our main running expression if there is one
				return body == null ? depthCheck : Expression.AndAlso(body, depthCheck);
			}
		}
	}
}
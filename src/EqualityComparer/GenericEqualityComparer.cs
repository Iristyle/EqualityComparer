using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EqualityComparer
{
  /// <summary>   A generic comparer that takes accepts a Func{T, T, bool} to create simple on-the-fly comparison routines. </summary>
  /// <remarks>   ebrown, 2/7/2011. </remarks>
  public class GenericEqualityComparer<T> : EqualityComparer<T>
  {
    private readonly Func<T, T, bool> _comparer;
    private readonly Func<T, int> _hasher;

    /// <summary> Constructor accepting the comparison function. </summary>
    /// <remarks>
    /// Uses a hasher function that always returns the default GetHashCode implementation for given instances.  Don't use this for any
    /// sorting operations.
    /// </remarks>
    /// <param name="comparer"> The comparison function to use when comparing the two instances. </param>
    ///
    /// <exception cref="ArgumentNullException">  Thrown when the comparer or hashers are null. </exception>
    public GenericEqualityComparer(Func<T, T, bool> comparer) :
      this(comparer, o => o.GetHashCode())
    { }

    /// <summary>   Constructor accepting the comparison function and hashing function. </summary>
    /// <remarks>   ebrown, 2/7/2011. </remarks>
    /// <exception cref="ArgumentNullException">    Thrown when the comparer or hashers are null. </exception>
    /// <param name="comparer">   The comparison function to use when comparing the two instances. </param>
    /// <param name="hasher">       The hash function used to generate object hashes on the instances. </param>
    public GenericEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hasher)
    {
      if (comparer == null)
        throw new ArgumentNullException("comparer");
      if (hasher == null)
        throw new ArgumentNullException("hasher");

      this._comparer = comparer;
      this._hasher = hasher;
    }

    /// <summary>   Tests if two T objects are considered equal. </summary>
    /// <remarks>   Uses the passed in Func{T, T, bool} for the comparison. </remarks>
    /// <param name="x">    T to be compared. </param>
    /// <param name="y">    T to be compared. </param>
    /// <returns>   true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(T x, T y)
    {
      return this._comparer(x, y);
    }

    /// <summary> Calculates the hash code for this object. </summary>
    /// <remarks>
    /// If no hasher function was supplied, will always return the default GetHashCode implementation.  Otherwise uses Func{T, int} hasher
    /// function supplied.
    /// </remarks>
    /// <param name="obj">  The object. </param>
    /// <returns> The hash code for this object. </returns>
    public override int GetHashCode(T obj)
    {
      return this._hasher(obj);
    }

    /// <summary>
    /// Shortcut method to get a simple generic IEqualityComparer{T} where the comparison is by all properties and fields on the instance.
    /// </summary>
    /// <remarks> ebrown, 6/6/2011. </remarks>
    /// <returns> A GenericEqualityComparer{T}. </returns>
    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "A static factory method is perfectly acceptable in this context")]
    public static GenericEqualityComparer<T> ByAllMembers()
    {
      return new GenericEqualityComparer<T>((x, y) => MemberComparer.Equal(x, y));
    }

    /// <summary>
    /// Shortcut method to get a simple generic IEqualityComparer{T} where the comparison is by all properties and fields on the instance,
    /// with user defined overrides available on specific encountered types.
    /// </summary>
    /// <remarks> ebrown, 6/6/2011. </remarks>
    /// <param name="customComparers">  A set of additional comparers to use to override default member by member comparison. </param>
    /// <returns> A GenericEqualityComparer{T}. </returns>
    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "A static factory method is perfectly acceptable in this context")]
    public static GenericEqualityComparer<T> ByAllMembers(IEnumerable<IEqualityComparer> customComparers)
    {
      return new GenericEqualityComparer<T>((x, y) => MemberComparer.Equal(x, y, customComparers));
    }
  }
}

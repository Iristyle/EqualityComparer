![Logo](https://github.com/EastPoint/EqualityComparer/raw/master/logo-128.png)

# EqualityComparer
A super basic way of comparing object instances on a property by property / member by member basis.  There are allowances for overriding default comparison behaviors, and it's mostly implemented using Expressions.

The code was heavily inspired / derived from Marc Gravells original [post on StackOverflow](http://stackoverflow.com/questions/986572/hows-to-quick-check-if-data-transfer-two-objects-have-equal-properties-in-c) discussing a basic comparison method for DTOs.  Like Marcs original post, this code also uses some caching of built Expression trees once an object type has been 'seen'.

This variant is a little more general purpose with a few more features, and is designed primarily around testing semantics.

## Installation

* Install-Package EqualityComparer

### Requirements

* .NET Framework 4+ Client Profile

### Usage Examples

There are plenty of examples in the Tests project.  Look there for more details.

Should work properly with:

* Nested types
* Anonymous types
* Custom hand-rolled comparers (derived from IEqualityComparer or using the included Func<T,T> based GenericEqualityComparer)
* Fuzzy date comparison (i.e. where Redis stores an inexact date value vs what .NET has)

### Basic Comparison
```csharp
  Guid sharedGuid = Guid.NewGuid();
  DateTime now = DateTime.Now;
  Assert.True(MemberComparer.Equal(new { PropertyA = "A", Integer = 23, Guid = sharedGuid, Date = now },
    new { PropertyA = "A", Integer = 23, Guid = sharedGuid, Date = now }));
```

### Fuzzy date comparisons.

```csharp
  DateTime one = DateTime.Parse("07:27:15.01"),
  two = DateTime.Parse("07:27:15.49");

  var a = new { Foo = 5, Bar = new { Now = one } };
  var b = new { Foo = 5, Bar = new { Now = two } };

  Assert.True(MemberComparer.Equal(one, two, new[] { new DateComparer(DateComparisonType.TruncatedToSecond) }));
```

### Custom Comparer

```csharp
  class ClassWithFieldsAndProperties
  {
    public string Foo;
    public string Bar { get; set; }
  }

  string Bar = "bar";
  Assert.True(MemberComparer.Equal(new { Integer = 5, Custom = new ClassWithFieldsAndProperties() { Foo = "456", Bar = Bar } },
    new { Integer = 5, Custom = new ClassWithFieldsAndProperties() { Foo = "4567", Bar = Bar } },
    new[] { new GenericEqualityComparer<ClassWithFieldsAndProperties>((a, b) => a.Bar == b.Bar) }));
```

### GenericEqualityComparer

In addition to being able to create a new instance of a GenericEqualityComparer<T> with an anonymous Func<T,T,bool>, the GenericEqualityComparer class has a ```ByAllMembers``` static
which will recursively examine the type and generate a (cached) comparison Expression.

Given this class:

```csharp
  class A
  {
    public A(int integer, string @string)
    {
      Integer = integer;
      String = @string;
    }

    public int Integer { get; set; }
    public string String { get; set; }

    public static GenericEqualityComparer<A> IntegerOnlyComparer = new GenericEqualityComparer<A>((a1, a2) => a1.Integer == a2.Integer);
  }
```

We can use the standard ByAllMembers comparison.

```csharp
  A a = new A(3, "Foo"),
  b = new A(3, "Foo");

  Assert.Equal(a, b, GenericEqualityComparer<A>.ByAllMembers());
```

ByAllMembers also has an overload that lets you specify comparers to use when a specific type is encountered.  In this example, we tell it to only look at the Integer member of A when A instances are found in the object graph.

```csharp
  class B
  {
    public B(int integer, A a)
    {
      Integer = integer;
      A = @a;
    }

    public int Integer { get; set; }
    public A A { get; set; }
  }

  B b = new B(6, new A(5, "Foo")),
  b2 = new B(6, new A(5, "Bar"));

  Assert.Equal(b, b2, GenericEqualityComparer<B>.ByAllMembers(new[] { A.IntegerOnlyComparer }));
```

## Similar Projects

* [AutoFixture](http://autofixture.codeplex.com/) includes a library called Ploeh.SemanticComparison .  I haven't checked out all the details, but it does pack a Fluent interface.
* [AnonymousComparer](http://linqcomparer.codeplex.com/) - the AnonymousComparer looks very similar to the GenericEqualityComparer class in our library, except for some syntactical differences.  It doesn't look like there are options to override the behavior of comparisons either.
* [System.DataStructures.FuncComparer](http://adjunct.codeplex.com/) - looks like a basic implementation of a Func<T,T,bool> IEqualityComparer.

## Future Improvements

* Ensure Mono works properly (it *should* already)

## Contributing

Fork the code, and submit a pull request!

Any useful changes are welcomed.  If you have an idea you'd like to see implemented that strays far from the simple spirit of the application, ping us first so that we're on the same page.

## Credits

Creative Commons icon courtesy [WikiMedia](http://commons.wikimedia.org/wiki/File:Emblem-equal.svg)

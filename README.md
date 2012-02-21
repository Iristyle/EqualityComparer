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

###Custom Comparer

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

## Future Improvements

* Ensure Mono works properly (it *should* already)

## Contributing

Fork the code, and submit a pull request!  

Any useful changes are welcomed.  If you have an idea you'd like to see implemented that strays far from the simple spirit of the application, ping us first so that we're on the same page.
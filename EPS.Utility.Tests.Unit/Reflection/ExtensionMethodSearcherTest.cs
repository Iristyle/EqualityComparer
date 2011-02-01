using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace EPS.Reflection.Tests.Unit
{
    public class FakeType
    {
        public int Id { get; set; }
    }
    public static class FakeTypeExtensions
    {
        public static bool IsIdEven(this FakeType fakeType)
        {
            return fakeType.Id % 2 == 0;
        }

        public static bool IEnumerableTest<T>(this IEnumerable<T> enumerable)
        {
            return true;
        }

        public static bool IEnumerableTest2(this IEnumerable<int> enumerable)
        {
            return true;
        }
    }

    public class ExtensionMethodSearcherTest
    {
        [Fact]
        public void GetExtensionMethods_FindsInSingleAssemblyWithGeneric()
        {
            var extensionMethods = ExtensionMethodSearcher.GetExtensionMethodsForCurrentAssemblies<FakeType>().ToList();
            Assert.True(extensionMethods.Count > 1);
        }

        [Fact]
        public void GetExtensionMethods_FindsInSingleAssembly()
        {            
            var extensionMethods = Assembly.GetExecutingAssembly().GetExtensionMethods(typeof(FakeType)).ToList();
            Assert.Equal(1, extensionMethods.Count);
        }

        [Fact]
        public void GetExtensionMethods_FindsInSingleAssemblyWithFiltering()
        {
            var extensionMethods = Assembly.GetExecutingAssembly()
                .GetExtensionMethods(typeof(FakeType), m => m.ReturnType == typeof(bool)).ToList();
            Assert.Equal(1, extensionMethods.Count);
        }

        [Fact]
        public void GetExtensionMethodsForCurrentAssemblies_FindsInAllAssemblies()
        {
            //we have to filter for extensions on only FakeType, since the default examines the type hierarchy -- and we have some generic extensions
            var extensionMethods = typeof(FakeType)
                .GetExtensionMethodsForCurrentAssemblies(m => m.GetParameters()[0].ParameterType == typeof(FakeType)).ToList();
            Assert.Equal(1, extensionMethods.Count);
        }

        [Fact]
        public void GetExtensionMethodsForCurrentAssemblies_FindsExtensionsOnGenericTypes()
        {
            //this should find at least the two extension methods defined above
            var localExtensions = Assembly.GetExecutingAssembly().GetExtensionMethods(typeof(IEnumerable<>));
            //TODO: currently only finds one, as the other is IEnumerable<int> and GetExtensionMethods has a bug
            Assert.True(localExtensions.Count() >= 2);
        }

        [Fact]
        public void GetExtensionMethodsForCurrentAssemblies_FindsExtensionsOnGenericTypesWithArgumentsSpecified()
        {
            //this should find at least the single extension method defined above
            var localExtensions = Assembly.GetExecutingAssembly().GetExtensionMethods(typeof(IEnumerable<int>));
            Assert.NotEmpty(localExtensions);
        }

        [Fact]
        public void GetExtensionMethodsForCurrentAssemblies_FindsInAllAssembliesOnBclInterface()
        {
            var typeToFind = typeof(IEnumerable<>);
            //we have to filter for extensions on only FakeType, since the default examines the type hierarchy -- and we have some generic extensions
            var bclExtensions = Assembly.GetAssembly(typeof(System.Linq.Enumerable)).GetExtensionMethods(typeToFind).Count();
            var localExtensions = Assembly.GetExecutingAssembly().GetExtensionMethods(typeToFind).Count();
            var allExtensions =  typeToFind.GetExtensionMethodsForCurrentAssemblies().Count();
            //throw new NotImplementedException("It would appear that there's a bug digging out generic interfaces like IEnumerable<>");
            Assert.True(bclExtensions > 0 && localExtensions > 0 && (bclExtensions + localExtensions <= allExtensions));
        }

        [Fact]
        public void GetExtensionMethodsForCurrentAssemblies_FindsInAllAssembliesWithFilter()
        {
            var extensionMethods = typeof(FakeType).GetExtensionMethodsForCurrentAssemblies(m => m.ReturnType == typeof(bool)).ToList();
            //we have at least our test extension method above -- who knows how many others return booleans ;0
            Assert.True(extensionMethods.Count >= 1);
        }

        [Fact]
        public void GetExtensionMethods_ThrowsOnNullAssembly()
        {
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethods(null, typeof(FakeType)));
        }

        [Fact]
        public void GetExtensionMethods_ThrowsOnNullAssemblyNullType()
        {
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethods(null, null));
        }

        [Fact]
        public void GetExtensionMethods_ThrowsOnNullType()
        {
            Assert.Throws<ArgumentNullException>(() => Assembly.GetExecutingAssembly().GetExtensionMethods(null));
        }

        [Fact]
        public void GetExtensionMethods_ThrowsOnNonNullTypeNullFilter()
        {
            Assert.Throws<ArgumentNullException>(() => Assembly.GetExecutingAssembly().GetExtensionMethods(typeof(FakeType), null));
        }

        [Fact]
        public void GetExtensionMethods_ThrowsOnNullTypeNonNullFilter()
        {
            Assert.Throws<ArgumentNullException>(() => Assembly.GetExecutingAssembly().GetExtensionMethods(null, m => true ));
        }
        
        [Fact]
        public void GetExtensionMethods_ThrowsOnNullTypeNullFilter()
        {
            Assert.Throws<ArgumentNullException>(() => Assembly.GetExecutingAssembly().GetExtensionMethods(null, null));
        }

        [Fact]
        public void GetExtensionMethodsForCurrentAssemblies_ThrowsOnNullFilter()
        {
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethodsForCurrentAssemblies(null));
        }

        [Fact]
        public void GetExtensionMethodsForCurrentAssemblies_ThrowsOnNonNullTypeNullFilter()
        {
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethodsForCurrentAssemblies(typeof(FakeType), null));
        }

        [Fact]
        public void GetExtensionMethodsForCurrentAssemblies_ThrowsOnNullTypeNonNullFilter()
        {
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethodsForCurrentAssemblies(null, m => true));
        }

        [Fact]
        public void GetExtensionMethodsForCurrentAssemblies_ThrowsOnNullTypeNullFilter()
        {
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethodsForCurrentAssemblies(null, null));
        }        
    }
}
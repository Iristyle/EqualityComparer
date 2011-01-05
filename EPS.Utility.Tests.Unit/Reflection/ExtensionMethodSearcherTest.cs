using System;
using System.Linq;
using System.Reflection;
using EPS.Reflection;
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
    }

    public class ExtensionMethodSearcherTest
    {        
        [Fact]
        public void CanFindExtensionMethodsBySingleAssembly()
        {            
            var extensionMethods = Assembly.GetExecutingAssembly().GetExtensionMethods(typeof(FakeType)).ToList();
            Assert.Equal(1, extensionMethods.Count);
        }
        [Fact]
        public void CanFindExtensionMethodsBySingleAssemblyWithFiltering()
        {
            var extensionMethods = Assembly.GetExecutingAssembly()
                .GetExtensionMethods(typeof(FakeType), m => m.ReturnType == typeof(bool)).ToList();
            Assert.Equal(1, extensionMethods.Count);
        }

        [Fact]
        public void CanFindExtensionMethodsInAllAssemblies()
        {
            var extensionMethods = typeof(FakeType).GetExtensionMethodsForCurrentAssemblies().ToList();
            Assert.Equal(1, extensionMethods.Count);
        }

        [Fact]
        public void CanFindExtensionMethodsInAllAssembliesWithFilter()
        {
            var extensionMethods = typeof(FakeType).GetExtensionMethodsForCurrentAssemblies(m => m.ReturnType == typeof(bool)).ToList();
            //we have at least our test extension method above -- who knows how many others return booleans ;0
            Assert.True(extensionMethods.Count >= 1);
        }

        [Fact]
        public void ThrowsOnNullParameters()
        {
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethods(null, typeof(FakeType)));
            Assert.Throws<ArgumentNullException>(() => Assembly.GetExecutingAssembly().GetExtensionMethods(null));
            Assert.Throws<ArgumentNullException>(() => Assembly.GetExecutingAssembly().GetExtensionMethods(typeof(FakeType), null));
            Assert.Throws<ArgumentNullException>(() => Assembly.GetExecutingAssembly().GetExtensionMethods(null, m => true ));
            Assert.Throws<ArgumentNullException>(() => Assembly.GetExecutingAssembly().GetExtensionMethods(null, null));

            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethodsForCurrentAssemblies(null));
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethodsForCurrentAssemblies(typeof(FakeType), null));
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethodsForCurrentAssemblies(null, m => true));
            Assert.Throws<ArgumentNullException>(() => ExtensionMethodSearcher.GetExtensionMethodsForCurrentAssemblies(null, null));
        }


    }
}
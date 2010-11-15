using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EPS.Reflection;
using Xunit;

namespace EPS.Reflection.Tests.Unit
{
    public class TypeExtensionsTest
    {

        [Fact]
        public void FindsGenericInterfaceDefinitionsProperly()
        {
            List<int> ints = new List<int>();
            Assert.True(typeof(IList<>).IsGenericInterfaceAssignableFrom(ints.GetType()));
        }

        [Fact]
        public void FindsGenericInterfaceDefinitionTypesProperly()
        {
            List<int> ints = new List<int>();
            var genericTypeParams = typeof(IList<>).GetGenericInterfaceTypeParameters(ints.GetType()).ToList();
            Assert.Equal(1, genericTypeParams.Count);
            Assert.Equal(typeof(int), genericTypeParams[0]);
        }

        [Fact]
        public void ReturnsFalseOnIsAssignableForUnimplementedInterface()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            Assert.Equal(false, typeof(IList<>).IsGenericInterfaceAssignableFrom(dictionary.GetType()));
        }

        [Fact]
        public void ThrowsOnUnimplementedInterface()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            Assert.Throws<ArgumentException>(() => typeof(IList<>).GetGenericInterfaceTypeParameters(dictionary.GetType()));
        }

        [Fact]
        public void ThrowsOnNonGenericInterface()
        {
            Assert.Throws<ArgumentException>(() => typeof(ICollection).IsGenericInterfaceAssignableFrom(typeof(int)));

            Assert.Throws<ArgumentException>(() => typeof(ICollection).GetGenericInterfaceTypeParameters(typeof(int)));
        }


        [Fact]
        public void ThrowsOnNullParameterCombinationsToIsGenericInterfaceAssignableFrom()
        {
            Assert.Throws<ArgumentNullException>(() => (null as Type).IsGenericInterfaceAssignableFrom(typeof(int)));
            Assert.Throws<ArgumentNullException>(() => typeof(int).IsGenericInterfaceAssignableFrom(null));
            Assert.Throws<ArgumentNullException>(() => (null as Type).IsGenericInterfaceAssignableFrom(null));
        }

        [Fact]
        public void ThrowsOnNullParameterCombinationsToGetGenericInterfaceTypeParameters()
        {
            Assert.Throws<ArgumentNullException>(() => (null as Type).GetGenericInterfaceTypeParameters(typeof(int)));
            Assert.Throws<ArgumentNullException>(() => typeof(int).GetGenericInterfaceTypeParameters(null));
            Assert.Throws<ArgumentNullException>(() => (null as Type).GetGenericInterfaceTypeParameters(null));
        }

    }
}

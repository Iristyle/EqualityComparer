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
        public void IsGenericInterfaceAssignableFrom_FindsGenericInterfaceDefinitionsProperly()
        {
            List<int> ints = new List<int>();
            Assert.True(typeof(IList<>).IsGenericInterfaceAssignableFrom(ints.GetType()));
        }

        [Fact]
        public void GetGenericInterfaceTypeParameters_FindsGenericInterfaceDefinitionTypesProperly()
        {
            List<int> ints = new List<int>();
            var genericTypeParams = typeof(IList<>).GetGenericInterfaceTypeParameters(ints.GetType()).ToList();
            Assert.Equal(1, genericTypeParams.Count);
            Assert.Equal(typeof(int), genericTypeParams[0]);
        }

        [Fact]
        public void IsGenericInterfaceAssignableFrom_ReturnsFalseOnIsAssignableForUnimplementedInterface()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            Assert.Equal(false, typeof(IList<>).IsGenericInterfaceAssignableFrom(dictionary.GetType()));
        }

        [Fact]
        public void GetGenericInterfaceTypeParameters_ThrowsOnUnimplementedInterface()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            Assert.Throws<ArgumentException>(() => typeof(IList<>).GetGenericInterfaceTypeParameters(dictionary.GetType()));
        }

        [Fact]
        public void IsGenericInterfaceAssignableFrom_ThrowsOnNonGenericInterface()
        {
            Assert.Throws<ArgumentException>(() => typeof(ICollection).IsGenericInterfaceAssignableFrom(typeof(int)));
        }

        [Fact]
        public void GetGenericInterfaceTypeParameters_ThrowsOnNonGenericInterface()
        {
            Assert.Throws<ArgumentException>(() => typeof(ICollection).GetGenericInterfaceTypeParameters(typeof(int)));
        }


        [Fact]
        public void IsGenericInterfaceAssignableFrom_ThrowsOnNullParameterCombinations()
        {
            Assert.Throws<ArgumentNullException>(() => (null as Type).IsGenericInterfaceAssignableFrom(typeof(int)));
            Assert.Throws<ArgumentNullException>(() => typeof(int).IsGenericInterfaceAssignableFrom(null));
            Assert.Throws<ArgumentNullException>(() => (null as Type).IsGenericInterfaceAssignableFrom(null));
        }

        [Fact]
        public void GetGenericInterfaceTypeParameters_ThrowsOnNullParameterCombinations()
        {
            Assert.Throws<ArgumentNullException>(() => (null as Type).GetGenericInterfaceTypeParameters(typeof(int)));
            Assert.Throws<ArgumentNullException>(() => typeof(int).GetGenericInterfaceTypeParameters(null));
            Assert.Throws<ArgumentNullException>(() => (null as Type).GetGenericInterfaceTypeParameters(null));
        }

        [Fact]
        public void GetAllBaseTypesAndInterfaces_FindsGenericDerivedTypesAndInterfaces()
        {
            //not the simplest example -- but should find these guys
            var interfaces = typeof(Dictionary<int,int>).GetAllBaseTypesAndInterfaces();
            
            Assert.Equal(10, interfaces.Count);
            Assert.True(interfaces[typeof(Dictionary<int, int>)] == 0);
            Assert.True(interfaces[typeof(IDictionary<int,int>)] == 1);
            Assert.True(interfaces[typeof(ICollection<KeyValuePair<int,int>>)] == 1);
            Assert.True(interfaces[typeof(IEnumerable<KeyValuePair<int, int>>)] == 1);
            Assert.True(interfaces[typeof(IEnumerable)] == 1);
            Assert.True(interfaces[typeof(IDictionary)] == 1);
            Assert.True(interfaces[typeof(ICollection)] == 1);
            Assert.True(interfaces[typeof(System.Runtime.Serialization.ISerializable)] == 1);
            Assert.True(interfaces[typeof(System.Runtime.Serialization.IDeserializationCallback)] == 1);
            Assert.True(interfaces[typeof(object)] == 2);
        }

        [Fact]
        public void GetAllBaseTypesAndInterfaces_FindsDerivedInterfaces()
        {
            var interfaces = typeof(IEnumerable<int>).GetAllBaseTypesAndInterfaces();
            //should find these guys
            Assert.Equal(2, interfaces.Count);
            Assert.True(interfaces[typeof(IEnumerable<int>)] == 0);
            Assert.True(interfaces[typeof(IEnumerable)] == 1);
        }

        [Fact]
        public void GetAllBaseTypesAndInterfaces_ThrowsOnNullType()
        {
            Assert.Throws<ArgumentNullException>(() => (null as Type).GetAllBaseTypesAndInterfaces());
        }

        interface IMarker { }
        interface IMarker2 : IMarker { }
        class A : IMarker { }
        class B : A { }
        class C : B, IMarker2 { }

        [Fact]
        public void GetAllBaseTypesAndInterfaces_FindsAllDerivations()
        {
            var types = typeof(C).GetAllBaseTypesAndInterfaces();

            Assert.Equal(6, types.Count);
            Assert.True(types[typeof(C)] == 0);
            Assert.True(types[typeof(IMarker2)] == 1);
            Assert.True(types[typeof(IMarker)] == 1);
            Assert.True(types[typeof(B)] == 2);
            Assert.True(types[typeof(A)] == 3);
            Assert.True(types[typeof(object)] == 4);
        }
    }
}
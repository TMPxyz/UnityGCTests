using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace MH.GCTests
{
    public class ComparerTest
    {
        public const string _constantStr = "HELLOWORLD";
        public string _globalStr = "HITHERE";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var eqV3 = EqualityComparer<Vector3>.Default; //eqV3 need warmup, eqString doesn't need it
        }

        [Test]
        public void GetHashCodeTest()
        {
            var localStr = "ABCDE";

            Assert.That( () => {100.GetHashCode();}, Is.Not.AllocatingGCMemory() );
            Assert.That( () => {100f.GetHashCode();},Is.Not.AllocatingGCMemory() );
            Assert.That( () => {((double)100).GetHashCode();}, Is.Not.AllocatingGCMemory() );
            Assert.That( () => {Vector3.one.GetHashCode();}, Is.Not.AllocatingGCMemory() ); 

            Assert.That( () => {EqualityComparer<Vector3>.Default.GetHashCode(Vector3.one * 2);}, Is.Not.AllocatingGCMemory() );

            Assert.That( () => {"hello".GetHashCode();}, Is.AllocatingGCMemory() ); //bad for literal
            Assert.That( () => {"hello".GetHashCode();}, Is.AllocatingGCMemory() ); //bad for literal
            Assert.That( () => {EqualityComparer<string>.Default.GetHashCode(localStr);}, Is.Not.AllocatingGCMemory() ); //good for string var
            Assert.That( () => {EqualityComparer<string>.Default.GetHashCode(_globalStr);}, Is.Not.AllocatingGCMemory() ); //good for string var
            Assert.That( () => {EqualityComparer<string>.Default.GetHashCode(_constantStr);}, Is.AllocatingGCMemory() ); //bad for string constant = literal
            Assert.That( () => {EqualityComparer<string>.Default.GetHashCode(_constantStr);}, Is.AllocatingGCMemory() ); //bad for string constant = literal
            Assert.That( () => {EqualityComparer<string>.Default.GetHashCode("XXX");}, Is.AllocatingGCMemory() ); //bad for string literal
            
        }
    }
}
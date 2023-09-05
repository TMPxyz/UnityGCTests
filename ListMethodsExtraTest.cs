using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Profiling;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace MH.GCTests
{
    public class ListMethodsExtra
    {
        const int ELEM_COUNT = 1000;
        const int LOOP_COUNT = 1000; //don't know why, but if the mem allocation is too small, the profiler.GetMonoUsedSizeLong cannot detect it

        List<DummyObjA> _cache = new List<DummyObjA>();

        [OneTimeSetUp]
        public void Init()
        {
            GC.Collect();
            Profiler.enabled = true;

            for(int i=0; i<ELEM_COUNT; ++i)
                _cache.Add(new DummyObjA(i));
        }

        [OneTimeTearDown]
        public void Fini()
        {
            Profiler.enabled = false;
            _cache.Clear();
        }

        [Test]
        public void OK_Reverse()
        {
            var cont = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(_cache[i]);
            
            //------------------//
            Assert.That(
                () => cont.Reverse(),
                Is.Not.AllocatingGCMemory()
            );

        }
    }
}

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
    public class IEnumerableParameter
    {
        const int ELEM_COUNT = 10;
        const int LOOP_COUNT = 100000;

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
        public void OK_List_AddRange()
        {
            var cont = new List<DummyObjA>();
            cont.AddRange(_cache);
            cont.Clear();

            Assert.That(
                () => 
                {
                    cont.Clear();
                    cont.AddRange(_cache);
                },
                Is.Not.AllocatingGCMemory()
            );

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                cont.AddRange(_cache);
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void OK_List_InsertRange()
        {
            var cont = new List<DummyObjA>();
            cont.AddRange(_cache);
            cont.Clear();

            Assert.That(
                () => 
                {
                    cont.Clear();
                    cont.InsertRange(0, _cache);
                },
                Is.Not.AllocatingGCMemory()
            );

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                cont.InsertRange(0, _cache);
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void BAD_HashSet_UnionWith()
        {
            var cont = new HashSet<DummyObjA>();
            cont.UnionWith(_cache);
            cont.Clear();

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                cont.UnionWith(_cache);
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.GreaterThan(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void OK_HashSet_IntersectWith()
        {
            var cont = new HashSet<DummyObjA>();
            cont.UnionWith(_cache);
            cont.Clear();

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                cont.IntersectWith(_cache);
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void OK_HashSet_ExceptWith()
        {
            var cont = new HashSet<DummyObjA>();
            cont.UnionWith(_cache);
            cont.Clear();

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                cont.ExceptWith(_cache);
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void BAD_HashSet_SymExcept()
        {
            var cont = new HashSet<DummyObjA>();
            cont.UnionWith(_cache);
            cont.Clear();

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                cont.SymmetricExceptWith(_cache);
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.GreaterThan(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }
    }
}
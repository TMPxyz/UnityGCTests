using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Profiling;

namespace MH.GCTests
{
    public class AddElement
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
        public void OK_List()
        {
            var cont = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(_cache[i]);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.Add(_cache[j]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
            
        }

        [Test]
        public void OK_Dictionary()
        {
            var cont = new Dictionary<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, _cache[i]);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.Add(j, _cache[j]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void OK_Queue()
        {
            var cont = new Queue<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Enqueue(_cache[i]);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.Enqueue(_cache[j]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void OK_Stack()
        {
            var cont = new Stack<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Push(_cache[i]);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.Push(_cache[j]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void OK_HashSet()
        {
            var cont = new HashSet<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(_cache[i]);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.Add(_cache[j]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void OK_SortedList()
        {
            var cont = new SortedList<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, _cache[i]);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.Add(j, _cache[j]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));            
        }

        //////////////////////////////////////////////////////////////////
        /// Below are tests that are GC_Dirty
        //////////////////////////////////////////////////////////////////

        [Test]
        public void BAD_SortedDictionary()
        {
            var cont = new SortedDictionary<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, _cache[i]);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.Add(j, _cache[j]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.GreaterThan(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void BAD_LinkedList()
        {
            var cont = new LinkedList<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.AddLast(_cache[i]);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.AddLast(_cache[j]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.GreaterThan(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }
    }

}
using UnityEngine;
using ExtMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Profiling;

namespace MH.GCTests
{
    public class Enumerate
    {
        const int ELEM_COUNT = 10;
        const int LOOP_COUNT = 10000; //don't know why, but if the mem allocation is too small, the profiler.GetMonoUsedSizeLong cannot detect it

        [OneTimeSetUp]
        public void Init()
        {
            GC.Collect();
            Profiler.enabled = true;
        }

        [OneTimeTearDown]
        public void Fini()
        {
            Profiler.enabled = false;
        }

        [Test]
        public void OK_List()
        {
            var cont = new List<int>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            int x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                foreach(var v in cont)
                    x += v;

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                for(var ie = cont.GetEnumerator(); ie.MoveNext(); )
                    x += ie.Current;

            long mem2 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem2, Is.EqualTo(startMem));

            //------------------//
            Debug.Log($"startMem = {startMem}, mem1={mem1}, mem2={mem2}");
        }

        [Test]
        public void OK_Dictionary()
        {
            var cont = new Dictionary<string, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i.ToString(), new DummyObjA(i));
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            int x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                foreach(var elem in cont)
                    x += elem.Value.v;

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                for(var ie = cont.GetEnumerator(); ie.MoveNext(); )
                    x += ie.Current.Value.v;

            long mem2 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem2, Is.EqualTo(startMem));

            //------------------//
            Debug.Log($"startMem = {startMem}, mem1={mem1}, mem2={mem2}");
        }

        [Test]
        public void OK_Queue()
        {
            var cont = new Queue<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Enqueue(new DummyObjA(i));
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            int x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                foreach(var elem in cont)
                    x += elem.v;

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                for(var ie = cont.GetEnumerator(); ie.MoveNext(); )
                    x += ie.Current.v;

            long mem2 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem2, Is.EqualTo(startMem));

            //------------------//
            Debug.Log($"startMem = {startMem}, mem1={mem1}, mem2={mem2}");
        }

        [Test]
        public void OK_Stack()
        {
            var cont = new Stack<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Push(new DummyObjA(i));
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            int x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                foreach(var elem in cont)
                    x += elem.v;

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                for(var ie = cont.GetEnumerator(); ie.MoveNext(); )
                    x += ie.Current.v;

            long mem2 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem2, Is.EqualTo(startMem));

            //------------------//
            Debug.Log($"startMem = {startMem}, mem1={mem1}, mem2={mem2}");
        }

        [Test]
        public void OK_HashSet()
        {
            var cont = new HashSet<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(new DummyObjA(i));
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            int x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                foreach(var elem in cont)
                    x += elem.v;

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                for(var ie = cont.GetEnumerator(); ie.MoveNext(); )
                    x += ie.Current.v;

            long mem2 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem2, Is.EqualTo(startMem));

            //------------------//
            Debug.Log($"startMem = {startMem}, mem1={mem1}, mem2={mem2}");
        }

        [Test]
        public void OK_SortedList_Keys()
        {
            var cont = new SortedList<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, new DummyObjA(i));
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            int x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                var keys = cont.Keys;
                for(int j=0; j<keys.Count; ++j)
                    x += cont[keys[j]].v;
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log($"startMem = {startMem}, mem1={mem1}");
        }

        //////////////////////////////////////////////////////
        /// below are tests which are GC_Dirty
        //////////////////////////////////////////////////////

        [Test]
        public void BAD_SortedList()
        {
            SortedList<int, string> lst = new SortedList<int, string>();
            for(int i=0; i<ELEM_COUNT; ++i)
                lst.Add(i, i.ToString());
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();
            // rec0.Record();

            //------------------//
            int x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                foreach(var v in lst)
                    x += v.Key;

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.GreaterThan(startMem));
            // rec1.Record();

            //------------------//
            GC.Collect();
            x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                for(var ie = lst.GetEnumerator(); ie.MoveNext(); )
                {
                    x += ie.Current.Key;
                }

            long mem2 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem2, Is.GreaterThan(startMem));
            // rec2.Record();

            //------------------//
            Debug.Log($"startMem = {startMem}, mem1={mem1}, mem2={mem2}");
            // rec0.Output();
            // rec1.Output();
            // rec2.Output();
        }

        [Test]
        public void BAD_SortedDictionary()
        {
            var cont = new SortedDictionary<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, new DummyObjA(i));
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            int x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                foreach(var elem in cont)
                    x += elem.Value.v;

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.GreaterThan(startMem));

            //------------------//
            GC.Collect();
            x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                for(var ie = cont.GetEnumerator(); ie.MoveNext(); )
                {
                    x += ie.Current.Value.v;
                }

            long mem2 = Profiler.GetMonoUsedSizeLong();
            // _OutputAll();
            Assert.That(mem2, Is.GreaterThan(startMem));

            //------------------//
            Debug.Log($"startMem = {startMem}, mem1={mem1}, mem2={mem2}");
        }

        // private void _OutputAll()
        // {
        //     string s = $"enabled={Profiler.enabled}, usedHeapSizeLong={Profiler.usedHeapSizeLong}, monoHeapSize={Profiler.GetMonoHeapSizeLong()}, monoUsedSize={Profiler.GetMonoUsedSizeLong()}, tempAllocator={Profiler.GetTempAllocatorSize()}, totalAllocate={Profiler.GetTotalAllocatedMemoryLong()}";
        //     Dbg.Log(s);
        // }
    }

    public class MemRecord
    {
        public long usedHeapSizeLong;
        public long monoHeapSize;
        public long monoUsedSize;
        public long tempAllocator;
        public long totalAllocate;
        public void Record()
        {
            usedHeapSizeLong = Profiler.usedHeapSizeLong;
            monoHeapSize = Profiler.GetMonoHeapSizeLong();
            monoUsedSize = Profiler.GetMonoUsedSizeLong();
            tempAllocator = Profiler.GetTempAllocatorSize();
            totalAllocate = Profiler.GetTotalAllocatedMemoryLong();
        }
        public void Output()
        {
            string s = $"enabled={Profiler.enabled}, usedHeapSizeLong={usedHeapSizeLong}, monoHeapSize={monoHeapSize}, monoUsedSize={monoUsedSize}, tempAllocator={tempAllocator}, totalAllocate={totalAllocate}";
            Dbg.Log(s);
        }
    }

    public class DummyObjA : IComparable<DummyObjA>
    {
        public int v;
        public DummyObjA(int v){ this.v = v;}

        public int CompareTo(DummyObjA other)
        {
            return v - other.v;
        }
    }   
}
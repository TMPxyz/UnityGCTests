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
    public class Enumerate
    {
        const int ELEM_COUNT = 10;
        const int LOOP_COUNT = 1000; //don't know why, but if the mem allocation is too small, the profiler.GetMonoUsedSizeLong cannot detect it

        List<DummyObjA> _cache = new List<DummyObjA>();

        [OneTimeSetUp]
        public void Init()
        {
            for(int i=0; i<ELEM_COUNT; ++i)
                _cache.Add(new DummyObjA(i));
            foreach(var x in _cache) {} //warmup
        }

        [OneTimeTearDown]
        public void Fini()
        {
            _cache.Clear();
        }

        [Test]
        public void BAD_ICollection()
        {
            var cont = new List<int>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i);

            Assert.That( () => 
                {
                    int v = 0;
                    ICollection<int> icol = cont;
                    foreach(var val in icol)
                    {
                        v += val;
                    }
                }, Is.AllocatingGCMemory() 
            );
        }

        [Test]
        public void BAD_IList_Foreach()
        {
            var cont = new List<int>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i);

            Assert.That( () => 
                {
                    int v = 0;
                    IList<int> icol = cont;
                    foreach(var val in icol)
                    {
                        v += val;
                    }
                }, Is.AllocatingGCMemory() 
            );
        }

        [Test]
        public void OK_IList_Index()
        {
            var cont = new List<int>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i);

            Assert.That( () => 
                {
                    int v = 0;
                    IList<int> icol = cont;
                    for(int i=0; i<icol.Count; ++i)
                    {
                        v += icol[i];
                    }
                }, Is.Not.AllocatingGCMemory() 
            );
        }

        [Test]
        public void OK_List()
        {
            var cont = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(_cache[i]);
            
            Assert.That( 
                () => {
                    int v = 0;
                    foreach(var x in cont){ v += x.v; }
                    for(var ie = cont.GetEnumerator(); ie.MoveNext(); )
                    {
                        v += ie.Current.v;
                    }
                }, Is.Not.AllocatingGCMemory()
            );
        }

        [Test]
        public void BAD_ReadOnlyCollectionForeachAndEnumerator()
        {
            var lst = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                lst.Add(new DummyObjA(i));

            var cont = lst.AsReadOnly();

            Assert.That( () => 
                { 
                    int k = 0;
                    foreach(var elem in cont)
                        k += elem.v; 
                }, 
                Is.AllocatingGCMemory()
            );

            Assert.That( () => 
                {
                    int k = 0;
                    for(var ie = cont.GetEnumerator(); ie.MoveNext(); )
                        k += ie.Current.v;
                },
                Is.AllocatingGCMemory()
            );

            // //------------------//
            // GC.Collect();
            // long startMem = Profiler.GetMonoUsedSizeLong();
            // int x = 0;
            // for(int i=0; i<LOOP_COUNT; ++i)
            //     foreach(var elem in cont)
            //         x += elem.v;

            // long mem1 = Profiler.GetMonoUsedSizeLong();
            // Assert.That(mem1, Is.GreaterThan(startMem));

            // //------------------//
            // x = 0;
            // for(int i=0; i<LOOP_COUNT; ++i)
            //     for(var ie = cont.GetEnumerator(); ie.MoveNext(); )
            //         x += ie.Current.v;

            // long mem2 = Profiler.GetMonoUsedSizeLong();
            // Assert.That(mem2, Is.GreaterThan(startMem));

            // //------------------//
            // Debug.Log(string.Format("startMem = {0}, mem1 = {1}, mem2 = {2}", startMem, mem1, mem2));
        }

        [Test]
        public void OK_ReadOnlyCollectionWithIndex()
        {
            var lst = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                lst.Add(new DummyObjA(i));

            var cont = lst.AsReadOnly();

            Assert.That( () => 
                { 
                    int k = 0;
                    for(int i=0; i<cont.Count; ++i)
                        k += cont[i].v; 
                }, 
                Is.Not.AllocatingGCMemory()
            );
        }

        [Test]
        public void OK_Dictionary()
        {
            var cont = new Dictionary<string, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i.ToString(), new DummyObjA(i));

            //warmup
            foreach(var pr in cont){}
            for(var ie = cont.GetEnumerator(); ie.MoveNext(); ) {}
            
            Assert.That( () => 
                { 
                    for(var ie = cont.GetEnumerator(); ie.MoveNext(); ){}
                },  
                Is.Not.AllocatingGCMemory()
            );

            Assert.That( () => 
                { 
                    foreach(var elem in cont){}
                }, 
                Is.Not.AllocatingGCMemory()
            );
        }

        [Test]
        public void OK_Queue()
        {
            var cont = new Queue<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Enqueue(new DummyObjA(i));

            //warmup
            foreach(var x in cont){}
            
            Assert.That(
                () => {
                    int v = 0;
                    foreach(var x in cont) { v+=x.v; }
                    for(var ie=cont.GetEnumerator(); ie.MoveNext(); ){  v+=ie.Current.v; }
                },
                Is.Not.AllocatingGCMemory()
            );            
        }

        [Test]
        public void OK_Stack()
        {
            var cont = new Stack<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Push(new DummyObjA(i));

            //warmup
            foreach(var x in cont){}
            
            Assert.That(
                () => {
                    int v = 0;
                    foreach(var x in cont) { v+=x.v; }
                    for(var ie=cont.GetEnumerator(); ie.MoveNext(); ){  v+=ie.Current.v; }
                },
                Is.Not.AllocatingGCMemory()
            );            
        }

        [Test]
        public void OK_HashSet()
        {
            var cont = new HashSet<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(new DummyObjA(i));

            //warmup
            foreach(var x in cont){}
            
            Assert.That(
                () => {
                    int v = 0;
                    foreach(var x in cont) { v+=x.v; }
                    for(var ie=cont.GetEnumerator(); ie.MoveNext(); ){  v+=ie.Current.v; }
                },
                Is.Not.AllocatingGCMemory()
            );            
        }

        [Test]
        public void OK_SortedList_Keys_With_Index()
        {
            var cont = new SortedList<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, new DummyObjA(i));
            var dummy = cont.Keys; //the backing variable of **Keys** is a one-time only alloc

            Assert.That(
                () => 
                {
                    int k = 0;
                    var keys = cont.Keys;
                    for(int j=0; j<keys.Count; ++j)
                        k += cont[keys[j]].v;
                },
                Is.Not.AllocatingGCMemory()
            );
        }

        [Test]
        public void BAD_SortedList_Keys_With_Foreach()
        {
            var cont = new SortedList<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, new DummyObjA(i));

            // warmup
            var dummy = cont.Keys; //the backing variable of **Keys** is a one-time only alloc
            foreach(var x in dummy){}
            
            Assert.That(
                () =>
                {
                    var keys = cont.Keys;
                    foreach(var o in keys) {}
                },
                Is.AllocatingGCMemory()
            );
        }

        [Test]
        public void OK_Dictionary_Keys()
        {
            var cont = new Dictionary<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, new DummyObjA(i));

            var keys = cont.Keys;
            //warmup
            foreach(var x in keys){}

            Assert.That(
                () => {
                    int v = 0;
                    keys = cont.Keys;
                    foreach(var x in keys) { v += x; }
                    for(var ie=keys.GetEnumerator(); ie.MoveNext(); ){ v += ie.Current; }
                },
                Is.Not.AllocatingGCMemory()
            );
        }

        [Test]
        public void OK_Dictionary_Values()
        {
            var cont = new Dictionary<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, new DummyObjA(i));
            
            var vals = cont.Values;
            //warmup
            foreach(var x in vals){}

            Assert.That(
                () => {
                    int v = 0;
                    vals = cont.Values;
                    foreach(var x in vals) { v += x.v; }
                    for(var ie=vals.GetEnumerator(); ie.MoveNext(); ){v += ie.Current.v; }
                },
                Is.Not.AllocatingGCMemory()
            );
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

            foreach(var v in lst){} //warmup

            Assert.That(
                () => {
                    int sum = 0;
                    foreach(var v in lst)
                        sum += v.Key;        
                }, Is.AllocatingGCMemory()
            );
        }

        [Test]
        public void BAD_SortedDictionary()
        {
            var cont = new SortedDictionary<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, new DummyObjA(i));

            foreach(var pr in cont) {} //warmup

            Assert.That(
                () => {
                    foreach(var pr in cont) {}
                },
                Is.AllocatingGCMemory()
            );

            Assert.That(
                () => {
                    for(var ie = cont.GetEnumerator(); ie.MoveNext(); ){}
                },
                Is.AllocatingGCMemory()
            );
        }

        [Test]
        public void BAD_SortedDictionary_Keys()
        {
            var cont = new SortedDictionary<int, DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(i, new DummyObjA(i));

            var keys = cont.Keys;
            // foreach(var k in keys) {} //warmup
            for(var ie = keys.GetEnumerator(); ie.MoveNext(); ){}

            Assert.That(
                () => {
                    for(var ie = keys.GetEnumerator(); ie.MoveNext(); ){}
                },
                Is.AllocatingGCMemory()
            );
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
            // string s = $"enabled={Profiler.enabled}, usedHeapSizeLong={usedHeapSizeLong}, monoHeapSize={monoHeapSize}, monoUsedSize={monoUsedSize}, tempAllocator={tempAllocator}, totalAllocate={totalAllocate}";
            string s = string.Format("enabled={0}, usedHeapSizeLong={1}, monoHeapSize={2}, monoUsedSize={3}, tempAllocator={4}, totalAllocate={5}", Profiler.enabled, usedHeapSizeLong, monoHeapSize, monoUsedSize, tempAllocator, totalAllocate);
            Debug.Log(s);
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
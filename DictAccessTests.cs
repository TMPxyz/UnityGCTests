using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Profiling;

namespace MH.GCTests
{
    public class DictAccess
    {
        public struct Data : IEquatable<Data>
        {
            public int x;
            public int y;

            public Data(int x_, int y_) { x = x_; y = y_; }

            public bool Equals(Data other)
            {
                return this.x == other.x && this.y == other.y;
            }
        }

        const int ELEM_COUNT = 1000;
        const int LOOP_COUNT = 1000; //don't know why, but if the mem allocation is too small, the profiler.GetMonoUsedSizeLong cannot detect it


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
        public void BAD_AccessStructKey()
        {
            var cache = new Dictionary<Data, int>();
            var cont = new List<int>(ELEM_COUNT);
            for(int i=0; i<ELEM_COUNT; ++i)
                cache.Add(new Data(i, i), i);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.Add(cache[new Data(j,j)]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.GreaterThan(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
            
        }

        [Test]
        public void OK_AccessStructKeyWithExplicitComparer()
        {
            var cache = new Dictionary<Data, int>( new _DataKeyComparer() );
            var cont = new List<int>(ELEM_COUNT);
            for(int i=0; i<ELEM_COUNT; ++i)
                cache.Add(new Data(i, i), i);
            
            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
            {
                cont.Clear();
                for(int j=0; j<ELEM_COUNT; ++j)
                {
                    cont.Add(cache[new Data(j,j)]);
                }
            }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        public class _DataKeyComparer : IEqualityComparer<Data>
        {
            public bool Equals(Data x, Data y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(Data obj)
            {
                return obj.x << 16 + obj.y;
            }
        }
    }

}
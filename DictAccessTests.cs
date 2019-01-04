using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Profiling;
using UnityEngine.TestTools.Constraints;

using Is = NUnit.Framework.Is;

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

        public class CData : IEquatable<CData>
        {
            public int x;
            public int y;
            public CData(int x_,  int y_) { x = x_; y = y_; }
            public bool Equals(CData other)
            {
                return this.x == other.x && this.y == other.y;
            }
        }

        public class AData : IEquatable<AData>
        {
            public int x;
            public int y;
            public AData(int x_,  int y_) { x = x_; y = y_; }
            // public override bool Equals(object obj)
            // {
            //     if ( object.ReferenceEquals(this, obj) )
            //         return true;
            //     if (! (obj is AData) )
            //         return false;

            //     var o = (AData)obj;
            //     return x == o.x && y == o.y;
            // }
            public bool Equals(AData o) //this will do too
            {
                return x == o.x && y == o.y;
            }
            public override int GetHashCode()
            {
                return x << 16 + y;
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

        ///<summary>
        /// Without comparer, it will cause boxing when compare keys 
        ///
        /// it seems very slow to access struct keys without explicit comparer
        ///</summary>
        [Test]
        public void BAD_AccessStructKey()
        {
            var cache = new Dictionary<Data, int>();
            var cont = new List<int>(ELEM_COUNT);
            for(int i=0; i<ELEM_COUNT; ++i)
                cache.Add(new Data(i, i), i);

            Assert.That( () => 
                {
                    for(int j=0; j<ELEM_COUNT; ++j)
                        cont.Add(cache[new Data(j,j)]);
                },
                UnityEngine.TestTools.Constraints.Is.AllocatingGCMemory()
            );
            
            
            // GC.Collect();
            // long startMem = Profiler.GetMonoUsedSizeLong();

            // //------------------//
            // for(int i=0; i<LOOP_COUNT; ++i)
            // {
            //     cont.Clear();
            //     for(int j=0; j<ELEM_COUNT; ++j)
            //     {
            //         cont.Add(cache[new Data(j,j)]);
            //     }
            // }

            // long mem1 = Profiler.GetMonoUsedSizeLong();
            // Assert.That(mem1, Is.GreaterThan(startMem));

            // //------------------//
            // Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
            
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

        public class _DataKeyComparer : EqualityComparer<Data>
        {
            public override bool Equals(Data x, Data y)
            {
                return x.Equals(y);
            }

            public override int GetHashCode(Data obj)
            {
                return obj.x << 16 + obj.y;
            }
        }

        [Test]
        public void OK_AccessClassKeyWithExplicitComparer()
        {
            var cache = new Dictionary<CData, int>(new _CDataComparer());
            var cont = new List<CData>();
            for(int i=0; i<ELEM_COUNT; ++i)
            {
                var o0 = new CData(i, i);
                cache.Add( o0, i );
                var o1 = new CData(i, i);
                cont.Add(o1);

                Assert.That(cache.ContainsKey(o1));
                Assert.That(o0.Equals(o1));
            }

            int k = 0;
            Assert.That( () => 
                {
                    for(int j=0; j<ELEM_COUNT; ++j)
                    {
                        var cdata = cont[j];
                        var v = cache[cdata];
                        k += v;
                    }
                },
                UnityEngine.TestTools.Constraints.Is.Not.AllocatingGCMemory()
            );
            Assert.That(k, Is.EqualTo((ELEM_COUNT-1) * ELEM_COUNT / 2));
        }

        public class _CDataComparer : EqualityComparer<CData>
        {
            public override bool Equals(CData x, CData y)
            {
                return x.Equals(y);
            }

            public override int GetHashCode(CData obj)
            {
                return obj.x << 16 + obj.y;
            }
        }

        [Test]
        public void OK_AccessClassKeyWithOverrideEqualAndHashCode()
        {
            var cache = new Dictionary<AData, int>();
            var cont = new List<AData>();
            for(int i=0; i<ELEM_COUNT; ++i)
            {
                var o0 = new AData(i, i);
                cache.Add( o0, i );
                var o1 = new AData(i, i);
                cont.Add(o1);

                Assert.That(cache.ContainsKey(o1));
                Assert.That(o0.Equals(o1));
            }

            int k = 0;
            Assert.That( () => 
                {
                    for(int j=0; j<ELEM_COUNT; ++j)
                    {
                        var data = cont[j];
                        var v = cache[data];
                        k += v;
                    }
                },
                UnityEngine.TestTools.Constraints.Is.Not.AllocatingGCMemory()
            );
            Assert.That(k, Is.EqualTo((ELEM_COUNT-1) * ELEM_COUNT / 2));
        }
    }

}
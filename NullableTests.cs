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
    using Random = UnityEngine.Random;

    public class NullableTests
    {
        const int LOOP_COUNT = 100000; //don't know why, but if the mem allocation is too small, the profiler.GetMonoUsedSizeLong cannot detect it

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
        public void OK_Nullable()
        {
            GC.Collect();
            //------------------//
            Assert.That(
                () => {
                    for (int i = 0; i<100; ++i)
                        _ReturnNullableInt(i).GetValueOrDefault();
                },
                Is.Not.AllocatingGCMemory()
            );
        }


        private int? _ReturnNullableInt(int v)
        {
            if( Random.value < 0.5f )
            {
                return v;
            }
            else
            {
                return null;
            }
        }
    }
}
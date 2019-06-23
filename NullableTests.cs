using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Profiling;

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
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            int x = 0;
            for(int i=0; i<LOOP_COUNT; ++i)
                x += _ReturnNullableInt(i).GetValueOrDefault();

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
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
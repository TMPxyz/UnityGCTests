using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Profiling;

namespace MH.GCTests
{
    public class DelegateTests
    {
        const int ELEM_COUNT = 10;
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
        public void OK_LocalLambda()
        {
            var cont = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(new DummyObjA(i));

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
                for(int j=0; j<ELEM_COUNT; ++j)       
                    cont.ForEach(x => x.v += 1);

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        [Test]
        public void BAD_MemberMethod() //Member method is indeed same as closure
        {
            var cont = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(new DummyObjA(i));

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
                for(int j=0; j<ELEM_COUNT; ++j)       
                    cont.ForEach( x => _ProcessDummyObj(x) );

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.GreaterThan(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        private void _ProcessDummyObj(DummyObjA o)
        {
            o.v ++;
        }

        [Test]
        public void OK_StaticMethod()
        {
            var cont = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(new DummyObjA(i));

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
                for(int j=0; j<ELEM_COUNT; ++j)       
                    cont.ForEach( x => _StaticProcessDummyObj(x) );

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        private static void _StaticProcessDummyObj(DummyObjA x)
        {
            x.v ++;
        }

        [Test]
        public void BAD_ClosureLambda()
        {
            var cont = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(new DummyObjA(i));

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
                for(int j=0; j<ELEM_COUNT; ++j)       
                    cont.ForEach(x => x.v += j);

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.GreaterThan(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }

        private int _cacheDelegate_var = 0;
        [Test]
        public void OK_CachedDelegate()
        {
            var cont = new List<DummyObjA>();
            for(int i=0; i<ELEM_COUNT; ++i)
                cont.Add(new DummyObjA(i));

            GC.Collect();
            long startMem = Profiler.GetMonoUsedSizeLong();

            Action<DummyObjA> del = (o) => o.v += _cacheDelegate_var;

            //------------------//
            for(int i=0; i<LOOP_COUNT; ++i)
                for(int j=0; j<ELEM_COUNT; ++j){
                    _cacheDelegate_var = j;       
                    cont.ForEach(del);
                }

            long mem1 = Profiler.GetMonoUsedSizeLong();
            Assert.That(mem1, Is.EqualTo(startMem));

            //------------------//
            Debug.Log(string.Format("startMem = {0}, mem1 = {1}", startMem, mem1));
        }
    }
}
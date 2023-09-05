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

    public class ParamTests
    {

        public void _VariableParam(int v, params int[] vs){ 
            // do nothing 
        }

        [Test]
        public void BAD_VariableParam()
        {
            Assert.That(
                () => _VariableParam(10, 1, 2, 3),
                Is.AllocatingGCMemory()
            );
        }
    }

}
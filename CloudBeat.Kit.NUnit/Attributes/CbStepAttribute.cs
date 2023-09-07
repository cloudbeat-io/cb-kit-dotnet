using AspectInjector.Broker;
using CloudBeat.Kit.NUnit.Aspects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudBeat.Kit.NUnit.Attributes
{
    [Injection(typeof(CbStepAspect))]
    [AttributeUsage(AttributeTargets.Method)]
    public class CbStepAttribute : Attribute
    {
        public CbStepAttribute() { }
        public CbStepAttribute(string stepName)
        {
            StepName = stepName;
        }

        public string StepName { get; set; }
    }
}

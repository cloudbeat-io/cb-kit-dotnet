using AspectInjector.Broker;
using CloudBeat.Kit.MSTest.Aspects;
using System;

namespace CloudBeat.Kit.MSTest.Attributes
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

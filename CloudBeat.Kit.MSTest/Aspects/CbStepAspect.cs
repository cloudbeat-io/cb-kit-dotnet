using System;
using System.Collections.Generic;
using System.Reflection;
using AspectInjector.Broker;
using CloudBeat.Kit.MSTest.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudBeat.Kit.MSTest.Aspects
{
    [Aspect(Scope.Global)]
    public class CbStepAspect
    {
        [Advice(Kind.Around, Targets = Target.Method)]
        public object WrapStep(
            [Argument(Source.Name)] string methodName,
            [Argument(Source.Metadata)] MethodBase methodBase,
            [Argument(Source.Arguments)] object[] arguments,
            [Argument(Source.Target)] Func<object[], object> method)
        {
            if (!CbMSTest.Current.IsConfigured)
                return method(arguments);
            var stepName = methodBase.GetCustomAttribute<CbStepAttribute>().StepName;
            bool isTestSetUpHook = methodBase.GetCustomAttribute<TestInitializeAttribute>() != null;
            bool isTestTearDownHook = methodBase.GetCustomAttribute<TestCleanupAttribute>() != null;
            bool isClassSetUpHook = methodBase.GetCustomAttribute<ClassInitializeAttribute>() != null;
            bool isClassTearDownHook = methodBase.GetCustomAttribute<ClassCleanupAttribute>() != null;
            bool isHook = isTestSetUpHook || isTestTearDownHook || isClassSetUpHook || isClassTearDownHook;
            string hookName = GetHookName(isTestSetUpHook, isTestTearDownHook, isClassSetUpHook, isClassTearDownHook);
            var reporter = CbMSTest.Current.Reporter;
            if (isHook)
                return reporter.Hook(stepName ?? hookName, methodName, method, arguments);
            return reporter.Step(stepName ?? methodName, method, arguments);
        }

        private string GetHookName(bool isTestSetUpHook, bool isTestTearDownHook, bool isClassSetUpHook, bool isClassTearDownHook)
        {
            // theoretically, the same method might be taged with multiple hook attributes
            // e.g. serve as SetUp and TearDown
            // in that case, hook name will combine multiple hooks in it, joined by ' \ '
            List<string> hookNames = new List<string>();
            if (isTestSetUpHook)
                hookNames.Add("TestInitialize");
            if (isTestTearDownHook)
                hookNames.Add("TestCleanup");
            if (isClassSetUpHook)
                hookNames.Add("ClassInitialize");
            if (isClassTearDownHook)
                hookNames.Add("ClassCleanup");
            return string.Join(" \\ ", hookNames);
        }
    }
}

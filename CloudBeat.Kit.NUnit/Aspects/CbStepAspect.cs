using System;
using System.Collections.Generic;
using System.Reflection;
using AspectInjector.Broker;
using CloudBeat.Kit.NUnit.Attributes;
using NUnit.Framework;

namespace CloudBeat.Kit.NUnit.Aspects
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
            if (!CbNUnit.Current.IsConfigured)
                return method(arguments);
            var stepName = methodBase.GetCustomAttribute<CbStepAttribute>().StepName;
            bool isSetUpHook = methodBase.GetCustomAttribute<SetUpAttribute>() != null;
            bool isTearDownHook = methodBase.GetCustomAttribute<TearDownAttribute>() != null;
            bool isOneTimeSetUpHook = methodBase.GetCustomAttribute<OneTimeSetUpAttribute>() != null;
            bool isOneTimeTearDownHook = methodBase.GetCustomAttribute<OneTimeTearDownAttribute>() != null;
            bool isHook = isSetUpHook || isTearDownHook || isOneTimeSetUpHook || isOneTimeTearDownHook;
            string hookName = GetHookName(isSetUpHook, isTearDownHook, isOneTimeSetUpHook, isOneTimeTearDownHook);
            var reporter = CbNUnit.Current.Reporter;
            /*var stepResult = string.IsNullOrEmpty(stepName)
                ? new StepResult { name = name, parameters = AllureStepParameterHelper.CreateParameters(arguments) }
                : new StepResult { name = stepName, parameters = AllureStepParameterHelper.CreateParameters(arguments) };
            */
            if (isHook)
                return reporter.Hook(stepName ?? hookName, methodName, method, arguments);
            return reporter.Step(stepName ?? methodName, method, arguments);
        }

        private string GetHookName(bool isSetUpHook, bool isTearDownHook, bool isOneTimeSetUpHook, bool isOneTimeTearDownHook)
        {
            // theoretically, the same method might be taged with multiple hook attributes
            // e.g. serve as SetUp and TearDown
            // in that case, hook name will combine multiple hooks in it, joined by ' \ '
            List<string> hookNames = new List<string>();
            if (isOneTimeSetUpHook)
                hookNames.Add("OneTimeSetUpHook");
            if (isOneTimeTearDownHook)
                hookNames.Add("OneTimeTearDownHook");
            if (isSetUpHook)
                hookNames.Add("SetUpHook");
            if (isTearDownHook)
                hookNames.Add("TearDownHook");
            return string.Join(" \\ ", hookNames);
        }
    }
}

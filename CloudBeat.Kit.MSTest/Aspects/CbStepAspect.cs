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
            string fullMethodName = $"{methodBase.DeclaringType}.{methodName}";
            return reporter.Step(
                ParameterizeStepName(stepName, methodBase.GetParameters(), arguments) ?? methodName,
                fullMethodName,
                method,
                arguments
            );
        }

        private static string ParameterizeStepName(string stepName, ParameterInfo[] parameterInfos, object[] arguments)
        {
            if (string.IsNullOrEmpty(stepName))
            {
                return stepName;
            }

            string parameterizedStepName = stepName;
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                var paramInfo = parameterInfos[i];
                if (string.IsNullOrEmpty(paramInfo.Name) || i >= arguments.Length)
                {
                    continue;
                }

                if (arguments[i] == null)
                {
                    parameterizedStepName = parameterizedStepName.Replace("{" + paramInfo.Name + "}", "null");
                    continue;
                }
                else if (arguments[i].GetType().IsPrimitive || arguments[i] is string)
                {
                    parameterizedStepName = parameterizedStepName.Replace("{" + paramInfo.Name + "}", arguments[i].ToString());
                }

                // Try to parametrize complex object (e.g. with public properties)
                IList<PropertyInfo> objProps = new List<PropertyInfo>(arguments[i].GetType().GetProperties());
                foreach (PropertyInfo objProp in objProps)
                {
                    var complexParamNameWithBraces = "{" + paramInfo.Name + "." + objProp.Name + "}";
                    if (!parameterizedStepName.Contains(complexParamNameWithBraces))
                    {
                        continue;
                    }

                    var objPropVal = objProp.GetValue(arguments[i]);
                    if (objPropVal == null)
                    {
                        continue;
                    }

                    parameterizedStepName = parameterizedStepName.Replace(complexParamNameWithBraces, objPropVal.ToString());
                }
            }
            return parameterizedStepName;
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

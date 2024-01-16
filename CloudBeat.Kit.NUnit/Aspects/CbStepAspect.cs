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
            if (isHook)
                return reporter.Hook(stepName ?? hookName, methodName, method, arguments);
            string fullMethodName = $"{methodBase.DeclaringType}.{methodName}";
            return reporter.StepWithFqn(
                ParameterizeStepName(stepName, methodBase.GetParameters(), arguments) ?? methodName,
                fullMethodName,
                method,
                arguments
            );
        }

        private static string ParameterizeStepName(
            string stepName,
            ParameterInfo[] parameterInfos,
            object[] arguments)
        {
            if (string.IsNullOrEmpty(stepName))
                return stepName;
            string parameterizedStepName = stepName;
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                var paramInfo = parameterInfos[i];
                if (string.IsNullOrEmpty(paramInfo.Name) || i >= arguments.Length)
                    continue;
                if (arguments[i].GetType().IsPrimitive || arguments[i] is string)
                    parameterizedStepName = parameterizedStepName
                        .Replace("{" + paramInfo.Name + "}", arguments[i].ToString());
                // Try to parametrize complex object (e.g. with public properties)
                IList<PropertyInfo> objProps = new List<PropertyInfo>(arguments[i].GetType().GetProperties());
                foreach (PropertyInfo objProp in objProps)
                {
                    var complexParamNameWithBraces = "{" + paramInfo.Name + "." + objProp.Name + "}";
                    if (!parameterizedStepName.Contains(complexParamNameWithBraces))
                        continue;
                    var objPropVal = objProp.GetValue(arguments[i]);
                    if (objPropVal == null)
                        continue;
                    parameterizedStepName = parameterizedStepName
                        .Replace(complexParamNameWithBraces, objPropVal.ToString());
                }
            }
            return parameterizedStepName;
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

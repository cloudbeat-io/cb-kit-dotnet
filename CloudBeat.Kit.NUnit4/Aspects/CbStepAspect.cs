using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AspectInjector.Broker;
using CloudBeat.Kit.Common.Models.Hook;
using CloudBeat.Kit.NUnit.Attributes;
using NUnit.Framework;

namespace CloudBeat.Kit.NUnit.Aspects
{
    [Aspect(Scope.Global)]
    public class CbStepAspect
    {
        private static readonly MethodInfo _asyncGenericHandler =
            typeof(CbStepAspect).GetMethod(nameof(CbStepAspect.WrapAsync), BindingFlags.NonPublic | BindingFlags.Static);
        
        private static readonly MethodInfo _syncGenericHandler =
            typeof(CbStepAspect).GetMethod(nameof(CbStepAspect.WrapSync), BindingFlags.NonPublic | BindingFlags.Static);
        
        [Advice(Kind.Around, Targets = Target.Method)]
        public object WrapStep(
            [Argument(Source.Name)] string methodName,
            [Argument(Source.Metadata)] MethodBase methodBase,
            [Argument(Source.Arguments)] object[] arguments,
            [Argument(Source.Target)] Func<object[], object> method,
            [Argument(Source.ReturnType)] Type retType)
        {
            if (!CbNUnit.Current.IsConfigured)
                return method(arguments);
            var stepName = methodBase.GetCustomAttribute<CbStepAttribute>()?.StepName;
            bool isSetUpHook = methodBase.GetCustomAttribute<SetUpAttribute>() != null;
            bool isTearDownHook = methodBase.GetCustomAttribute<TearDownAttribute>() != null;
            bool isOneTimeSetUpHook = methodBase.GetCustomAttribute<OneTimeSetUpAttribute>() != null;
            bool isOneTimeTearDownHook = methodBase.GetCustomAttribute<OneTimeTearDownAttribute>() != null;
            bool isHook = isSetUpHook || isTearDownHook || isOneTimeSetUpHook || isOneTimeTearDownHook;
            string hookName = GetHookName(isSetUpHook, isTearDownHook, isOneTimeSetUpHook, isOneTimeTearDownHook);
            var reporter = CbNUnit.Current.Reporter;
            var testId = TestContext.CurrentContext.Test.ID;
            reporter.SetCurrentTestId(testId);
            if (isOneTimeSetUpHook)
                return reporter.SuiteHook(stepName ?? hookName, HookTypeEnum.Before, methodName, method, arguments);
            else if (isOneTimeTearDownHook)
                return reporter.SuiteHook(stepName ?? hookName, HookTypeEnum.After, methodName, method, arguments);
            string methodFqn = $"{methodBase.DeclaringType}.{methodName}";
            string methodDisplayName = ParameterizeStepName(stepName, methodBase.GetParameters(), arguments) ?? methodName;
            try
            {
                if (typeof(Task).IsAssignableFrom(retType))
                {
                    if (retType.IsConstructedGenericType)
                        return _asyncGenericHandler.MakeGenericMethod(retType.GenericTypeArguments[0]).Invoke(null, new object[] {
                            method/*ConvertToTaskFunc(method, arguments)*/, arguments, methodDisplayName, methodFqn, reporter
                        });
                    else
                        return WrapAsyncVoid(
                            ConvertToVoidTaskFunc(method, arguments), methodDisplayName, methodFqn, reporter
                        );
                }
                else if (retType == typeof(void))
                {
                    return reporter.StepWithFqn(methodDisplayName, methodFqn, method, arguments);
                }
                else
                {
                    return _syncGenericHandler.MakeGenericMethod(retType).Invoke(null, new object[] {
                    method, arguments, methodDisplayName, methodFqn, reporter
                });
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static Func<Task<object>> ConvertToTaskFunc(Func<object[], object> func, object[] args)
        {
            return () => (Task<object>)func(args);
        }
        
        private static Func<Task<T>> ConvertToTypedTaskFunc<T>(Func<object[], object> func, object[] args)
        {
            return () => (Task<T>)func(args);
        }

        private static Func<Task> ConvertToVoidTaskFunc(Func<object[], object> func, object[] args)
        {
            return () => (Task)func(args);
        }

        private static T WrapSync<T>(
            Func<object[], object> func,
            object[] args,
            string methodName,
            string methodFqn,
            CbNUnitTestReporter reporter)
        {
            try
            {
                return (T)reporter.StepWithFqn(
                    methodName,
                    methodFqn,
                    func,
                    args
                );
            }
            catch (Exception e)
            {
                // return default(T);
                throw;
            }
        }

        private static async Task<T> WrapAsync<T>(
            //Func<Task<T>> func,
            Func<object[], object> method,
            object[] arguments,
            string methodName,
            string methodFqn,
            CbNUnitTestReporter reporter)
        {
            try
            {
                var asyncFunc = ConvertToTypedTaskFunc<T>(method, arguments);
                return (T)await reporter.StepWithFqnAsync(
                    methodName,
                    methodFqn,
                    asyncFunc
                );
                // return await (Task<T>)func(args);
            }
            catch (Exception e)
            {
                throw;
                // return default(T);
            }
        }

        private static async Task WrapAsyncVoid(
            Func<Task> func,
            string methodName,
            string methodFqn,
            CbNUnitTestReporter reporter)
        {
            try
            {
                await reporter.StepWithFqnAsync(
                    methodName,
                    methodFqn,
                    func
                );
                // return await (Task<T>)func(args);
            }
            catch (Exception e)
            {
                throw;
                // return default(T);
            }
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

using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Models;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using CbExceptionHelper = CloudBeat.Kit.Common.CbExceptionHelper;
using System.Collections.Concurrent;
using System.Threading;
using System.Reflection;

namespace CloudBeat.Kit.NUnit
{
    public class CbNUnitTestReporter : CbTestReporter
    {
        protected readonly ConcurrentDictionary<string, CaseResult> _startedCasePerThread = new ConcurrentDictionary<string, CaseResult>();
        private static readonly object _lock = new object();

        public CbNUnitTestReporter(CbConfig config) : base(config)
        {
        }
        
        private void WriteCaseResultToFile(CaseResult caseResult)
        {
            string technologyName = TestContext.Parameters["technologyName"]?.ToString();
            if (string.IsNullOrEmpty(technologyName))
                return;
            // if no direct API access details were defined, then write case result to local file
            // so CB Logger can pick it up and send it indirectly to CB server
            var cwd = TestContext.CurrentContext.WorkDirectory;
            var caseResultFile = $"{CbGeneralHelpers.FqnToFileName(caseResult.Fqn)}_case_result.json";
            var fullFilePath = Path.Combine(cwd, caseResultFile);
            File.WriteAllText(fullFilePath, JsonConvert.SerializeObject(caseResult));
            TestContext.AddTestAttachment(fullFilePath);
        }

        public void StartSuite(TestSuite suite)
        {
            var categoryAttributes = suite.GetCustomAttributes<CategoryAttribute>(true);
            base.StartSuite(suite.Name, suite.FullName, x =>    // NUnitHelpers.GetTestSuiteFqn(
            {
                x.Arguments = suite.Arguments?.Select(a => a.ToString()).ToArray();
                AddCategoriesAsTagsAndTestAttributes(x, categoryAttributes);
            });
        }

		private static void AddCategoriesAsTagsAndTestAttributes(SuiteResult suiteResult, CategoryAttribute[] categoryAttributes)
		{
            foreach (var categoryAttr in categoryAttributes)
			{
                if (string.IsNullOrEmpty(categoryAttr.Name))
                    continue;
                if (categoryAttr.Name.Contains(":"))
				{
                    var keyVal = categoryAttr.Name.Split(':');
                    if (keyVal.Length < 2) continue;
                    if (suiteResult.TestAttributes.ContainsKey(keyVal[0])) continue;
                    suiteResult.TestAttributes.Add(keyVal[0], keyVal[1]);
				}
			}
		}

        private static void AddCategoriesAsTagsAndTestAttributes(CaseResult caseResult, CategoryAttribute[] categoryAttributes)
        {
            foreach (var categoryAttr in categoryAttributes)
            {
                if (string.IsNullOrEmpty(categoryAttr.Name))
                    continue;
                if (categoryAttr.Name.Contains(":"))
                {
                    var keyVal = categoryAttr.Name.Split(':');
                    if (keyVal.Length < 2) continue;
                    if (caseResult.TestAttributes.ContainsKey(keyVal[0])) continue;
                    caseResult.TestAttributes.Add(keyVal[0], keyVal[1]);
                }
            }
        }

        public void StartCase(Test test)
        {
            var categoryAttributes = test.GetCustomAttributes<CategoryAttribute>(true);
            lock (_lock)
            {
                base.StartCase(test.Name, test.FullName, x =>
                {
                    var testParams = NUnitHelpers.GenerateTestParametersContext(test.Method, test.Arguments);
                    x.Arguments = test.Arguments?.Select(a => a.ToString()).ToArray();
                    if (x.Context.ContainsKey("params"))
                        x.Context["params"] = testParams;
                    else
                        x.Context.Add("params", testParams);

                    AddCategoriesAsTagsAndTestAttributes(x, categoryAttributes);
                    // store for later use the current test case result object (per thread)
                    var threadId = Thread.CurrentThread.ManagedThreadId.ToString();
                    if (!_startedCasePerThread.TryAdd(threadId, x))
                        _startedCasePerThread[threadId] = x;
                });
            }
        }

        public bool EndSuite(TestSuite suite)
        {
            return base.EndSuite(suite.FullName); // NUnitHelpers.GetTestSuiteFqn(
        }

        public void EndCase(Test test)
        {
            var nuTestResult = TestContext.CurrentContext.Result;
            // generate failure object, if relevant
            FailureResult failure = GetFailureFromResult(nuTestResult);
            var status = NUnitHelpers.DetermineTestStatus(nuTestResult.Outcome);
            lock (_lock)
            {
                var startedCase = GetStartedCase();
                var fqn = test.FullName; // NUnitHelpers.GetTestCaseFqn(test);
                CaseResult endedCase;
                // remove ended case from current case per-thread storage
                if (startedCase != null)
                {
                    var threadId = Thread.CurrentThread.ManagedThreadId.ToString();
                    _startedCasePerThread.Remove(threadId, out startedCase);
                }
                endedCase = base.EndCase(fqn, status, failure);
                WriteCaseResultToFile(endedCase);
            }
        }
        private static FailureResult GetFailureFromResult(TestContext.ResultAdapter result)
		{
            if (!NUnitHelpers.IsFailure(result.Outcome) && !NUnitHelpers.IsError(result.Outcome))
                return null;
            var failure = new FailureResult();
            failure.Message = result.Message;
            failure.Data = result.StackTrace;
            // check if this is assertion related error
            bool isAssertionFailure = result.Assertions.Any(x => x.Message == result.Message && x.StackTrace == result.StackTrace);
            failure.Type = isAssertionFailure ? CbExceptionHelper.ERROR_TYPE_ASSERT : CbExceptionHelper.ERROR_TYPE_GENERAL;
            //failure.Location = GetLocationByStackTrace(result.StackTrace);
            return failure;
        }
		
        public TResult Hook<T, TResult>(string hookName, string methodName, Func<T, TResult> func, T arg)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return base.Step(testFqn, hookName, StepTypeEnum.Hook, func, arg, x => x.MethodName = methodName);
        }
        public TResult Step<T, TResult>(string name, Func<T, TResult> func, T arg)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return base.Step(testFqn, name, func, arg);
        }
        public TResult StepWithFqn<T, TResult>(string name, string fqn, Func<T, TResult> func, T arg)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            var step = base.Step(testFqn, name, func, arg);
            return base.Step(testFqn, name, StepTypeEnum.General, func, arg, x => x.Fqn = fqn);
        }
        public TResult Step<TResult>(string name, Func<TResult> func)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return base.Step<TResult>(testFqn, name, StepTypeEnum.General, func);
        }
        // We must override the standard action Step method,
        // as in Nunit we don't necessary know upfront which test method is currently executed
        public StepResult Step(string name, Action action)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return base.Step(testFqn, name, action);
        }
        public override StepResult StartStep(string stepName, StepTypeEnum type = StepTypeEnum.General, CaseResult parentCase = null)
        {
            // var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            // return base.StartStep(testFqn, name);
            return base.StartStep(stepName, type, GetStartedCase());
        }

        public override StepResult EndStep(
            StepResult stepResult,
            TestStatusEnum? status = null,
            Exception exception = null,
            string screenshot = null)
        {
            return base.EndStep(stepResult, status, exception, screenshot);
        }

        public void HasWarnings(bool hasWarnings = true)
        {
            SetCaseHasWarnings(hasWarnings);
        }

        private CaseResult GetStartedCase()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId.ToString();
            _startedCasePerThread.TryGetValue(threadId, out CaseResult caseResult);
            return caseResult;
        }
    }
}

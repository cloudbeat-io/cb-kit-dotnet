using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Models;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CloudBeat.Kit.Common.Attributes;
using CloudBeat.Kit.Common.Json;
using CloudBeat.Kit.Common.Models.Hook;

namespace CloudBeat.Kit.NUnit
{
    public class CbNUnitTestReporter : CbTestReporter
    {
        private static readonly object _lock = new object();

        public CbNUnitTestReporter(CbConfig config) : base(config, TestContext.Progress)
        {
        }
        
        private void WriteCaseResultToFile(CaseResult caseResult)
        {
            // If no direct API access details were defined, then write case result to local file
            // so CB Logger can pick it up and send it indirectly to CB server
            var cwd = TestContext.CurrentContext.WorkDirectory;
            var caseResultFile = $"{CbGeneralHelpers.FqnToFileName(caseResult.Fqn)}_case_result.json";
            var fullFilePath = Path.Combine(cwd, caseResultFile);
			File.WriteAllText(fullFilePath, CbJsonConvert.SerializeObject(caseResult));
            TestContext.AddTestAttachment(fullFilePath);
        }

        public void StartSuite(TestSuite suite, TestSuite parentSuite)
        {
            StartSuite(suite.Name, suite.FullName, parentSuite?.FullName, x =>
            {
                x.Arguments = suite.Arguments?.Select(a => a.ToString()).ToList();
                ProcessSuiteAttributes(suite, x);
            });
        }

		private static void ProcessSuiteAttributes(TestSuite suite, SuiteResult suiteResult)
		{
            var categoryAttributes = suite.GetCustomAttributes<CategoryAttribute>(true);
            foreach (var categoryAttr in categoryAttributes)
			{
                if (string.IsNullOrEmpty(categoryAttr.Name))
                    continue;
                if (categoryAttr.Name.Contains(':'))
				{
                    var keyVal = categoryAttr.Name.Split(':');
                    if (keyVal.Length < 2) continue;
                    if (suiteResult.TestAttributes.ContainsKey(keyVal[0])) continue;
                    suiteResult.TestAttributes.Add(keyVal[0], keyVal[1]);
				}
			}
            var ownerAttributes = suite.GetCustomAttributes<CbOwner>(true);
            if (ownerAttributes.Length > 0)
            {
                suiteResult.Owner = ownerAttributes[0].Name;
            }
		}

        private static void ProcessTestCategories(Test test, CaseResult caseResult)
        {
            var categoryAttributes = test.GetCustomAttributes<CategoryAttribute>(true);
            foreach (var categoryAttr in categoryAttributes)
            {
                if (string.IsNullOrEmpty(categoryAttr.Name))
                    continue;
                if (categoryAttr.Name.Contains(':'))
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
            lock (_lock)
            {
                base.StartCase(test.Name, test.FullName, x =>
                {
                    var testParams = NUnitHelpers.GenerateTestParametersContext(test.Method, test.Arguments);
                    x.Arguments = test.Arguments?.Select(a => a.ToString()).ToList();
                    if (x.Context.ContainsKey("params"))
                        x.Context["params"] = testParams;
                    else
                        x.Context.Add("params", testParams);

                    AddMetadataToTestCase(test, x);
                });
            }
        }

        private void AddMetadataToTestCase(Test test, CaseResult caseResult)
        {
            ProcessTestCategories(test, caseResult);
            ProcessTestRailAttributes(test, caseResult);
        }
        
        private static void ProcessTestRailAttributes(Test test, CaseResult caseResult)
        {
            var testRailAttributes = test.GetCustomAttributes<CbTestRail>(true);
            if (testRailAttributes.Length == 0)
                return;
            if (!caseResult.Context.ContainsKey("testrail"))
                caseResult.Context.Add("testrail", new Dictionary<string, string>());
            var testRailCtx = caseResult.Context["testrail"] as Dictionary<string, string>;
            // the below condition is not suppose to happen
            if (testRailCtx == null)
                return;
            foreach (var testRailAttribute in testRailAttributes)
            {
                string refTypeAsStr = testRailAttribute.Type.ToString().ToLower();
                testRailCtx[refTypeAsStr] = testRailAttribute.Value;
            }
        }

        public bool EndSuite(TestSuite suite)
        {
            return EndSuite(suite.FullName);
        }

        public void EndCase(Test test)
        {
            var nuTestResult = TestContext.CurrentContext.Result;
            // generate failure object, if relevant
            FailureResult failure = GetFailureFromResult(nuTestResult);
            var status = NUnitHelpers.DetermineTestStatus(nuTestResult.Outcome);
            lock (_lock)
            {
                var fqn = test.FullName;
                CaseResult endedCase;
                // remove ended case from current case per-thread storage
                endedCase = EndCase(fqn, status, failure);

                // Make sure to clear Failure Reason if test has not failed
                // as the user may call SetFailureReason even on passed test
                if (endedCase != null && (endedCase.Status == TestStatusEnum.Passed || endedCase.Status == TestStatusEnum.Skipped))
                {
                    endedCase.FailureReasonId = null;
                }

                if (endedCase != null && endedCase.HasWarnings)
                {
                    endedCase.Status = TestStatusEnum.Warning;
                }

                endedCase.ReRunCount = TestContext.CurrentContext.CurrentRepeatCount;

                WriteCaseResultToFile(endedCase);
            }
        }

        private static FailureResult GetFailureFromResult(TestContext.ResultAdapter result)
		{
            if (!NUnitHelpers.IsFailure(result.Outcome) && !NUnitHelpers.IsError(result.Outcome))
                return null;
            var failure = new FailureResult
            {
                Type = NUnitHelpers.GetFailureType(result),
                Message = result.Message,
                Data = result.StackTrace
            };
            return failure;
        }

		public TResult CaseHook<T, TResult>(string hookName, HookTypeEnum hookType, string methodName, Func<T, TResult> func, T arg)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return base.CaseHook(testFqn, hookName, hookType, func, arg, x => x.MethodName = methodName);
        }
        
        public TResult SuiteHook<T, TResult>(string hookName, HookTypeEnum hookType, string methodName, Func<T, TResult> func, T arg)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return base.SuiteHook(testFqn, hookName, hookType, func, arg, x => x.MethodName = methodName);
        }
        
        public TResult Step<T, TResult>(string name, Func<T, TResult> func, T arg)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return base.Step(testFqn, name, func, arg);
        }
        public TResult StepWithFqn<T, TResult>(string name, string fqn, Func<T, TResult> func, T arg)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return base.Step(testFqn, name, StepTypeEnum.General, func, arg, x => x.Fqn = fqn);
        }
        public Task<TResult> StepWithFqnAsync<TResult>(string name, string fqn, Func<Task<TResult>> func)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return StepAsync(testFqn, name, StepTypeEnum.General, func, x => x.Fqn = fqn);
        }
        public Task StepWithFqnAsync(string name, string fqn, Func<Task> func)
        {
            var testFqn = NUnitHelpers.GetFqn(TestContext.CurrentContext.Test);
            return StepAsync(testFqn, name, StepTypeEnum.General, func, x => x.Fqn = fqn);
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
            return base.StartStep(stepName, type);
        }

        public void HasWarnings(bool hasWarnings = true)
        {
            SetCaseHasWarnings(hasWarnings);
        }

        public void AddLogs(IList<LogMessage> logList)
        {
            CaseResult caseResult = _lastCaseResult.Value;
            if (caseResult == null || logList == null)
            {
                return;
            }

            foreach (var logMsg in logList)
            {
                caseResult.Logs.Add(logMsg);
            }
        }

        // based on https://github.com/nunit/nunit/blob/main/src/NUnitFramework/framework/Internal/StackFilter.cs
        protected override string GetCleanedFullStackTrace(string outerTrace, string innerTrace, bool verbose)
        {
            return NUnitHelpers.GetCleanedFullStackTrace(outerTrace, innerTrace, verbose);
        }
    }
}

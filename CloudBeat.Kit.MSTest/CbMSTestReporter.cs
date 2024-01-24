using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using CbExceptionHelper = CloudBeat.Kit.Common.CbExceptionHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading;

namespace CloudBeat.Kit.MSTest
{
    public class CbMSTestReporter : CbTestReporter
    {
        protected readonly ConcurrentDictionary<string, CaseResult> _startedCasePerThread = new ConcurrentDictionary<string, CaseResult>();
        private static readonly object _lock = new object();

        public CbMSTestReporter(CbConfig config) : base(config)
        {
        }

        private void WriteCaseResultToFile(CaseResult caseResult, Microsoft.VisualStudio.TestTools.UnitTesting.TestResult testResult = null)
        {
			if (caseResult == null)
                return;
            string caseResultFile;
            if (testResult == null)
                caseResultFile = $"{CbGeneralHelpers.FqnToFileName(caseResult.Fqn)}_case_result.json";
            else
            {
				string argsHash = string.Empty;
				if (caseResult.Arguments != null && caseResult.Arguments.Count > 0)
					argsHash = "-" + CbGeneralHelpers.GetHashString(string.Join("'", caseResult.Arguments));
				string randomSuffix = CbGeneralHelpers.GenerateShortUid();				
				caseResultFile = $"{CbGeneralHelpers.FqnToFileName(caseResult.Fqn)}{argsHash}-{randomSuffix}_case_result.json";
			}
			var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
			var cwd = Path.GetDirectoryName(assembly.Location);
			var fullFilePath = Path.Combine(cwd, caseResultFile);
            File.WriteAllText(fullFilePath, JsonConvert.SerializeObject(caseResult));
            if (testResult != null)
            {
				if (testResult.ResultFiles == null)
					testResult.ResultFiles = new List<string>();
				testResult.ResultFiles.Add(fullFilePath);
			}            
        }

        public void StartSuite(TestContext msTestContext)
        {
            if (msTestContext == null)
                throw new ArgumentNullException("msTestContext");
            var suiteFqn = msTestContext.FullyQualifiedTestClassName;
            var suiteName = ExtractClassNameFronFqn(suiteFqn);
            //var categoryAttributes = suite.GetCustomAttributes<CategoryAttribute>(true);
            base.StartSuite(suiteName, suiteFqn, x =>    // NUnitHelpers.GetTestSuiteFqn(
            {
                //x.Arguments = suite.Arguments?.Select(a => a.ToString()).ToArray();
                //AddCategoriesAsTagsAndTestAttributes(x, categoryAttributes);
            });
        }

        private static string ExtractClassNameFronFqn(string suiteFqn)
        {
            if (string.IsNullOrEmpty(suiteFqn))
                return null;
            return suiteFqn.Split('.').Last();
        }

		public void StartCase(string name, string fqn, TestContext testContext = null)
        {
            lock (_lock)
            {
                var suiteFqn = MSTestHelpers.GetSuiteFqnFromCaseFqn(fqn);
				var suiteName = ExtractClassNameFronFqn(suiteFqn);
				// If suite does not exist (on the first class method call)
				// then create the suite first
				if (_result.GetSuite(suiteFqn) == null)
				{
					base.StartSuite(suiteName, suiteFqn);
				}
                base.StartCase(name, fqn, x =>
                {
					var threadId = Thread.CurrentThread.ManagedThreadId.ToString();
					if (!_startedCasePerThread.TryAdd(threadId, x))
						_startedCasePerThread[threadId] = x;
				});
			}
        }

		public void StartCase(ITestMethod testMethod)
        {
            if (testMethod == null)
                throw new ArgumentNullException("testMethod");
            lock (_lock)
            {
                var suiteFqn = testMethod.TestClassName;
                var suiteName = ExtractClassNameFronFqn(suiteFqn);
                // If suite does not exist (on the first class method call)
                // then create the suite first
                if (_result.GetSuite(suiteFqn) == null)
                {
                    base.StartSuite(suiteName, suiteFqn);
                }
                var testName = testMethod.TestMethodName;
                var testFqn = suiteFqn + "." + testName;
                var categoryAttributes = testMethod.GetAttributes<TestCategoryAttribute>(true);
                base.StartCase(testName, testFqn, x =>
                {
                    x.Arguments = testMethod.Arguments?.Select(a => a.ToString()).ToArray();
                    var contextParams = MSTestHelpers.GenerateTestParametersContext(testMethod);
                    if (x.Context.ContainsKey("params"))
                        x.Context["params"] = contextParams;
                    else
                        x.Context.Add("params", contextParams);
                    AddCategoriesAsTagsAndTestAttributes(x, categoryAttributes);
                    // cache last started case per execution thread
                    var threadId = Thread.CurrentThread.ManagedThreadId.ToString();
                    if (!_startedCasePerThread.TryAdd(threadId, x))
                        _startedCasePerThread[threadId] = x;
                });
            }           
        }

        public bool EndSuite(TestContext msTestContext)
        {
            if (msTestContext == null)
                throw new ArgumentNullException("msTestContext");

            var suiteFqn = msTestContext.FullyQualifiedTestClassName;
            return base.EndSuite(suiteFqn);
        }

        public void EndCase(TestContext testContext = null)
        {
			// we prefer to use caches started case object if possible
			// because multiple cases might have the same FQN (parameterized tests)
			CaseResult startedCase = GetStartedCase(testContext), endedCase = null;

			if (startedCase == null) return;
			string suiteFqn = testContext?.FullyQualifiedTestClassName ?? MSTestHelpers.GetSuiteFqnFromCaseFqn(startedCase?.Fqn);
			string caseFqn = startedCase?.Fqn ?? GetCaseFqn(testContext);
			FailureResult failureResult = null;
            TestStatusEnum testStatus = MSTestHelpers.DetermineTestStatus(testContext.CurrentTestOutcome);
			if (startedCase != null)
				endedCase = base.EndCase(startedCase, testStatus, failureResult);
            else
				endedCase = base.EndCase(caseFqn, testStatus, failureResult);
            // Make sure to clear Failure Reason if test has not failed
            // as the user may call SetFailureReason even on passed test
            if (endedCase != null && (testStatus == TestStatusEnum.Passed || testStatus == TestStatusEnum.Skipped))
                endedCase.FailureReasonId = null;
			WriteCaseResultToFile(endedCase);
		}

        public void EndCase(ITestMethod testMethod, Microsoft.VisualStudio.TestTools.UnitTesting.TestResult[] results, TestContext testContext = null)
        {
            if (testMethod == null)
                throw new ArgumentNullException("testMethod");

            FailureResult failureResult = null;
            TestStatusEnum testStatus = TestStatusEnum.Passed;
            var mainTestResult = results.Length > 0 ? results[0] : null;
            foreach (var result in results)
            {
                Console.WriteLine(result.ExecutionId);
                if (result.Outcome == UnitTestOutcome.Passed ||
                    result.Outcome == UnitTestOutcome.InProgress)
                    continue;
                testStatus = MSTestHelpers.DetermineTestStatus(result.Outcome);
                if (result.Outcome == UnitTestOutcome.Failed && result.TestFailureException != null)
                    failureResult = CbExceptionHelper.GetFailureFromException(result.TestFailureException);
                break;
            }            
            // we prefer to use caches started case object if possible
            // because multiple cases might have the same FQN (parameterized tests)
            CaseResult startedCase = GetStartedCase(testContext), endedCase = null;
            string suiteFqn = testContext?.FullyQualifiedTestClassName ?? MSTestHelpers.GetSuiteFqnFromCaseFqn(startedCase?.Fqn);
            string caseFqn = startedCase?.Fqn ?? GetCaseFqn(testContext);
            if (startedCase != null)
                endedCase = base.EndCase(startedCase, testStatus, failureResult);
            else 
                endedCase = base.EndCase(caseFqn, testStatus, failureResult);
            // Make sure to clear Failure Reason if test has not failed
            // as the user may call SetFailureReason even on passed test
            if (endedCase != null && (testStatus == TestStatusEnum.Passed || testStatus == TestStatusEnum.Skipped))
                endedCase.FailureReasonId = null;
            WriteCaseResultToFile(endedCase, mainTestResult);
        }

        public StepResult StartStep(string stepName)
        {
            CaseResult caseResult = GetStartedCase(null);
            if (caseResult == null) return null;
            return base.StartStep(stepName);
            // return base.StartStep(caseResult, stepName);
        }

        /*public StepResult EndStep(StepResult stepResult, TestStatusEnum? status = null)
        {
            CaseResult caseResult = GetStartedCase(null);
            if (caseResult == null) return null;
            return base.EndStep(stepResult, caseResult, status);
        }*/

        public void AddOutputData(string entryName, object entyData, TestContext msTestContext = null)
        {
            CaseResult caseResult = GetStartedCase(msTestContext);
            if (caseResult == null)
                return;
            var dataEntry = new OutputDataEntry(entryName, entyData);
            List<OutputDataEntry> outputDataList = caseResult.Context.ContainsKey("resultData") ? 
                caseResult.Context["resultData"] as List<OutputDataEntry> : new List<OutputDataEntry>();
            outputDataList.Add(dataEntry);
            if (!caseResult.Context.ContainsKey("resultData"))
                caseResult.Context.Add("resultData", outputDataList);
        }

		public void AddTestAttribute(string name, object value, TestContext msTestContext = null)
		{
			CaseResult caseResult = GetStartedCase(msTestContext);
			if (caseResult == null)
				return;
			if (!caseResult.TestAttributes.ContainsKey(name))
				caseResult.TestAttributes.Add(name, value);
            else
                caseResult.TestAttributes[name] = value;
		}

		public void SetFailureReason(FailureReasonEnum reason, TestContext testContext = null)
        {
            CaseResult caseResult = GetStartedCase(testContext);
            if (caseResult == null)
                return;
            caseResult.FailureReasonId = (long)reason;
        }

        public void HasWarnings(bool warnings = true, TestContext testContext = null)
		{
            CaseResult caseResult = GetStartedCase(testContext);
            if (caseResult == null)
                return;
            caseResult.HasWarnings = warnings;
        }


        public TResult Hook<T, TResult>(string hookName, string methodName, Func<T, TResult> func, T arg, TestContext testContext = null)
        {
            var startedCase = GetStartedCase(testContext);
            if (startedCase == null)
                return func.Invoke(arg);
            return Step(startedCase, hookName, StepTypeEnum.Hook, func, arg, x => x.MethodName = methodName);
        }
        public TResult Step<T, TResult>(string name, Func<T, TResult> func, T arg, TestContext testContext = null)
        {
            var startedCase = GetStartedCase(testContext);
            if (startedCase == null)
                return func.Invoke(arg);
            return Step(startedCase, name, StepTypeEnum.General, func, arg);
        }
        public TResult Step<T, TResult>(string stepName, string methodName, Func<T, TResult> func, T arg, TestContext testContext = null)
        {
            var startedCase = GetStartedCase(testContext);
            if (startedCase == null)
                return func.Invoke(arg);
            return Step(startedCase, stepName, StepTypeEnum.General, func, arg, x => x.MethodName = methodName);
        }
        public TResult Step<TResult>(string name, Func<TResult> func, TestContext testContext = null)
        {
            var startedCase = GetStartedCase(testContext);
            if (startedCase == null)
                return func.Invoke();
            return Step(startedCase, name, StepTypeEnum.General, func);
        }
        public StepResult Step(string name, Action action, TestContext testContext = null)
        {
            var startedCase = GetStartedCase(testContext);
            if (startedCase == null)
			{
                action();
                return null;
            }
                
            return base.Step(startedCase, name, action);
        }

        public StepResult Transaction(string name, Action action, TestContext testContext = null)
        {
            var startedCase = GetStartedCase(testContext);
            if (startedCase == null)
            {
                action();
                return null;
            }

            return base.Step(startedCase, name, StepTypeEnum.Transaction, action);
        }

        private string GetCaseFqn(TestContext testContext)
		{
            if (testContext == null) return null;
            var suiteFqn = testContext.FullyQualifiedTestClassName;
            return $"{suiteFqn}.{testContext.TestName}";
        }
        private CaseResult GetStartedCase(TestContext testContext)
        {
            CaseResult caseResult = null;
            var threadId = Thread.CurrentThread.ManagedThreadId.ToString();
            _startedCasePerThread.TryGetValue(threadId, out caseResult);
            if (caseResult == null && testContext != null)
            {
                var suiteFqn = testContext.FullyQualifiedTestClassName;
                var caseFqn = $"{suiteFqn}.{testContext.TestName}";
                var suiteResult = _result.GetSuite(suiteFqn);
                caseResult = suiteResult?.GetCaseByFqn(caseFqn);
            }
            return caseResult;
        }

        private static void AddCategoriesAsTagsAndTestAttributes(CaseResult caseResult, TestCategoryAttribute[] categoryAttributes)
        {
            foreach (var categoryAttr in categoryAttributes)
            {
                if (categoryAttr.TestCategories == null || categoryAttr.TestCategories.Count == 0)
                    continue;
                foreach (var testCategoryName in categoryAttr.TestCategories)
                {
                    if (testCategoryName.Contains(":"))
                    {
                        var keyVal = testCategoryName.Split(':');
                        if (keyVal.Length < 2) continue;
                        if (caseResult.TestAttributes.ContainsKey(keyVal[0])) continue;
                        caseResult.TestAttributes.Add(keyVal[0], keyVal[1]);
                    }
                }
            }
        }
    }
}

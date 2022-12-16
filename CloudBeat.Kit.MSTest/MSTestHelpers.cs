using System;
using System.Collections.Generic;
using CloudBeat.Kit.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CloudBeat.Kit.MSTest
{
	public static class MSTestHelpers
	{
		private const string DEFAULT_SELENIUM_URL = "http://localhost:4437/wd/hub";

		private static readonly Dictionary<UnitTestOutcome, TestStatusEnum> TEST_OUTCOME_RESULT_STATUS_MAP =
            new Dictionary<UnitTestOutcome, TestStatusEnum>()
            {
                { UnitTestOutcome.Aborted, TestStatusEnum.Skipped },
                { UnitTestOutcome.Passed, TestStatusEnum.Passed },
                { UnitTestOutcome.NotRunnable, TestStatusEnum.Skipped },
                { UnitTestOutcome.Failed, TestStatusEnum.Failed },
                { UnitTestOutcome.Error, TestStatusEnum.Failed },
                { UnitTestOutcome.Timeout, TestStatusEnum.Broken },
                { UnitTestOutcome.Inconclusive, TestStatusEnum.Broken },
                { UnitTestOutcome.Unknown, TestStatusEnum.Failed }
            };

        public static TestStatusEnum DetermineTestStatus(UnitTestOutcome outcome)
        {
            return TEST_OUTCOME_RESULT_STATUS_MAP.ContainsKey(outcome) ?
                TEST_OUTCOME_RESULT_STATUS_MAP[outcome] : TestStatusEnum.Passed;
        }

        internal static bool IsFailure(UnitTestOutcome outcome)
        {
            return outcome == UnitTestOutcome.Failed
                || outcome == UnitTestOutcome.Timeout
                || outcome == UnitTestOutcome.Unknown;
        }

        internal static bool IsError(UnitTestOutcome outcome)
        {
            return outcome == UnitTestOutcome.Error;
        }

        internal static bool IsSkipped(UnitTestOutcome outcome)
        {
            return outcome == UnitTestOutcome.Aborted
                || outcome == UnitTestOutcome.NotRunnable;
        }

        internal static Dictionary<string, object> GenerateTestParametersContext(ITestMethod testMethod)
        {            
            object[] arguments = testMethod?.Arguments;
            if (testMethod == null || arguments == null)
                return null;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            var paramInfoList = testMethod.MethodInfo.GetParameters();
            if (paramInfoList.Length == 0 || arguments.Length != paramInfoList.Length)
                return null;
            for (var i = 0; i < paramInfoList.Length; i++)
            {
                var paramInfo = paramInfoList[i];
                var name = paramInfo.Name;
                var value = arguments[i];
                parameters.Add(name, value);
            }
            return parameters;
        }

        internal static string GetSuiteFqnFromCaseFqn(string caseFqn)
		{
            if (string.IsNullOrEmpty(caseFqn))
                return null;
            var fqnElms = caseFqn.Split('.');
            return string.Join(".", fqnElms.Take(fqnElms.Length - 1));

        }
		internal static Uri GetSeleniumUrl(TestContext testContext) => new Uri(testContext.Properties["SeleniumUrl"]?.ToString() ?? DEFAULT_SELENIUM_URL);		
	}
}


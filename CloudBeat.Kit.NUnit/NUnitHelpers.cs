using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Models;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static NUnit.Framework.TestContext;

namespace CloudBeat.Kit.NUnit
{
    internal static class NUnitHelpers
    {
		private static readonly Regex exceptionTypeRegex = new Regex(@"^[a-zA-Z.]+\.([a-zA-Z]+) *:", RegexOptions.Compiled | RegexOptions.Multiline);

		private static readonly Dictionary<ResultState, TestStatusEnum> TEST_OUTCOME_RESULT_STATUS_MAP =
            new Dictionary<ResultState, TestStatusEnum>()
            {
                { ResultState.Failure, TestStatusEnum.Failed },
                { ResultState.Success, TestStatusEnum.Passed },
                { ResultState.Skipped, TestStatusEnum.Skipped },
                { ResultState.Cancelled, TestStatusEnum.Skipped },
                { ResultState.ChildFailure, TestStatusEnum.Failed },
                { ResultState.ChildIgnored, TestStatusEnum.Passed },
                { ResultState.ChildWarning, TestStatusEnum.Passed },
                { ResultState.Error, TestStatusEnum.Failed },
                { ResultState.Ignored, TestStatusEnum.Skipped },
                { ResultState.SetUpError, TestStatusEnum.Failed },
                { ResultState.SetUpFailure, TestStatusEnum.Failed },
                { ResultState.TearDownError, TestStatusEnum.Failed },
                { ResultState.Warning, TestStatusEnum.Warning },
                { ResultState.Inconclusive, TestStatusEnum.Broken },
                { ResultState.NotRunnable, TestStatusEnum.Skipped }
                
            };

		private static readonly Dictionary<string, string> EXCEPTION_FAILURE_TYPE_MAP =
			new Dictionary<string, string>()
			{
				{ "HttpRequestException", CbExceptionHelper.ERROR_TYPE_HTTP }
			};

		public static IEnumerable<string> GetTestProperties(ITest test, string propertyName)
        {
            var list = new List<string>();
            var currentTest = test;
            while (currentTest.GetType() != typeof(TestSuite) && currentTest.GetType() != typeof(TestAssembly))
            {
                if (currentTest.Properties.ContainsKey(propertyName))
                    if (currentTest.Properties[propertyName].Count > 0)
                        for (var i = 0; i < currentTest.Properties[propertyName].Count; i++)
                            list.Add(currentTest.Properties[propertyName][i].ToString());

                currentTest = currentTest.Parent;
            }

            return list;
        }

        public static TestStatusEnum DetermineTestStatus(ResultState outcome)
        {
            return TEST_OUTCOME_RESULT_STATUS_MAP.ContainsKey(outcome) ?
                TEST_OUTCOME_RESULT_STATUS_MAP[outcome] : TestStatusEnum.Passed;
        }

        internal static bool IsFailure(ResultState outcome)
        {
            return outcome == ResultState.Failure
                || outcome == ResultState.SetUpFailure
                || outcome == ResultState.ChildFailure;
        }
        internal static bool IsError(ResultState outcome)
        {
            return outcome == ResultState.Error
                || outcome == ResultState.SetUpError
                || outcome == ResultState.TearDownError;
        }
        internal static bool IsSkipped(ResultState outcome)
        {
            return outcome == ResultState.Skipped
                || outcome == ResultState.Ignored
                || outcome == ResultState.NotRunnable;
        }

		internal static Dictionary<string, object> GenerateTestParametersContext(IMethodInfo method, object[] arguments)
		{
            if (method == null || arguments == null)
                return null;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            var paramInfoList = method.GetParameters();
            if (paramInfoList.Length == 0 || arguments.Length != paramInfoList.Length)
                return null;
            for (var i=0; i<paramInfoList.Length; i++)
			{
                var paramInfo = paramInfoList[i];
                var name = paramInfo.ParameterInfo.Name;
                var value = arguments[i];
                parameters.Add(name, value);
            }
            return parameters;
		}

        internal static string GetFqn(TestAdapter test)
        {
            //return Regex.Replace(test.FullName, "(.*)(\\(.*\\))(\\..*)", "$1$3");
            return test.FullName;
        }

		internal static string GetFailureType(ResultAdapter result)
		{
            if (string.IsNullOrEmpty(result.Message))
				return CbExceptionHelper.ERROR_TYPE_GENERAL;
			// check if this is assertion related error
			bool isAssertionFailure = result.Assertions.Any(x => x.Message == result.Message && x.StackTrace == result.StackTrace);
			if (isAssertionFailure)
				return CbExceptionHelper.ERROR_TYPE_ASSERT;
            if (result.Message.StartsWith("Multiple failures or warnings in test"))
				return CbExceptionHelper.ERROR_TYPE_ASSERT;
            else if (!string.IsNullOrEmpty(result.StackTrace) && result.StackTrace.Contains("at FluentAssertions"))
				return CbExceptionHelper.ERROR_TYPE_ASSERT;
			var match = exceptionTypeRegex.Match(result.Message);
			var exceptionName = match.Success ? match.Groups[1].Value : null;
            if (string.IsNullOrEmpty(exceptionName) || !EXCEPTION_FAILURE_TYPE_MAP.ContainsKey(exceptionName))
                return CbExceptionHelper.ERROR_TYPE_GENERAL;
			// failure.Location = GetLocationByStackTrace(result.StackTrace);
			return EXCEPTION_FAILURE_TYPE_MAP[exceptionName];
		}
	}
}

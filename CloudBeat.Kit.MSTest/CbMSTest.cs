using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Models;
using CloudBeat.Kit.Common.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TestResult = Microsoft.VisualStudio.TestTools.UnitTesting.TestResult;

namespace CloudBeat.Kit.MSTest
{
    public static class CbMSTest
    {
        private const string TEST_DATA_PARAM_NAME = "testData";
        public const string FRAMEWORK_NAME = "MSTest";
        public const string FRAMEWORK_VERSION = "2";

        private static CbMSTestContext currentCbContext;

        static CbMSTest()
        {
            //Debugger.Launch();
        }
        public static CbMSTestContext Current
        {
            get
            {
                if (currentCbContext == null)
				{
                    currentCbContext = CreateCloudBeatMSTestContext();
                    currentCbContext.Reporter.StartInstance();
                    currentCbContext.Reporter.SetFramework(FRAMEWORK_NAME, FRAMEWORK_VERSION);
                }
                return currentCbContext;
            }
        }

        public static Dictionary<string, object> GetCapabilities()
		{
            TestContext msTestContext = Current.MSTestContext;
            if (!Current.IsConfigured || msTestContext == null)
                return null;
            Dictionary<string, object> caps = new Dictionary<string, object>();
            var deviceName = msTestContext.Properties["deviceName"]?.ToString();            
            var platformName = msTestContext.Properties["platformName"]?.ToString();
            var browserName = msTestContext.Properties["browserName"]?.ToString();
            var platformVersion = msTestContext.Properties["platformVersion"]?.ToString();
            var udid = msTestContext.Properties["udid"]?.ToString();
            if (!string.IsNullOrEmpty(deviceName)) caps.Add("deviceName", deviceName);
            if (!string.IsNullOrEmpty(platformName)) caps.Add("platformName", platformName);
            if (!string.IsNullOrEmpty(browserName)) caps.Add("browserName", browserName);
            if (!string.IsNullOrEmpty(platformVersion)) caps.Add("platformVersion", platformVersion);
            if (!string.IsNullOrEmpty(udid)) caps.Add("udid", udid);

            return caps;
        }

        public static TResult Hook<T, TResult>(string hookName, string methodName, Func<T, TResult> func, T arg, TestContext testContext = null)
        {
            if (!Current.IsConfigured)
                return func.Invoke(arg);
            if (testContext == null)
                testContext = Current.MSTestContext;
            return Current.Reporter.Step(hookName, methodName, func, arg);
        }
        public static TResult Step<T, TResult>(string name, Func<T, TResult> func, T arg, TestContext testContext = null)
        {
            if (!Current.IsConfigured)
                return func.Invoke(arg);
            if (testContext == null)
                testContext = Current.MSTestContext;
            return Current.Reporter.Step(name, func, arg);
        }
        public static TResult Step<TResult>(string name, Func<TResult> func, TestContext testContext = null)
        {
            if (!Current.IsConfigured)
                return func.Invoke();
            if (testContext == null)
                testContext = Current.MSTestContext;
            return Current.Reporter.Step(name, func);
        }
        public static void Step(string name, Action action, TestContext testContext = null)
        {
            if (!Current.IsConfigured)
			{
                action();
                return;
            }

            if (testContext == null)
                testContext = Current.MSTestContext;
            Current.Reporter.Step(name, action);
        }

        public static void Transaction(string name, Action action, TestContext testContext = null)
        {
            if (!Current.IsConfigured)
            {
                action();
                return;
            }

            if (testContext == null)
                testContext = Current.MSTestContext;
            Current.Reporter.Transaction(name, action);
        }

        public static void WrapWebDriver(EventFiringWebDriver driver, bool takeFullPageScreenshots = true)
        {
            if (!Current.IsConfigured)
                return;
            new CbWebDriverWrapper(driver, Current.Reporter, new CbWebDriverWrapper.Options { FullPageScreenshot = takeFullPageScreenshots });
            //Current.Reporter.WrapWebDriver(driver);
        }

        public static IEnumerable<object[]> GetTestData()
		{
            TestContext msTestContext = Current.MSTestContext;
            if (!Current.IsConfigured || msTestContext == null)
                return Enumerable.Empty<object[]>();
            
            if (!msTestContext.Properties.Contains(TEST_DATA_PARAM_NAME))
                return Enumerable.Empty<object[]>();

            var csvDataAsString = msTestContext.Properties[TEST_DATA_PARAM_NAME] as string;
            if (string.IsNullOrEmpty(csvDataAsString))
                return Enumerable.Empty<object[]>();
            // decode csv data value, as it's escaped using XML
            csvDataAsString = WebUtility.HtmlDecode(csvDataAsString);
            return CbConfig.ParseCsvStringAsObjectArray(csvDataAsString);
        }

        public static void SetMSTestContext(TestContext context)
        {
            if (Current.IsConfigured)
                Current.MSTestContext = context;
        }

        private static CbMSTestContext CreateCloudBeatMSTestContext()
        {
            CbConfig config = new CbConfig();
            config.loadFromEnvironment();
            return new CbMSTestContext(config);
        }

        internal static void StartCase(ITestMethod testMethod)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter.StartCase(testMethod);
        }

		internal static void StartCase(string name, string fqn)
		{
			if (!Current.IsConfigured)
				return;
            Current.Reporter.StartCase(name, fqn, Current.MSTestContext);
		}

		internal static void EndCase(ITestMethod testMethod, TestResult[] results)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter.EndCase(testMethod, results, Current.MSTestContext);
        }

		internal static void EndCase()
		{
			if (!Current.IsConfigured)
                return;
			Current.Reporter.EndCase(Current.MSTestContext);
		}

		public static void AddOutputData(string name, object data, TestContext testContext = null)
		{
            if (!Current.IsConfigured)
                return;
            if (testContext == null)
                testContext = Current.MSTestContext;
            Current.Reporter.AddOutputData(name, data, testContext);
        }

        public static void SetFailureReason(FailureReasonEnum reason, TestContext testContext = null)
        {
            if (!Current.IsConfigured)
                return;
            if (testContext == null)
                testContext = Current.MSTestContext;
            Current.Reporter.SetFailureReason(reason, testContext);
        }

        public static void HasWarnings(bool warnings = true, TestContext testContext = null)
        {
            if (!Current.IsConfigured)
                return;
            if (testContext == null)
                testContext = Current.MSTestContext;
            Current.Reporter.HasWarnings(warnings, testContext);
        }		
	}
}

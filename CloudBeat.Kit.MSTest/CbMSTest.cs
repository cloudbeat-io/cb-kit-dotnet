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
        private const string FRAMEWORK_NAME = "MSTest";
        private const string FRAMEWORK_VERSION = "2";

        private static CbMSTestContext currentCbContext;

        static CbMSTest()
        {
            //Debugger.Launch();
        }

        /// <summary>
        /// Retrieves a boolean value indicating whether the code is running from CloudBeat.
        /// </summary>
        /// <returns><c>true</c> if running from CloudBeat; <c>false</c> otherwise.</returns>
        public static bool IsRunningFromCB()
        {
            return Current.IsConfigured;
        }

        /// <summary>
        /// Retrieves current MSTestContext. 
        /// </summary>
        public static CbMSTestContext Current
        {
            get
            {
                if (currentCbContext == null)
				{
                    CbConfig config = new CbConfig();
                    config.loadFromEnvironment();
                    currentCbContext = new CbMSTestContext(config);

                    if (currentCbContext.Reporter != null)
                    {
                        currentCbContext.Reporter.StartInstance();
                        currentCbContext.Reporter.SetFramework(FRAMEWORK_NAME, FRAMEWORK_VERSION);
                    }
                }
                return currentCbContext;
            }
        }

        // Retrieves copy of TesRunParameters. This method is intended for debugging purposes only.
        [Obsolete("Intended for debugging purposes only. Do not use in production code.")]
        public static Dictionary<string, object> GetTesRunParameters()
		{
            if (!Current.IsConfigured || Current.MSTestContext == null)
                return null;

            var props = Current.MSTestContext.Properties as Dictionary<string, object>;
            return props.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        /// <summary>
        /// Retrieves environment name.
        /// </summary>
        /// <returns>Environment name or <c>defaultName</c> if no environment was selected during test execution.</returns>
        public static string GetEnvironmentName(string defaultName = null)
		{
			TestContext msTestContext = Current.MSTestContext;
			if (!Current.IsConfigured || msTestContext == null)
				return defaultName;
			return msTestContext.Properties["environmentName"]?.ToString() ?? defaultName;
		}

        /// <summary>
        /// Retrieves environment variable.
        /// </summary>
        /// <returns>Environment value. null if no environment was selected during test execution or the specified variable is not defined.</returns>
        public static string GetEnvironmentValue(string name)
        {
            TestContext msTestContext = Current.MSTestContext;
            if (!Current.IsConfigured || msTestContext == null || name == null)
                return null;
            return msTestContext.Properties[name]?.ToString();
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

        /// <summary>
        /// Adds CloudBeat event handlers to the provided Web Driver.
        /// </summary>
        /// <param name="driver">EventFiringWebDriver to wrap.</param>
        /// <param name="takeFullPageScreenshots">Take full page screenshots. Works only when using ChromeDriver.</param>
        /// <param name="ignoreFindElement">Ignore exceptions produced by IWebDriver.FindElement(s)</param>
        public static void WrapWebDriver(EventFiringWebDriver driver, bool takeFullPageScreenshots = true, bool ignoreFindElement = true)
        {
            if (!Current.IsConfigured)
                return;

			Current.Reporter?.SetCurrentWebDriver(driver?.WrappedDriver);

			new CbWebDriverWrapper(driver, Current.Reporter, 
                new CbWebDriverWrapper.Options { 
                    FullPageScreenshot = takeFullPageScreenshots,
					IgnoreFindElement = ignoreFindElement
				});

            Current.Reporter?.SetScreenshotProvider(new CbMSTestScreenshotProvider(driver?.WrappedDriver, takeFullPageScreenshots));
        }

        /// <summary>
        /// Retrieves parameters passed from CloudBeat.
        /// </summary>
        /// <returns>Enumerable containing parameter rows. Parameter header is not returned.</returns>
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

        internal static void SetMSTestContext(TestContext context)
        {
            if (Current.IsConfigured)
                Current.MSTestContext = context;
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

        /// <summary>
        /// Adds name/value data pair to the test result.
        /// </summary>
        /// <param name="name">Data name.</param>
        /// <param name="data">Data value.</param>
        /// <param name="testContext">Optional TestContext.</param>
		public static void AddOutputData(string name, object data, TestContext testContext = null)
		{
            if (!Current.IsConfigured)
                return;
            if (testContext == null)
                testContext = Current.MSTestContext;
            Current.Reporter.AddOutputData(name, data, testContext);
        }

        /// <summary>
        /// Adds name/value test attribute pair to the test result.
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="value">Attribute value</param>
        /// <param name="testContext">Optional TestContext.</param>
		public static void AddTestAttribute(string name, object value, TestContext testContext = null)
		{
			if (!Current.IsConfigured)
				return;
			if (testContext == null)
				testContext = Current.MSTestContext;
			Current.Reporter.AddTestAttribute(name, value, testContext);
		}

        /// <summary>
        /// Sets failure reason.
        /// Could be used from cleanup methods or catch blocks to set reason for the test failure.
        /// </summary>
        /// <param name="reason">Failure reason.</param>
        /// <param name="testContext">Optional TestContext.</param>
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

        /// <summary>
        /// Takes screenshot (only if current test has failed) and adds it to last failed step if it doesn't have any screenshot. 
        /// If there is no last failed step then screenshot is added as attachment to the test result.
        /// This method is intended to be used from TestCleanup methods for taking screenshots for exceptions happening outside of "steps".
        /// </summary>
        public static void AddScreenshotOnError()
        {
            if (!Current.IsConfigured)
            {
                return;
            }

            if (Current.MSTestContext.CurrentTestOutcome == UnitTestOutcome.Failed ||
                Current.MSTestContext.CurrentTestOutcome == UnitTestOutcome.Error)
            {
                try
                {
                    var screenshot = Current.Reporter?.GetScreenshotProvider().TakeScreenshot();
                    Current.Reporter.AddScreenshot(screenshot);
                }
                catch
                {
                    // ignored
                }
            }
        }
	}
}

using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Models;
using CloudBeat.Kit.Common.Wrappers;
using NUnit.Framework;
using OpenQA.Selenium.Support.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace CloudBeat.Kit.NUnit
{
    public static class CbNUnit
    {
        private const string TEST_DATA_PARAM_NAME = "testData";

        private static CbNUnitContext currentCbContext;

        static CbNUnit()
        {
            Thread.AllocateNamedDataSlot("_cbContext");
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
        /// Retrieves current CbNUnitContext. 
        /// </summary>
        public static CbNUnitContext Current
        {
            get
            {
                if (currentCbContext == null)
                {
                    CbConfig config = new CbConfig();
                    config.loadFromEnvironment();
                    currentCbContext = new CbNUnitContext(config);
                }
                return currentCbContext;
            }
        }

        /// <summary>
        /// Retrieves capabilities.
        /// </summary>
        /// <returns><Dictionary containing the capabilities./returns>
        public static Dictionary<string, object> GetCapabilities()
        {
            Dictionary<string, object> caps = new Dictionary<string, object>();

            foreach (var capName in CapabilitiesList.Capabilities)
            {
                var cap = TestContext.Parameters[capName]?.ToString();
                if (!string.IsNullOrEmpty(cap))
                {
                    caps.Add(capName, cap);
                }
            }
            return caps;
        }

        // Retrieves copy of TesRunParameters. This method is intended for debugging purposes only.
        [Obsolete("Intended for debugging purposes only. Do not use in production code.")]
        public static Dictionary<string, object> GetTesRunParameters()
        {
            if (!Current.IsConfigured)
                return null;

            var dict = new Dictionary<string, object>(TestContext.Parameters.Count);
            foreach (var param in TestContext.Parameters.Names)
            {
                dict.Add(param, TestContext.Parameters[param]);
            }

            return dict;
        }

        /// <summary>
        /// Retrieves environment name.
        /// </summary>
        /// <returns>Environment name or <c>defaultName</c> if no environment was selected during test execution.</returns>
        public static string GetEnvironmentName(string defaultName = null)
        {
			return TestContext.Parameters["environmentName"]?.ToString() ?? defaultName;
		}

        /// <summary>
        /// Starts new step.
        /// </summary>
        /// <param name="name">Step name.</param>
		public static void StartStep(string name)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter.StartStep(name);
        }

        /// <summary>
        /// Closes currently active step.
        /// </summary>
        /// <param name="name">Step name.</param>
        public static void EndStep(string name)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter.EndStep(name);
        }

        /// <summary>
        /// Encapsulates a code block inside step.
        /// </summary>
        /// <param name="name">Step name.</param>
        /// <param name="action">Delegate to execute.</param>
        public static void Step(string name, Action action)
        {
            if (!Current.IsConfigured)
                action.Invoke();
            else
                Current.Reporter.Step(name, action);
        }

        /// <summary>
        /// Encapsulates a code block inside step.
        /// </summary>
        /// <typeparam name="TResult">The type returned from this method.</typeparam>
        /// <param name="name">Step name.</param>
        /// <param name="func">Delegate to execute.</param>
        /// <returns>Delegate's return value.</returns>
        public static TResult Step<TResult>(string name, Func<TResult> func)
        {
            if (!Current.IsConfigured)
                return func.Invoke();
            else
                return Current.Reporter.Step(name, func);
        }

        /// <summary>
        /// Encapsulates a code block inside transaction.
        /// </summary>
        /// <param name="name">Transaction name.</param>
        /// <param name="action">Delegate to execute</param>
        public static void Transaction(string name, Action action)
        {
            if (!Current.IsConfigured)
                action.Invoke();
            else
            {
                var step = Current.Reporter.Step(name, action);
                if (step != null)
                    step.Type = StepTypeEnum.Transaction;
            }
        }

        /// <summary>
        /// Retrieves parameters passed from CloudBeat.
        /// </summary>
        /// <returns>Enumerable containing parameter rows. Parameter header is not returned.</returns>
        public static IEnumerable<object[]> GetTestData()
		{
            if (!Current.IsConfigured || !TestContext.Parameters.Exists(TEST_DATA_PARAM_NAME))
                return Enumerable.Empty<object[]>();
            var csvDataAsString = TestContext.Parameters.Get(TEST_DATA_PARAM_NAME);
            if (string.IsNullOrEmpty(csvDataAsString))
                return Enumerable.Empty<object[]>();
            // decode csv data value, as it's escaped using XML
            csvDataAsString = WebUtility.HtmlDecode(csvDataAsString);
            return CbConfig.ParseCsvStringAsObjectArray(csvDataAsString);
        }

        /// <summary>
        /// Retrieves environment variable.
        /// </summary>
        /// <returns>Environment value. null if no environment was selected during test execution or the specified variable is not defined.</returns>
        public static string GetEnvironmentValue(string name)
        {
            if (!Current.IsConfigured || !TestContext.Parameters.Exists(name))
                return null;
            return TestContext.Parameters.Get(name);
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

            new CbWebDriverWrapper(
                driver,
                Current.Reporter,
                new CbWebDriverWrapper.Options { FullPageScreenshot = takeFullPageScreenshots, IgnoreFindElement = ignoreFindElement });

            Current.Reporter?.SetScreenshotProvider(new CbNUnitScreenshotProvider(driver?.WrappedDriver, takeFullPageScreenshots));
        }

        /// <summary>
        /// Adds name/value data pair to the test result.
        /// </summary>
        /// <param name="name">Data name.</param>
        /// <param name="data">Data value.</param>
        public static void AddOutputData(string name, object data)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.AddOutputData(name, data);
        }

        /// <summary>
        /// Adds name/value test attribute pair to the test result.
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="value">Attribute value</param>
        public static void AddTestAttribute(string name, object value)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.AddTestAttribute(name, value);
        }

        /// <summary>
        /// Sets failure reason.
        /// Could be used from cleanup methods or catch blocks to set reason for the test failure.
        /// </summary>
        /// <param name="reason">Failure reason.</param>
        public static void SetFailureReason(FailureReasonEnum reason)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.SetCaseFailureReason(reason);
        }

        /// <summary>
        /// Takes screenshot (only if current test has failed) and adds it to last failed step if it doesn't have any screenshot. 
        /// If there is no last failed step then screenshot is added as attachment to the test result.
        /// This method is intended to be used from TearDown methods for taking screenshots for exceptions happening outside of "steps".
        /// </summary>
        public static void AddScreenshotOnError()
        {
            if (!Current.IsConfigured)
            {
                return;
            }

            if (TestContext.CurrentContext.Result.FailCount > 0)
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

        public static void AddScreenRecording(string videoBase64Data)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.AddScreenRecordingAttachment(videoBase64Data);
        }
        
        public static bool AddScreenRecordingFromUrl(string url)
        {
            if (!Current.IsConfigured)
                return false;
            return Current.Reporter.AddScreenRecordingAttachmentFromUrl(url);
        }

        public static void AddScreenRecordingFromPath(string videoFilePath)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.AddScreenRecordingAttachmentFromPath(videoFilePath);
        }

        public static void HasWarnings(bool hasWarnings = true)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.HasWarnings(hasWarnings);
        }
    }
}

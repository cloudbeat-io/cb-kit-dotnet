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
        private const string CB_ENVIRONMENT_PARAM_NAME = "environment";

        private static CbNUnitContext currentCbContext;

        static CbNUnit()
        {
            Thread.AllocateNamedDataSlot("_cbContext");
        }
        public static CbNUnitContext Current
        {
            get
            {
                if (currentCbContext == null)
                {
                    currentCbContext = CreateCloudBeatNUnitContext();
                }
                return currentCbContext;
            }
        }

        public static Dictionary<string, object> GetCapabilities()
        {
            Dictionary<string, object> caps = new Dictionary<string, object>();
            var deviceName = TestContext.Parameters["deviceName"]?.ToString();
            var platformName = TestContext.Parameters["platformName"]?.ToString();
            var browserName = TestContext.Parameters["browserName"]?.ToString();
            var platformVersion = TestContext.Parameters["platformVersion"]?.ToString();
            var udid = TestContext.Parameters["udid"]?.ToString();
            if (!string.IsNullOrEmpty(deviceName)) caps.Add("deviceName", deviceName);
            if (!string.IsNullOrEmpty(platformName)) caps.Add("platformName", platformName);
            if (!string.IsNullOrEmpty(browserName)) caps.Add("browserName", browserName);
            if (!string.IsNullOrEmpty(platformVersion)) caps.Add("platformVersion", platformVersion);
            if (!string.IsNullOrEmpty(udid)) caps.Add("udid", udid);

            return caps;
        }

        public static string GetEnvironmentName(string defaultName = null)
        {
			return TestContext.Parameters["environmentName"]?.ToString() ?? defaultName;
		}

        public static void StartStep(string name)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter.StartStep(name);
        }

        public static void EndStep(string name)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter.EndStep(name);
        }

        public static void Step(string name, Action action)
		{
            if (!Current.IsConfigured)
                action.Invoke();
            else
                Current.Reporter.Step(name, action);
		}

        public static T Step<T>(string name, Func<T> func)
        {
            if (!Current.IsConfigured)
                return func.Invoke();
            else
                return Current.Reporter.Step(name, func);
        }

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

        public static string GetParameter(string name)
        {
            if (!Current.IsConfigured || !TestContext.Parameters.Exists(name))
                return null;
            return TestContext.Parameters.Get(name);
        }

        public static void WrapWebDriver(EventFiringWebDriver driver, bool takeFullPageScreenshots = true, bool ignoreFindElement = true)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.SetCurrentWebDriver(driver?.WrappedDriver);
            new CbWebDriverWrapper(
                driver,
                Current.Reporter,
                new CbWebDriverWrapper.Options { FullPageScreenshot = takeFullPageScreenshots, IgnoreFindElement = ignoreFindElement });
        }

        public static void AddOutputData(string name, object data)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.AddOutputData(name, data);
        }

        public static void AddTestAttribute(string name, object value)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.AddTestAttribute(name, value);
        }

        public static void SetFailureReason(FailureReasonEnum reason)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.SetCaseFailureReason(reason);
        }

		public static void AddScreenshot(string base64Data, bool toLastFailedStep = true)
		{
			if (!Current.IsConfigured || Current.Reporter == null)
				return;
			if (toLastFailedStep)
				if (Current.Reporter.AddScreenshotToLastFailedStep(base64Data))
					return;
			Current.Reporter?.AddScreenshotAttachment(base64Data);
		}

		public static void AddScreenRecording(string videoBase64Data)
        {
            if (!Current.IsConfigured)
                return;
            Current.Reporter?.AddScreenRecordingAttachment(videoBase64Data);
        }
        
        public static bool AddScreenRecordingFromUrl(string url)
        {
            if (!Current.IsConfigured || Current.Reporter == null)
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

        private static CbNUnitContext CreateCloudBeatNUnitContext()
        {
            CbConfig config = new CbConfig();
            config.loadFromEnvironment();
            return new CbNUnitContext(config);
        }
    }
}

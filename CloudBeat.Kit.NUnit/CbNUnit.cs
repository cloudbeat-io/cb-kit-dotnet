using CloudBeat.Kit.Common;
using NUnit.Framework;
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

        public static void Step(string name, Action action)
		{
            if (!Current.IsConfigured)
                action.Invoke();
            else
                Current.Reporter.Step(name, action);
		}

        public static void Transaction(string name, Action action)
        {
            if (!Current.IsConfigured)
                action.Invoke();
            else
			{
                var step = Current.Reporter.Step(name, action);
                if (step != null)
                    step.Type = Common.Models.StepTypeEnum.Transaction;
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

        private static CbNUnitContext CreateCloudBeatNUnitContext()
        {
            CbConfig config = new CbConfig();
            config.loadFromEnvironment();
            return new CbNUnitContext(config);
        }
    }
}

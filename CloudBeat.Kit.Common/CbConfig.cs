using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudBeat.Kit.Common
{
    public class CbConfig
	{
        public const string CB_API_KEY = "CB_API_KEY";
        public const string CB_API_URL = "CB_API_URL";
        public const string CB_TEST_MONITOR_URL = "CB_TEST_MONITOR_URL";
        public const string CB_TEST_MONITOR_TOKEN = "CB_TEST_MONITOR_TOKEN";
        public const string CB_PROJECT_ID = "CB_PROJECT_ID";
        public const string CB_RUN_ID = "CB_RUN_ID";
        public const string CB_AGENT = "CB_AGENT";
        public const string CB_DEBUG = "CB_DEBUG";
        public const string CB_INSTANCE_ID = "CB_INSTANCE_ID";
        public const string CB_CAPS_PREFIX = "CB_CAPS.";
        public const string CB_META_PREFIX = "CB_META.";
        public const string CB_ENV_PREFIX = "CB_ENV.";
        public const string CB_OPT_PREFIX = "CB_OPT.";

        string _runId;
        string _instanceId;
        string _projectId;
        string _apiKey;
        string _apiUrl;
        bool _isCbAgent;
        private bool _debugMode;

        public void LoadFromEnvironment()
        {
            _apiKey = Environment.GetEnvironmentVariable(CB_API_KEY)
                      ?? Environment.GetEnvironmentVariable(CB_TEST_MONITOR_TOKEN);
            _apiUrl = Environment.GetEnvironmentVariable(CB_API_URL)
                      ?? Environment.GetEnvironmentVariable(CB_TEST_MONITOR_URL);
            _runId = Environment.GetEnvironmentVariable(CB_RUN_ID);
            _instanceId = Environment.GetEnvironmentVariable(CB_INSTANCE_ID);
            _projectId = Environment.GetEnvironmentVariable(CB_PROJECT_ID);
            _isCbAgent = ParseBool(Environment.GetEnvironmentVariable(CB_AGENT));
            _debugMode = ParseBool(Environment.GetEnvironmentVariable(CB_DEBUG));
        }

        private static bool ParseBool(string str)
        {
            bool.TryParse(str, out bool result);
            return result;
        }

        public bool HasMandatory()
		{
            return _isCbAgent || _debugMode;
        }

        public bool DebugMode => _debugMode;

        public string RunId => _runId;
        public string InstanceId => _instanceId;
        public string ProjectId => _projectId;
        public string ApiKey => _apiKey;
        public string ApiUrl => _apiUrl;

        public static IEnumerable<object[]> ParseCsvStringAsObjectArray(string csvStr)
		{
            if (string.IsNullOrEmpty(csvStr))
                return Enumerable.Empty<object[]>();
            var lines = csvStr.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            // ignore first line
            if (lines.Length <= 1)
                return Enumerable.Empty<object[]>();
            List<object[]> rows = new List<object[]>();
            for (int i=1; i< lines.Length; i++)
			{
                var line = lines[i];
                var values = line.Split(',');
                rows.Add(values);
			}
            return rows;
        }
    }
}

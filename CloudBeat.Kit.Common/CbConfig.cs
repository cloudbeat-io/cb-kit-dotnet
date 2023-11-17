using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudBeat.Kit.Common
{
    public class CbConfig
	{
        public const string DEFAULT_WEBDRIVER_URL = "http://localhost:4444/wd/hub";
        public const string CB_API_KEY = "CB_API_KEY";
        public const string CB_API_URL = "CB_API_URL";
        public const string CB_PROJECT_ID = "CB_PROJECT_ID";
        public const string CB_RUN_ID = "CB_RUN_ID";
        public const string CB_AGENT = "CB_AGENT";
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
        string seleniumUrl;
        string appiumUrl;
        Dictionary<string, string> _metadata;
        Dictionary<string, string> _capabilities;
        Dictionary<string, string> _envVars;
        Dictionary<string, string> _options;
        IList<string> tags;
        IList<string> cases;

        public void loadFromEnvironment()
		{
            _apiKey = Environment.GetEnvironmentVariable(CB_API_KEY);
            _apiUrl = Environment.GetEnvironmentVariable(CB_API_URL);
            _runId = Environment.GetEnvironmentVariable(CB_RUN_ID);
            _instanceId = Environment.GetEnvironmentVariable(CB_INSTANCE_ID);
            _projectId = Environment.GetEnvironmentVariable(CB_PROJECT_ID);
            _isCbAgent = parseBool(Environment.GetEnvironmentVariable(CB_AGENT));
        }

        private static bool parseBool(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            bool.TryParse(str, out bool result);
            return result;
        }

        public bool HasMandatory()
		{
            return _isCbAgent;
        }

        public string RunId => _runId;
        public string InstanceId => _instanceId;
        public string ProjectId => _projectId;
        public string ApiKey => _apiKey;
        public string ApiUrl => _apiUrl;
        public Dictionary<string, string> Metadata => _metadata;
        public Dictionary<string, string> Capabilities => _capabilities;
        public Dictionary<string, string> EnvironmentVariables => _envVars;
        public Dictionary<string, string> Options => _options;

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

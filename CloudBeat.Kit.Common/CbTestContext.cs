using System;

namespace CloudBeat.Kit.Common
{
    public class CbTestContext
	{
		public const string CB_ENV_AGENT = "CB_AGENT";

		protected readonly CbTestReporter _reporter;
		protected readonly CbConfig _config;

		public CbTestContext(CbConfig config)
		{
			if (config.HasMandatory())
				_reporter = new CbTestReporter(config);
			_config = config;
		}
		public CbTestContext(CbConfig config, CbTestReporter reporter)
        {
			_reporter = reporter;
			_config = config;
        }
		public static bool IsEnabled
		{
			get
			{
				var isEnabled = parseBool(Environment.GetEnvironmentVariable(CB_ENV_AGENT));
				return isEnabled;
			}
		}
		public bool IsConfigured => _reporter != null;
		public CbTestReporter Reporter => _reporter;
		public CbConfig Config => _config;

        private static bool parseBool(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            bool.TryParse(str, out bool result);
            return result;
        }
    }
}

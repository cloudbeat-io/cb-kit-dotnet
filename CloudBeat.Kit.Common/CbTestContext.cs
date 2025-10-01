using System;

namespace CloudBeat.Kit.Common
{
    public class CbTestContext
	{
		private const string CB_ENV_AGENT = "CB_AGENT";
        private const string CB_VERBOSE_STACK_TRACE = "CB_VERBOSE_STACK_TRACE";

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
				bool.TryParse(Environment.GetEnvironmentVariable(CB_ENV_AGENT), out bool isEnabled);
                return isEnabled;
			}
		}

        public static bool IsVerboseStackTrace
        {
            get
            {
                bool.TryParse(Environment.GetEnvironmentVariable(CB_VERBOSE_STACK_TRACE), out bool IsVerboseStackTrace);
                return IsVerboseStackTrace;
            }
        }

        public bool IsConfigured => _reporter != null;
		public CbTestReporter Reporter => _reporter;
		public CbConfig Config => _config;
    }
}

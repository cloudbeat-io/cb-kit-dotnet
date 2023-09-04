using System;

namespace CloudBeat.Kit.Common
{
    public class CbTestContext
	{
		public const string CB_ENV_ENABLED = "CB_ENABLED";

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
				var isEnabled = Environment.GetEnvironmentVariable(CB_ENV_ENABLED);
				if (string.IsNullOrEmpty(isEnabled))
					return false;
				return isEnabled.ToLower() == "true";
			}
		}
		public bool IsConfigured => _reporter != null;
		public CbTestReporter Reporter => this._reporter;
		public CbConfig Config => _config;
	}
}

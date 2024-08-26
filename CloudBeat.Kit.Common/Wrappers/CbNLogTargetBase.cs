using CloudBeat.Kit.Common.Enums;
using CloudBeat.Kit.Common.Models;
using NLog;
using NLog.Targets;
using System.Collections.Generic;

namespace CloudBeat.Kit.Common.Wrappers
{
    public abstract class CbNLogTargetBase : TargetWithLayout
	{
		private static readonly Dictionary<LogLevel, LogLevelEnum> NLOG_CB_LOG_LEVEL_MAP =
			new Dictionary<LogLevel, LogLevelEnum>()
			{
				{ LogLevel.Debug, LogLevelEnum.Debug },
				{ LogLevel.Info, LogLevelEnum.Info },
				{ LogLevel.Warn, LogLevelEnum.Warn },
				{ LogLevel.Error, LogLevelEnum.Error },
				{ LogLevel.Fatal, LogLevelEnum.Error },
				{ LogLevel.Trace, LogLevelEnum.Debug }
			};
		readonly CbTestReporter _reporter;
		public CbNLogTargetBase(CbTestReporter reporter)
		{
			_reporter = reporter;
		}
		protected override void Write(NLog.LogEventInfo logEvent)
		{
			if (_reporter == null)
				return;
			LogMessage logMessage = new LogMessage();
			logMessage.Time = logEvent.TimeStamp.ToUniversalTime();
			logMessage.Src = Enums.LogSourceEnum.User;
			logMessage.Message = logEvent.FormattedMessage;
			logMessage.Level = NLOG_CB_LOG_LEVEL_MAP.ContainsKey(logEvent.Level) 
				? NLOG_CB_LOG_LEVEL_MAP[logEvent.Level] : LogLevelEnum.Info;
			logMessage.Failure = CbExceptionHelper.GetFailureFromException(logEvent.Exception);

			_reporter.LogMessage(logMessage);
		}
	}
}

using CloudBeat.Kit.Common.Enums;
using System;

namespace CloudBeat.Kit.Common.Models
{
    public class LogMessage
	{
		public string Message { get; set; }
		public LogLevelEnum Level { get; set; }
        public DateTime Time { get; set; }
        public LogSourceEnum Src { get; set; }
        public FailureResult Failure { get; set; }
	}
}

using CloudBeat.Kit.Common.Enums;
using System;
using System.Collections.Generic;

namespace CloudBeat.Kit.Common.Models
{
    public class LogMessage
	{
        public string Id { get; set; } = Guid.NewGuid().ToString();
		public string Message { get; set; }
		public LogLevelEnum Level { get; set; }
        public DateTime Time { get; set; }
        public LogSourceEnum Src { get; set; }
        public IList<object> Args { get; set; }
        public FailureResult Failure { get; set; }
	}
}

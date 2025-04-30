using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CloudBeat.Kit.Common.Models
{
    public class CaseResult : TestableResultBase
	{
        protected readonly Dictionary<string, object> _context = new Dictionary<string, object>();
        
        public CaseResult() : base()
        {
        }

        public CaseResult(SuiteResult parentSuite) : base()
        {
            ParentSuite = parentSuite;
        }
        public Dictionary<string, object> Context => _context;

        public long? FailureReasonId { get; set; }

        public int? ReRunCount { get; set; }
        
        [JsonIgnore]
        public SuiteResult ParentSuite { get; private set; }
        public void End(TestStatusEnum? status = null, FailureResult failure = null)
        {
            EndTime = DateTime.UtcNow;
            Duration = ModelHelpers.CalculateDuration(StartTime, EndTime);
            Status = status ?? CalculateHooksAndStepsStatus();
            Failure = failure;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CloudBeat.Kit.Common.Models
{
    public class SuiteResult : IResultWithAttachment
    {
		private readonly string _id;
		private readonly ConcurrentBag<CaseResult> _cases;
		private readonly IList<LogMessage> _logs;
		private readonly Dictionary<string, object> _testAttributes = new Dictionary<string, object>();

		public SuiteResult() : this(Guid.NewGuid().ToString()) { }
		public SuiteResult(string id)
		{
			_id = id;
			_cases = new ConcurrentBag<CaseResult>();
			_logs = new List<LogMessage>();
			StartTime = DateTime.UtcNow;
		}
		public string Id => _id;
		public ConcurrentBag<CaseResult> Cases { get { return _cases; } }
		public IList<LogMessage> Logs => _logs;
		public string Name { get; set; }
		public string Fqn { get; set; }
		public IList<string> Arguments { get; set; }
		public Dictionary<string, object> TestAttributes => _testAttributes;
		public TestStatusEnum? Status { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public long? Duration { get; set; }
		public IList<Attachment> Attachments { get; set; } = new List<Attachment>();

        public CaseResult GetCaseByFqn(string fqn)
		{
			// Search from last to first (the latest added case first)
			return _cases.Reverse().Where(x => x.Fqn == fqn).FirstOrDefault();
		}

		internal CaseResult AddNewCase(string name, string fqn)
		{
			CaseResult newCase;
			_cases.Add(newCase = new CaseResult(this)
			{
				Name = name,
				Fqn = fqn,
				StartTime = DateTime.UtcNow
			});
			// add Suite's TestAttributes to the new Case (inherit)
			if (TestAttributes.Keys.Count > 0)
				foreach (var kvp in TestAttributes)
					newCase.TestAttributes.Add(kvp.Key, kvp.Value);
			return newCase;
		}

		public void Start()
		{
			StartTime = DateTime.UtcNow;
		}
		
		internal void End(TestStatusEnum? status = null)
        {
			EndTime = DateTime.UtcNow;
			Duration = ModelHelpers.CalculateDuration(StartTime, EndTime);
			if (!status.HasValue)
				UpdateSuiteStatus();
			else
				Status = status.Value;
		}

        internal CaseResult EndCase(string fqn, TestStatusEnum? status = null, FailureResult failure = null)
        {
			if (fqn == null)
				throw new ArgumentNullException("fqn");
			var caseResult = GetCaseByFqn(fqn);
			if (caseResult == null)
				return null;			
			return EndCase(caseResult, status, failure);
		}

		internal CaseResult EndCase(CaseResult caseResult, TestStatusEnum? status = null, FailureResult failure = null)
		{
			if (caseResult == null)
				return null;
			if (!_cases.Contains(caseResult))
				return null;
			caseResult.End(status, failure);
			End();
			return caseResult;
		}

		private void UpdateSuiteStatus()
        {
			if (Status == TestStatusEnum.Failed)
				return;
			if (_cases != null && _cases.Any(x => x.Status == TestStatusEnum.Failed || x.Status == TestStatusEnum.Broken))
				Status = TestStatusEnum.Failed;
			if (!Status.HasValue)
				Status = TestStatusEnum.Passed;
        }
    }
}

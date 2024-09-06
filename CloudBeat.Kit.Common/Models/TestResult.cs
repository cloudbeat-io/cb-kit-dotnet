using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CloudBeat.Kit.Common.Models
{
	public class TestResult
	{
		private readonly ConcurrentBag<SuiteResult> _suites = new ConcurrentBag<SuiteResult>();

		public string RunId { get; set; }
		public string InstanceId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public long? Duration { get; set; }
		public TestStatusEnum? Status { get; set; }
		public Dictionary<string, string> Options { get; set; }
		public Dictionary<string, string> Capabilities { get; set; }
		public Dictionary<string, string> MetaData { get; set; }
		public Dictionary<string, string> EnvironmentVariables { get; set; }
		public FailureResult Failure { get; set; }
		public IEnumerable<SuiteResult> Suites { get { return _suites; } }

        public TestResult() : this(null, null)
        {
        }

        public TestResult(string runId, string instanceId)
        {
            RunId = runId;
            InstanceId = instanceId;
            StartTime = DateTime.UtcNow;
        }

        public SuiteResult AddNewSuite(string name, string fqn)
		{
			SuiteResult newSuite;
			_suites.Add(newSuite = new SuiteResult()
			{
				Name = name,
				Fqn = fqn,
				StartTime = DateTime.UtcNow
			});
			return newSuite;
		}

		public SuiteResult GetSuite(string fqn)
		{
			return _suites.FirstOrDefault(x => x.Fqn == fqn);
		}

		public SuiteResult GetSuiteByCaseFqn(string caseFqn)
		{
			return _suites.Where(x => caseFqn.StartsWith(x.Fqn)).FirstOrDefault();
		}

		public void End()
        {
			EndTime = DateTime.UtcNow;
			Duration = ModelHelpers.CalculateDuration(StartTime, EndTime);
			UpdateStatus();
        }

		private void UpdateStatus()
		{
			if (Status.HasValue)
				return;

			var hasFailedSuite = _suites.Any(x => x.Status.HasValue && (x.Status.Value == TestStatusEnum.Failed || x.Status.Value == TestStatusEnum.Broken));
			Status = hasFailedSuite ? TestStatusEnum.Failed : TestStatusEnum.Passed;
		}
    }
}

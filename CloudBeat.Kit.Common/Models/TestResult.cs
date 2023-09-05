using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CloudBeat.Kit.Common.Models
{
	public class TestResult
	{
		private readonly ConcurrentBag<SuiteResult> _suites = new ConcurrentBag<SuiteResult>();

		public TestResult() : this(null, null) { }
		public TestResult(string runId, string instanceId) : this(runId, instanceId, null, null, null, null)
		{

		}
		public TestResult(
			string runId,
			string instanceId,
			Dictionary<string, string> options,
			Dictionary<string, string> capabilities,
			Dictionary<string, string> metaData,
			Dictionary<string, string> environmentVariables)
		{
			RunId = runId;
			InstanceId = instanceId;
			StartTime = DateTime.UtcNow;
			//_suites = new List<SuiteResult>();
		}

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
			return _suites.Where(x => x.Fqn == fqn).FirstOrDefault();
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

			if (_suites.Any(x => x.Status.HasValue && (x.Status.Value == TestStatusEnum.Failed || x.Status.Value == TestStatusEnum.Broken)))
			{
				Status = TestStatusEnum.Failed;
            }
			else if (_suites.Any(x => x.Status.HasValue && x.Status.Value == TestStatusEnum.Warning))
			{
                Status = TestStatusEnum.Warning;
            }
            else
			{
                Status = TestStatusEnum.Passed;
            }
		}
    }
}

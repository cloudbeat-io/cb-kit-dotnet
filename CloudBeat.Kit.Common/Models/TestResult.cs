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
				Fqn = fqn
			});
			return newSuite;
		}

		public SuiteResult GetSuite(string fqn)
		{
			return FindSuiteInSuites(_suites, fqn);
        }

        private SuiteResult FindSuiteInSuites(ConcurrentBag<SuiteResult> suites, string suiteFqn)
        {
            foreach (var suite in suites)
            {
                if (suite.Fqn == suiteFqn)
                {
                    return suite;
                }

                var suiteInner = FindSuiteInSuites(suite.Suites, suiteFqn);
                if (suiteInner != null)
                {
                    return suiteInner;
                }
            }

            return null;
        }

        public SuiteResult GetSuiteByCaseFqn(string caseFqn)
		{
            var suiteFQN = caseFqn.Substring(0, caseFqn.LastIndexOf('.'));
            return _suites.Where(x => x.Fqn == suiteFQN).SingleOrDefault();
		}

        public CaseResult GetCase(string caseFqn)
        {
			return FindCaseInSuite(_suites, caseFqn);
        }

		private CaseResult FindCaseInSuite(ConcurrentBag<SuiteResult> suites, string caseFqn)
		{
            foreach (var suite in suites)
            {
                var caze = suite.Cases.Where(x => x.Fqn == caseFqn).SingleOrDefault();
                if (caze != null)
                {
                    return caze;
                }

				caze = FindCaseInSuite(suite.Suites, caseFqn);
                if (caze != null)
                {
                    return caze;
                }
            }

			return null;
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CloudBeat.Kit.Common.Models.Hook;
using Newtonsoft.Json;

namespace CloudBeat.Kit.Common.Models
{
    public class SuiteResult : TestableResultBase
    {
		public SuiteResult(SuiteResult parentSuite = null) : this(Guid.NewGuid().ToString()) { }

		private SuiteResult(string id, SuiteResult parentSuite = null) : base(id)
		{
			ParentSuite = parentSuite;
		}

		public ConcurrentBag<CaseResult> Cases { get; } = new ConcurrentBag<CaseResult>();
		public ConcurrentBag<SuiteResult> Suites { get; } = new ConcurrentBag<SuiteResult>();
		
		[JsonIgnore]
		public SuiteResult ParentSuite { get; }

		public CaseResult GetCaseByFqn(string fqn)
		{
			// Search from last to first (the latest added case first)
			return Cases.Reverse().FirstOrDefault(x => x.Fqn == fqn);
		}

		internal CaseResult AddNewCase(string name, string fqn)
		{
			CaseResult newCase;
			Cases.Add(newCase = new CaseResult(this)
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
		
		public void End(TestStatusEnum? status = null)
        {
			EndSubSuites();
			EndTime = DateTime.UtcNow;
			Duration = ModelHelpers.CalculateDuration(StartTime, EndTime);
			if (!status.HasValue)
				CalculateSuiteStatus();
			else
				Status = status.Value;
		}

		private void EndSubSuites()
		{
			foreach (var subSuite in Suites)
			{
				subSuite.End();
			}
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
			if (!Cases.Contains(caseResult))
				return null;
			caseResult.End(status, failure);
			End();
			return caseResult;
		}

		private void CalculateSuiteStatus()
        {
			if (Status == TestStatusEnum.Failed)
				return;
			if (Cases != null &&
			    Cases.Any(x => x.Status == TestStatusEnum.Failed || x.Status == TestStatusEnum.Broken))
				Status = TestStatusEnum.Failed;
			else
				Status = CalculateHooksAndStepsStatus();
			Status ??= TestStatusEnum.Passed;
        }

        public SuiteResult AddNewSuite(string name, string fqn)
        {
	        SuiteResult newSuite;
	        Suites.Add(newSuite = new SuiteResult(this)
	        {
		        Name = name,
		        Fqn = fqn,
		        StartTime = DateTime.UtcNow
	        });
	        return newSuite;
        }
    }
}

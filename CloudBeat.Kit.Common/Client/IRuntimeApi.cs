using CloudBeat.Kit.Common.Models;

namespace CloudBeat.Kit.Common.Client
{
    public interface IRuntimeApi
	{
		void UpdateRunStatus(string runId, RunStatusEnum status);
		void UpdateInstanceStatus(string instanceId, RunStatusEnum status);
		void UpdateTestCaseStatus(
			string runId,
			string instanceId,
			string caseResultId,
			string caseFqn,
			string caseName,
			TestStatusEnum? status,
			FailureResult failure);
		void StartRun(string projectId, string runId = null, RunStatusEnum? status = null);
		void StartInstance(string runId, string instanceId = null, RunStatusEnum? status = null);
        void EndInstance(string runId, string instanceId, TestResult result);     
    }
}

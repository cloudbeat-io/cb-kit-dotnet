using CloudBeat.Kit.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBeat.Kit.Common.Client
{
	public interface IRunApi
	{
		void UpdateRunStatus(string runId, RunStatusEnum status);
		void UpdateInstanceStatus(string instanceId, RunStatusEnum status);
		void StartRun(string projectId, string runId = null, RunStatusEnum? status = null);
		void StartInstance(string runId, string instanceId = null, RunStatusEnum? status = null, Dictionary<string, string> attributes = null);
        void EndInstance(string runId, string instanceId, TestResult result);
    }
}

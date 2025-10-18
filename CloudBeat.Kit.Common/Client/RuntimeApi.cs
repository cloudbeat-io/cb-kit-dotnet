using System;
using CloudBeat.Kit.Common.Client.Dto;
using CloudBeat.Kit.Common.Models;

namespace CloudBeat.Kit.Common.Client;

public class RuntimeApi : IRuntimeApi
{
    private readonly string baseUrl;
    private readonly string token;

    public RuntimeApi(string baseUrl, string token)
    {
        this.baseUrl = baseUrl;
        this.token = token;
    }
    public void UpdateRunStatus(string runId, RunStatusEnum status)
    {
        throw new NotImplementedException();
    }

    public void UpdateInstanceStatus(string instanceId, RunStatusEnum status)
    {
        throw new NotImplementedException();
    }

    public void UpdateTestCaseStatus(string runId, string instanceId, string caseResultId, string caseFqn, string caseName,
        TestStatusEnum? status, FailureResult failure)
    {
        throw new NotImplementedException();
    }

    public void UpdateRuntimeCaseStatus(CaseStatusUpdateRequest statusUpdateRequest)
    {
        if (string.IsNullOrEmpty(statusUpdateRequest.RunId))
            throw new ArgumentException("RunId must be specified");
        if (string.IsNullOrEmpty(statusUpdateRequest.InstanceId))
            throw new ArgumentException("InstanceId must be specified");
        if (string.IsNullOrEmpty(baseUrl))
            throw new ArgumentException("BaseUrl must be specified");

        var url = $"{baseUrl.TrimEnd('/')}/runtime/run/{statusUpdateRequest.RunId}/instance/{statusUpdateRequest.InstanceId}/case/status";
        CbHttpHelper.PostJsonAsync(url, this.token, statusUpdateRequest).Wait();
    }

    public void UpdateRuntimeSuiteStatus(SuiteStatusUpdateRequest statusUpdateRequest)
    {
        if (string.IsNullOrEmpty(statusUpdateRequest.RunId))
            throw new ArgumentException("RunId must be specified");
        if (string.IsNullOrEmpty(statusUpdateRequest.InstanceId))
            throw new ArgumentException("InstanceId must be specified");
        if (string.IsNullOrEmpty(baseUrl))
            throw new ArgumentException("BaseUrl must be specified");

        var url = $"{baseUrl.TrimEnd('/')}/runtime/run/{statusUpdateRequest.RunId}/instance/{statusUpdateRequest.InstanceId}/suite/status";
        CbHttpHelper.PostJsonAsync(url, this.token, statusUpdateRequest).Wait();
    }

    public void StartRun(string projectId, string runId = null, RunStatusEnum? status = null)
    {
    }

    public void StartInstance(string runId, string instanceId = null, RunStatusEnum? status = null)
    {
    }

    public void EndInstance(string runId, string instanceId, TestResult result)
    {
    }
}
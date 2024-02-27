using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CloudBeat.Kit.Common.Wrappers
{
	public class CbHttpMessageHandler : DelegatingHandler
    {
        readonly CbTestReporter _reporter;
        public CbHttpMessageHandler(CbTestReporter reporter)
		{
            _reporter = reporter;
            InnerHandler = new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            var httpStep = _reporter.StartStep(GetStepNameFromRequest(request));
            httpStep.Type = Models.StepTypeEnum.Http;
            var response = await base.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _reporter.EndStep(httpStep, CloudBeat.Kit.Common.Models.TestStatusEnum.Failed);
                httpStep.Failure = new CloudBeat.Kit.Common.Models.FailureResult
                {
                    Type = "HTTP_ERROR",
                    Message = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}",
                    Data = response.ReasonPhrase
                };
            }
            else
                _reporter.EndStep(httpStep);
            return response;
        }

        private static string GetStepNameFromRequest(HttpRequestMessage request)
        {
            var url = request.RequestUri.ToString();
            var method = request.Method.Method.ToUpper();
            return $"{method} {url}";
        }
    }
}


using CloudBeat.Kit.Common.Models.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
            if (_reporter == null)
                return await base.SendAsync(request, cancellationToken);
            // StartStep method might return null if API call is made in NUnit hook method and not in the test method
            var httpStep = _reporter.StartStep(GetStepNameFromRequest(request));
            if (httpStep == null)
                return await base.SendAsync(request, cancellationToken);
            httpStep.Type = Models.StepTypeEnum.Http;
            HttpStepExtra extra = new HttpStepExtra();
			httpStep.Extra.Add("http", extra);
            extra.Request = GetRequestResult(request);
			var response = await base.SendAsync(request, cancellationToken);
			extra.Response = GetResponseResult(response);
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

		private static ResponseResult GetResponseResult(HttpResponseMessage response)
		{
			ResponseResult respRes = new ResponseResult();
            respRes.StatusCode = (int)response.StatusCode;
			respRes.StatusText = response.ReasonPhrase;
			respRes.Version = response.Version.ToString();
			respRes.Headers = GetResponseHeaders(response.Headers, response.Content?.Headers);
			respRes.ContentType = GetContentTypeFromHeaders(respRes.Headers);
			respRes.Cookies = GetCookiesFromHeaders(respRes.Headers);
			respRes.Content = response.Content?.ReadAsStringAsync().Result;

			return respRes;
		}

		private static string GetCookiesFromHeaders(Dictionary<string, string> headers)
		{
			if (headers.ContainsKey("Set-Cookie"))
				return headers["Set-Cookie"];
			if (headers.ContainsKey("set-cookie"))
				return headers["set-cookie"];
			return null;
		}

		private static string GetContentTypeFromHeaders(Dictionary<string, string> headers)
		{
			if (headers.ContainsKey("Content-Type"))
				return headers["Content-Type"];
			else if (headers.ContainsKey("content-type"))
				return headers["content-type"];
			return null;
		}

		private static Dictionary<string, string> GetResponseHeaders(HttpResponseHeaders responseHeaders, HttpContentHeaders contentHeaders)
		{
			Dictionary<string, string> headersDic = new Dictionary<string, string>();
			if (contentHeaders != null)
				foreach (var header in contentHeaders)
				{
					if (headersDic.ContainsKey(header.Key)) continue;
					string value = header.Value != null ? string.Join('\n', header.Value.Select(x => x.ToString()).ToArray()) : null;
					headersDic.Add(header.Key, value);
				}
			foreach (var header in responseHeaders)
			{
				if (headersDic.ContainsKey(header.Key)) continue;
				string value = header.Value != null ? string.Join('\n', header.Value.Select(x => x.ToString()).ToArray()) : null;
				headersDic.Add(header.Key, value);
			}
			return headersDic;
		}

		private static RequestResult GetRequestResult(HttpRequestMessage request)
		{
            RequestResult reqRes = new RequestResult();
            reqRes.Method = request.Method.ToString().ToUpper();
            reqRes.Url = request.RequestUri.ToString();
            reqRes.Version = request.Version.ToString();
            reqRes.Headers = GetRequestHeaders(request.Headers, request.Content?.Headers);
			reqRes.ContentType = GetContentTypeFromHeaders(reqRes.Headers);
			reqRes.Content = request.Content?.ReadAsStringAsync().Result;

			return reqRes;
		}

		private static Dictionary<string, string> GetRequestHeaders(HttpRequestHeaders httpHeaders, HttpContentHeaders contentHeaders)
		{
			Dictionary<string, string> headersDic = new Dictionary<string, string>();
			if (contentHeaders != null)
				foreach (var header in contentHeaders)
				{
					if (headersDic.ContainsKey(header.Key)) continue;
					string value = header.Value != null ? string.Join('\n', header.Value.Select(x => x.ToString()).ToArray()) : null;
					headersDic.Add(header.Key, value);
				}
			foreach (var header in httpHeaders)
            {
                if (headersDic.ContainsKey(header.Key)) continue;
                string value = header.Value != null ? string.Join('\n', header.Value.Select(x => x.ToString()).ToArray()) : null;
                headersDic.Add(header.Key, value);
			}
            return headersDic;
		}

		private static string GetStepNameFromRequest(HttpRequestMessage request)
        {
            var url = request.RequestUri.ToString();
            var method = request.Method.Method.ToUpper();
            return $"{method} {url}";
        }
    }
}


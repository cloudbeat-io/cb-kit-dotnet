using System;
using System.Threading;
using System.Threading.Tasks;
using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Models;
using CloudBeat.Kit.Common.Models.Http;
using Microsoft.Playwright;
using System.Linq;
using System.Web;

namespace CloudBeat.Kit.Playwright
{
    public static class Helper
	{
        public static Task<StepResult> StartStepAsync(string namePrefix, string value, IPage page, CbTestReporter reporter)
        {
            if (page == null || reporter == null)
                return null;
            return Task.FromResult(reporter?.StartStep($"{namePrefix} \"{value}\""));
        }


        public static void EndTaskStep(Task task, StepResult step, CbTestReporter reporter)
		{
			if (reporter == null)
				return;
            if (!task.IsCompleted)
                task.Wait();
            if (task.IsFaulted)
                reporter.EndStep(step, TestStatusEnum.Failed, task.Exception);
			else
                reporter.EndStep(step);
        }

		public static async Task<string> GetLocatorLabel(ILocator locator, IPage page)
		{
            string nameOrValue = await locator.InnerTextAsync();
            var tagName = (await locator.EvaluateAsync<string>("e => e.tagName"))?.ToLower();
            var elmName = await locator.GetAttributeAsync("name");
            var elmType = await locator.GetAttributeAsync("type");
            string typeLabel = "";
            // determine element type (link, button or other)
            if (tagName == "a")
            {
                typeLabel = "link";
            }
            else if (tagName == "div")
            {
                nameOrValue = TrimAndReplaceNewLines(nameOrValue);
            }
            else if (tagName == "button")
            {
                typeLabel = "button";
            }
            else if (tagName == "option")
            {
                typeLabel = "option";
            }
            else if (tagName == "label")
            {
                typeLabel = "label";
                nameOrValue = TrimAndReplaceLastColon(nameOrValue);
            }
            else if (tagName == "input" && elmType != null && (elmType == "button" || elmType == "submit"))
            {
                typeLabel = "button";
            }
            else if (tagName == "input" && elmType != null && elmType == "text")
            {
                typeLabel = "field";
            }
            else if (tagName == "input" && elmType != null && elmType == "password")
            {
                typeLabel = "password field";
            }
            else if (tagName == "input" && elmType != null && elmType == "url")
            {
                typeLabel = "link";
            }

            // try to retrieve element's display text or caption
            if (string.IsNullOrEmpty(nameOrValue))
            {
                if (typeLabel.EndsWith("field") && !string.IsNullOrEmpty(elmName))
                    nameOrValue = await GetFormFieldLabel(elmName, page);
                else
                    nameOrValue = await locator.GetAttributeAsync("placeholder");
                if (string.IsNullOrEmpty(nameOrValue))
                    nameOrValue = await locator.GetAttributeAsync("value");
            }

            if (!string.IsNullOrEmpty(nameOrValue) && !string.IsNullOrEmpty(typeLabel))
                return $"{typeLabel} \"{nameOrValue.Replace("\n", " ")}\"";
            else if (!string.IsNullOrEmpty(nameOrValue))
                return $"\"{nameOrValue}\"";
            else
                return $"<{tagName}>";
        }


        private static string TrimAndReplaceNewLines(string text)
        {
            text = text?.Trim().Replace("\n", " ");
            if (string.IsNullOrEmpty(text))
                return null;
            return text;
        }

        private static string TrimAndReplaceLastColon(string text)
        {
            text = text?.Trim();
            if (string.IsNullOrEmpty(text))
                return null;
            if (text.EndsWith(":"))
                return text.Substring(0, text.Length - 1);
            return text;
        }

        private static async Task<string> GetFormFieldLabel(string fieldName, IPage page)
        {
            var locator = page.Locator($"label[for='{fieldName}']");
            if (locator == null)
                return null;
            // remove white space and ":" suffix if presented
            var labelText = await locator.InnerTextAsync();
            return TrimAndReplaceLastColon(labelText);
        }

        public static Task<T> WrapStepTask<T>(
            Task<T> task,
            StepResult step,
            IPage page,
            CbTestReporter reporter)
        {
            if (reporter == null || step == null)
                return task;
            task.ConfigureAwait(true);
            if (page != null)
                reporter.SetScreenshotProvider(new CbPwScreenshotProvider(page));
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            task.ContinueWith(ignored =>
            {
                switch (task.Status)
                {
                    case TaskStatus.Canceled:
                        reporter.EndStep(step, TestStatusEnum.Skipped);
                        tcs.SetCanceled();
                        break;
                    case TaskStatus.RanToCompletion:
                        reporter.EndStep(step, TestStatusEnum.Passed);
                        tcs.SetResult(task.Result);
                        break;
                    case TaskStatus.Faulted:
                        string screenshot = null;// GetScreenshot(page);
                        reporter.EndStep(step, TestStatusEnum.Failed, task.Exception, screenshot);
                        tcs.SetException(task.Exception);
                        break;
                    default:
                        break;
                }
            }, new CancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.Current);

            return tcs.Task;
        }

        public static Task<T> WrapHttpStepTask<T>(
            Task<T> task,
            StepResult step,
            CbTestReporter reporter)
        {
            if (reporter == null || step == null)
                return task;
            task.ConfigureAwait(true);
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            task.ContinueWith(ignored =>
            {
                switch (task.Status)
                {
                    case TaskStatus.Canceled:
                        reporter.EndStep(step, TestStatusEnum.Skipped);
                        tcs.SetCanceled();
                        break;
                    case TaskStatus.RanToCompletion:
                        EndHttpStep(step, reporter, task.Result as IAPIResponse, null);
                        tcs.SetResult(task.Result);
                        break;
                    case TaskStatus.Faulted:
                        EndHttpStep(step, reporter, null, task.Exception);
                        tcs.SetException(task.Exception);
                        break;
                    default:
                        break;
                }
            }, new CancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.Current);

            return tcs.Task;
        }

        public static StepResult StartHttpStep(string url, string method, APIRequestContextOptions options, CbTestReporter reporter)
        {
            if (reporter == null)
                return null;
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            string stepName = $"{method} {(uri.IsAbsoluteUri ? uri.PathAndQuery : uri.OriginalString)}";
            StepResult httpStep = reporter.StartStep(stepName, StepTypeEnum.Http);
            if (httpStep == null)
            {
                // could happen if HTTP method was invoked from OneTimeSetUp or similar methods.
                // such methods do not trigger StartCase and as the result there is nothing to add the new step to
                // and as a consequence we get null returned instead of an actual new step.
                return null;
            }
            var extra = new HttpStepExtra();
            extra.Request = new RequestResult();
            extra.Request.Url = url;
            extra.Request.Method = method;
            extra.Request.Path = uri.IsAbsoluteUri ? uri.AbsolutePath: uri.OriginalString;
            // parse Query string if specified
            if (uri.IsAbsoluteUri && !string.IsNullOrEmpty(uri.Query))
            {
                var queryString = HttpUtility.ParseQueryString(uri.Query);
                extra.Request.QueryParams = queryString.AllKeys.ToDictionary(k => k, k => queryString[k]);
            }
            if (options != null)
            {
                if (options.Headers != null)
                    foreach (var header in options.Headers)
                    {
                        if (!extra.Request.Headers.ContainsKey(header.Key))
                            extra.Request.Headers.Add(header.Key, header.Value);
                        if (header.Key.ToLower() == "content-type")
                            extra.Request.ContentType = header.Value;
                        if (header.Key.ToLower() == "version")
                            extra.Request.Version = header.Value;
                        if (header.Key.ToLower() == "content-length")
                            extra.Response.ContentLength = GetContentLengthFromHeader(header.Value);
                    }
                // Assume that if Data starts with '{' and not Content-Type is defined
                // then we have a serialized object in JSON format
                if (options.Data != null && string.IsNullOrEmpty(extra.Request.ContentType) && options.Data.StartsWith("{"))
                    extra.Request.ContentType = "application/json";
                extra.Request.Content = options.Data ?? options.DataString;
            }
            httpStep.Extra.Add("http", extra);
            return httpStep;
        }

        private static void EndHttpStep(StepResult step, CbTestReporter reporter, IAPIResponse response, Exception e)
        {
            TestStatusEnum status = e != null ? TestStatusEnum.Failed : TestStatusEnum.Passed;
            reporter.EndStep(step, status, e, null);
            if (response == null)
                return;
            if (!step.Extra.ContainsKey("http"))
                return;
            HttpStepExtra extra = step.Extra["http"] as HttpStepExtra;
            if (extra == null)
                return;
            extra.Response = new ResponseResult();
            extra.Response.StatusCode = response.Status;
            extra.Response.StatusText = response.StatusText;
            extra.Response.Url = response.Url;
            if (response.HeadersArray != null)
            {
                foreach (var header in response.HeadersArray)
                {
                    if (!extra.Response.Headers.ContainsKey(header.Name))
                        extra.Response.Headers.Add(header.Name, header.Value);
                    if (header.Name.ToLower() == "content-type")
                        extra.Response.ContentType = header.Value;
                    if (header.Name.ToLower() == "version")
                        extra.Response.Version = header.Value;
                    if (header.Name.ToLower() == "content-length")
                        extra.Response.ContentLength = GetContentLengthFromHeader(header.Value);
                }
                    
            }
            extra.Response.Content = response.TextAsync().Result;
        }

        private static long? GetContentLengthFromHeader(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            long contentLength;
            if (long.TryParse(value, out contentLength))
                return contentLength;
            return null;
        }

        public static Task WrapStepTask(
            Task task,
            StepResult step,
            IPage page,
            CbTestReporter reporter)
        {
            if (reporter == null)
                return task;
            task.ConfigureAwait(true);
            reporter.SetScreenshotProvider(new CbPwScreenshotProvider(page));
            TaskCompletionSource tcs = new TaskCompletionSource();
            task.ContinueWith(ignored =>
            {
                switch (task.Status)
                {
                    case TaskStatus.Canceled:
                        reporter.EndStep(step, TestStatusEnum.Skipped);
                        tcs.SetCanceled();
                        break;
                    case TaskStatus.RanToCompletion:
                        reporter.EndStep(step, TestStatusEnum.Passed);
                        tcs.SetResult();
                        break;
                    case TaskStatus.Faulted:
                        var screenshot = TakeScreenshot(page);
                        reporter.EndStep(step, TestStatusEnum.Failed, task.Exception, screenshot);
                        tcs.SetException(task.Exception);
                        break;
                    default:
                        break;
                }
            }, new CancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.Current);

            return tcs.Task;
        }

        public static string TakeScreenshot(IPage page)
        {
            try
            {
                var task = page.ScreenshotAsync();
                var success = task.Wait(30000);
                if (!success) return null;
                var bytes = task.Result;
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return null;
            }
        }
    }
}


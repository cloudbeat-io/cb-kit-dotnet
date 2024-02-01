
using System.Threading;
using System.Threading.Tasks;
using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Models;
using Microsoft.Playwright;

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
            if (reporter == null)
                return task;
            task.ConfigureAwait(true);
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


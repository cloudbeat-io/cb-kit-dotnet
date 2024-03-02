using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using CloudBeat.Kit.Common;

namespace CloudBeat.Kit.Playwright
{
	public class CbAPIRequestContextWrapper : IAPIRequestContext
    {
        protected readonly IAPIRequestContext context;
        protected readonly CbTestReporter reporter;

        public CbAPIRequestContextWrapper(IAPIRequestContext context, CbTestReporter reporter)
		{
			this.context = context;
            this.reporter = reporter;
        }

        public IFormData CreateFormData()
        {
            return context.CreateFormData();
        }

        public Task<IAPIResponse> DeleteAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.DeleteAsync(url, options);
            var step = Helper.StartHttpStep(url, "DELETE", options, reporter);
            var task = context.DeleteAsync(url, options);
            return Helper.WrapHttpStepTask(task, step, reporter);
        }

        public ValueTask DisposeAsync()
        {
            return context.DisposeAsync();
        }

        public Task<IAPIResponse> FetchAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.FetchAsync(url, options);
            var step = Helper.StartHttpStep(url, "FETCH", options, reporter);
            var task = context.FetchAsync(url, options);
            return Helper.WrapHttpStepTask(task, step, reporter);
        }

        public Task<IAPIResponse> FetchAsync(IRequest request, APIRequestContextOptions options = null)
        {
            return context.FetchAsync(request, options);
        }

        public Task<IAPIResponse> GetAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.GetAsync(url, options);
            var step = Helper.StartHttpStep(url, "GET", options, reporter);
            var task = context.GetAsync(url, options);
            return Helper.WrapHttpStepTask(task, step, reporter);
        }

        public Task<IAPIResponse> HeadAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.HeadAsync(url, options);
            var step = Helper.StartHttpStep(url, "HEAD", options, reporter);
            var task = context.HeadAsync(url, options);
            return Helper.WrapHttpStepTask(task, step, reporter);
        }

        public Task<IAPIResponse> PatchAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.PatchAsync(url, options);
            var step = Helper.StartHttpStep(url, "PATCH", options, reporter);
            var task = context.PatchAsync(url, options);
            return Helper.WrapHttpStepTask(task, step, reporter);
        }

        public Task<IAPIResponse> PostAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.PostAsync(url, options);
            var step = Helper.StartHttpStep(url, "POST", options, reporter);
            var task = context.PostAsync(url, options);
            return Helper.WrapHttpStepTask(task, step, reporter);
        }

        public Task<IAPIResponse> PutAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.PutAsync(url, options);
            var step = Helper.StartHttpStep(url, "PUT", options, reporter);
            var task = context.PutAsync(url, options);
            return Helper.WrapHttpStepTask(task, step, reporter);
        }

        public Task<string> StorageStateAsync(APIRequestContextStorageStateOptions options = null)
        {
            return context.StorageStateAsync(options);
        }
    }
}


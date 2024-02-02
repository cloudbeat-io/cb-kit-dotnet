using System;
using System.Threading.Tasks;
using CloudBeat.Kit.Common;
using Microsoft.Playwright;

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
            var step = reporter?.StartStep($"DELETE {url}");
            var task = context.DeleteAsync(url, options);
            return Helper.WrapStepTask(task, step, null, reporter);
        }

        public ValueTask DisposeAsync()
        {
            return context.DisposeAsync();
        }

        public Task<IAPIResponse> FetchAsync(string urlOrRequest, APIRequestContextOptions options = null)
        {
            return context.FetchAsync(urlOrRequest, options);
        }

        public Task<IAPIResponse> FetchAsync(IRequest urlOrRequest, APIRequestContextOptions options = null)
        {
            return context.FetchAsync(urlOrRequest, options);
        }

        public Task<IAPIResponse> GetAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.GetAsync(url, options);
            var step = reporter?.StartStep($"GET {url}");
            var task = context.GetAsync(url, options);
            return Helper.WrapStepTask(task, step, null, reporter);
        }

        public Task<IAPIResponse> HeadAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.HeadAsync(url, options);
            var step = reporter?.StartStep($"HEAD {url}");
            var task = context.HeadAsync(url, options);
            return Helper.WrapStepTask(task, step, null, reporter);
        }

        public Task<IAPIResponse> PatchAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.PatchAsync(url, options);
            var step = reporter?.StartStep($"PATCH {url}");
            var task = context.PatchAsync(url, options);
            return Helper.WrapStepTask(task, step, null, reporter);
        }

        public Task<IAPIResponse> PostAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.PostAsync(url, options);
            var step = reporter?.StartStep($"POST {url}");
            var task = context.PostAsync(url, options);
            return Helper.WrapStepTask(task, step, null, reporter);
        }

        public Task<IAPIResponse> PutAsync(string url, APIRequestContextOptions options = null)
        {
            if (reporter == null)
                return context.PutAsync(url, options);
            var step = reporter?.StartStep($"PUT {url}");
            var task = context.PutAsync(url, options);
            return Helper.WrapStepTask(task, step, null, reporter);
        }

        public Task<string> StorageStateAsync(APIRequestContextStorageStateOptions options = null)
        {
            return context.StorageStateAsync(options);
        }
    }
}


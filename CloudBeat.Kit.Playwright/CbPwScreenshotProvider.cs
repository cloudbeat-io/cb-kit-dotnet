using CloudBeat.Kit.Common;
using Microsoft.Playwright;

namespace CloudBeat.Kit.Playwright
{
    public class CbPwScreenshotProvider : ICbScreenshotProvider
	{
        private readonly IPage page;

        public CbPwScreenshotProvider(IPage page)
		{
            this.page = page;
		}

        public string TakeScreenshot()
        {
            return Helper.TakeScreenshot(page);
        }
    }
}


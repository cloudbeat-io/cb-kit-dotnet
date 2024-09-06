using CloudBeat.Kit.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace CloudBeat.Kit.MSTest
{
    public class CbMSTestScreenshotProvider : ICbScreenshotProvider
	{
        private readonly IWebDriver driver;

        public CbMSTestScreenshotProvider(IWebDriver driver)
		{
            this.driver = driver;
		}

        public string TakeScreenshot()
        {
            return driver.TakeScreenshot().AsBase64EncodedString;
        }
    }
}


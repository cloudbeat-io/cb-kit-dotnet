using CloudBeat.Kit.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using System;

namespace CloudBeat.Kit.MSTest
{
    public class CbMSTestScreenshotProvider : ICbScreenshotProvider
	{
        private readonly IWebDriver driver;
        private readonly bool takeFullPageScreenshots;

        public CbMSTestScreenshotProvider(IWebDriver driver, bool takeFullPageScreenshots)
		{
            this.driver = driver;
            this.takeFullPageScreenshots = takeFullPageScreenshots;
        }

        public string TakeScreenshot()
        {
            try
            {
                if (takeFullPageScreenshots && driver is ChromeDriver)
                {
                    try
                    {
                        return CbScreenshotHelper.TakeFullPageScreenshot(driver as ChromeDriver);
                    }
                    catch
                    {
                    }
                }
                return driver?.TakeScreenshot()?.AsBase64EncodedString;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}


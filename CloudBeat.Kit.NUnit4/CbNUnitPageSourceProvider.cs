using CloudBeat.Kit.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using System;

namespace CloudBeat.Kit.NUnit
{
    public class CbNUnitPageSourceProvider : ICbPageSourceProvider
	{
        private readonly IWebDriver _driver;

        public CbNUnitPageSourceProvider(IWebDriver driver, bool takePageSourceOnError)
		{
            this._driver = driver;
            this.TakePageSourceOnError = takePageSourceOnError;
        }

        public bool TakePageSourceOnError { get; }

        public (string Source, string MimeType) PageSource()
        {
            try
            {
                
                var source = _driver?.TakeScreenshot()?.AsBase64EncodedString;
                var mimeType = IsAppiumDriver(_driver) ? "application/x-appium+xml" : "text/html";
                return (source, mimeType);
            }
            catch (Exception)
            {
                return (null, null);
            }
        }
        
        private static bool IsAppiumDriver(IWebDriver driver)
        {
            // Method 1: Check driver type name
            var driverTypeName = driver.GetType().Name;
            if (driverTypeName.Contains("Appium") || 
                driverTypeName.Contains("Android") || 
                driverTypeName.Contains("IOS"))
            {
                return true;
            }

            // Method 2: Check capabilities for mobile-specific properties
            if (driver is IHasCapabilities capabilitiesDriver)
            {
                var capabilities = capabilitiesDriver.Capabilities;
        
                // Check for Appium/mobile-specific capabilities
                if (capabilities.HasCapability("platformName"))
                {
                    var platformName = capabilities.GetCapability("platformName")?.ToString();
                    if (platformName == "Android" || 
                        platformName == "iOS" || 
                        platformName == "android" || 
                        platformName == "ios")
                    {
                        return true;
                    }
                }

                // Check for other mobile-specific capabilities
                if (capabilities.HasCapability("appium:automationName") ||
                    capabilities.HasCapability("automationName") ||
                    capabilities.HasCapability("deviceName") ||
                    capabilities.HasCapability("udid"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}


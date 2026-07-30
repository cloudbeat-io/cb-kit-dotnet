using CloudBeat.Kit.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Reflection;

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
                var source = _driver?.PageSource;
                var mimeType = DetermineMimeType(_driver);
                return (source, mimeType);
            }
            catch (Exception)
            {
                return (null, null);
            }
        }

        private static string DetermineMimeType(IWebDriver driver)
        {
            // Appium drivers (AndroidDriver/IOSDriver) can switch between a NATIVE_APP context
            // (PageSource returns the UiAutomator2/XCUITest XML hierarchy) and a WEBVIEW_* context
            // (PageSource returns real HTML DOM - e.g. an embedded webview, or a mobile-web test
            // driven entirely through Appium). That can change mid-session, so IsAppiumDriver alone
            // (a static, session-level check) isn't enough - read the driver's live Context instead.
            var context = GetCurrentContext(driver);
            if (context != null)
                return string.Equals(context, "NATIVE_APP", StringComparison.OrdinalIgnoreCase) ? "application/xml" : "text/html";

            // Driver doesn't expose a live Context (not a context-aware/Appium driver) - fall back
            // to the coarser "is this an Appium driver at all" heuristic.
            return IsAppiumDriver(driver) ? "application/xml" : "text/html";
        }

        // Appium's Context property (OpenQA.Selenium.Appium.Interfaces.IContextAware, implemented by
        // AndroidDriver/IOSDriver) is read via reflection rather than a direct `is IContextAware` check
        // so this project doesn't need a hard dependency on the Appium.WebDriver package - it only
        // ever receives driver instances through the generic IWebDriver interface.
        private static string GetCurrentContext(IWebDriver driver)
        {
            try
            {
                var contextProperty = driver?.GetType().GetProperty("Context", BindingFlags.Public | BindingFlags.Instance);
                return contextProperty?.GetValue(driver) as string;
            }
            catch (Exception)
            {
                return null;
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


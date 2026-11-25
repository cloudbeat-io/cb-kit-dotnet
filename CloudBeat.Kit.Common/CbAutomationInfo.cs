using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace CloudBeat.Kit.Common
{
    /// <summary>
    /// Describes the automation engine behind a WebDriver: Appium (mobile) or Selenium (browser), with the platform,
    /// automation name, browser and device. Stored in each test result's context as "automation", so CloudBeat
    /// (Test Health) knows what a test-framework project (NUnit, MSTest…) actually drove and can apply the right rules.
    /// Never throws: a driver that cannot tell (closed session, custom driver) gives null.
    /// </summary>
    public static class CbAutomationInfo
    {
        public static Dictionary<string, object> Describe(IWebDriver driver)
        {
            if (driver == null)
                return null;
            try
            {
                ICapabilities caps = (driver as IHasCapabilities)?.Capabilities;
                string Cap(string name) => Text(caps, name) ?? Text(caps, "appium:" + name);

                var platform = Cap("platformName");
                var automationName = Cap("automationName");
                var browser = Text(caps, "browserName");
                var device = Cap("deviceName");
                var isAppium = IsAppiumType(driver.GetType()) || automationName != null
                               || string.Equals(platform, "ios", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(platform, "android", StringComparison.OrdinalIgnoreCase);

                var info = new Dictionary<string, object> { ["engine"] = isAppium ? "appium" : "selenium" };
                if (platform != null) info["platform"] = platform;
                if (automationName != null) info["automationName"] = automationName;
                if (browser != null) info["browser"] = browser;
                if (device != null) info["device"] = device;
                var platformVersion = Cap("platformVersion");
                if (platformVersion != null) info["platformVersion"] = platformVersion;
                return info;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // AppiumDriver, IOSDriver, AndroidDriver… (the kit does not reference the Appium client)
        private static bool IsAppiumType(Type type)
        {
            for (var t = type; t != null; t = t.BaseType)
                if ((t.Namespace ?? string.Empty).StartsWith("OpenQA.Selenium.Appium", StringComparison.Ordinal))
                    return true;
            return false;
        }

        private static string Text(ICapabilities caps, string name)
        {
            if (caps == null)
                return null;
            var value = caps.GetCapability(name)?.ToString();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}

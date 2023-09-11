using System;
using OpenQA.Selenium;
using static System.Net.Mime.MediaTypeNames;

namespace CloudBeat.Kit.Common
{
	public static class CbSeleniumHelper
	{
        public static string GetElementDisplayName(IWebElement elm, IWebDriver driver)
        {
            if (elm == null)
                return "element";
            var elmObjType = elm.GetType().Name;
            if (elmObjType == "AppiumElement")
                return GetAndroidElementDetails(elm);
            else if (elmObjType == "IOSElement")
                return GetIOSElementDetails(elm);
            return GetWebElementDetails(elm);
        }

        private static string GetWebElementDetails(IWebElement elm)
        {
            string text = elm.Text;
            string tagName = elm.TagName;
            string elmType = GetElementAttribute("type", elm);
            string elmTypeLabel = "";
            // determine element type (link, button or other)
            if (tagName == "a")
            {
                elmTypeLabel = "link ";
            }
            else if (tagName == "button")
            {
                elmTypeLabel = "button ";
            }
            else if (tagName == "option")
            {
                elmTypeLabel = "option ";
            }
            else if (tagName == "label")
            {
                elmTypeLabel = "label ";
            }
            else if (tagName == "input" && elmType != null && (elmType == "button" || elmType == "submit"))
            {
                elmTypeLabel = "button ";
            }
            else if (tagName == "input" && elmType != null && elmType == "url")
            {
                elmTypeLabel = "link ";
            }

            if (!string.IsNullOrEmpty(text))
                return $"{elmTypeLabel}\"{text}\"";
            else
                return $"<{tagName}>";
        }

        private static string GetAndroidElementDetails(IWebElement elm)
        {
            string elmType = "element";
            string text = elm.Text;
            var className = GetElementAttribute("className", elm);
            var clickable = GetElementAttribute("clickable", elm);
            bool isClickable = string.IsNullOrEmpty(clickable) ? false : bool.Parse(clickable);
            if (className != null && className.Contains("TextView") && !isClickable)
                elmType = "text";
            else if (className != null && className.Contains("EditText") && !isClickable)
                elmType = "field";

            if (!string.IsNullOrEmpty(text))
                return $"{elmType} \"{text}\"";
            return $"{elmType}";
        }

        private static string GetIOSElementDetails(IWebElement elm)
        {
            string elmType = "element";
            string text = elm.Text;

            var className = GetElementAttribute("type", elm);
            if (string.IsNullOrEmpty(className))
                return elmType;
            if (className == "XCUIElementTypeButton")
                elmType = "button";
            else if (className == "XCUIElementTypeStaticText")
                elmType = "text";
            else if (className == "XCUIElementTypeTextField")
                elmType = "field";

            if (!string.IsNullOrEmpty(text))
                return $"{elmType} \"{text}\"";
            return $"{elmType}";
        }

        private static string GetElementAttribute(string name, IWebElement elm)
        {
            try
            {
                return elm.GetAttribute(name);
            }
            catch { }
            return null;
        }
    }
}


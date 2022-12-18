using OpenQA.Selenium.Chrome;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CloudBeat.Kit.Common
{
    // following code is taken from https://stackoverflow.com/a/52063597
    // another example based on Selenium 4: https://dev.to/gdledsan/selenium-4-and-chrome-driver-take-full-page-screenthos-2j8d
    internal static class CbScreenshotHelper
    {
        private const string DEFAULT_SCREENSHOT_FORMAT = "Png";
		private const int DEFAULT_SCREENSHOT_QUALITY = 80;

		public static string TakeFullPageScreenshot(ChromeDriver driver)
        {
            // Evaluate this only to get the object that the
            // Emulation.setDeviceMetricsOverride command will expect.
            // Note that we can use the already existing ExecuteChromeCommand
            // method to set and clear the device metrics, because there's no
            // return value that we care about.
            string metricsScript = @"({
        width: Math.max(window.innerWidth,document.body.scrollWidth,document.documentElement.scrollWidth)|0,
        height: Math.max(window.innerHeight,document.body.scrollHeight,document.documentElement.scrollHeight)|0,
        deviceScaleFactor: window.devicePixelRatio || 1,
        mobile: typeof window.orientation !== 'undefined'
        })";
            Dictionary<string, object> metrics = EvaluateDevToolsScript(driver, metricsScript);
            driver.ExecuteCdpCommand("Emulation.setDeviceMetricsOverride", metrics);
            Thread.Sleep(1000);
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["format"] = "png",
                ["fromSurface"] = true
            };
            var fullHeight = int.Parse(metrics["height"]?.ToString() ?? "0");
            var splitSSAt = 8192;

            if (fullHeight > splitSSAt)
            {
                var currentHeight = splitSSAt;
                var startHeight = 0;
                List<SKBitmap> bitmaps = new List<SKBitmap>();

                while (fullHeight > 0)
                {
                    if (currentHeight > fullHeight)
                    {
                        currentHeight = fullHeight;
                    }
                    parameters["clip"] = new Dictionary<string, object>
                    {
                        ["x"] = 0,
                        ["y"] = startHeight,
                        ["width"] = metrics["width"],
                        ["height"] = currentHeight,
                        ["scale"] = 1,
                    };

                    object splitScreenshotObject = driver.ExecuteCdpCommand("Page.captureScreenshot", parameters);
                    Dictionary<string, object> splitScreenshotResult = splitScreenshotObject as Dictionary<string, object>;
                    Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(splitScreenshotResult["data"] as string));
                    MemoryStream streamBitmap = new MemoryStream(bitmapData);
                    var bitmap = SKBitmap.Decode(streamBitmap);

                    //bitmaps.Add(new Bitmap((Bitmap)Image.FromStream(streamBitmap)));
                    bitmaps.Add(bitmap);
                    fullHeight -= splitSSAt;
                    startHeight += splitSSAt;
                }

                var fullScreenBitmap = MergeImages(bitmaps);
				var imageData = fullScreenBitmap.Encode(SKEncodedImageFormat.Png, 80);
				return Convert.ToBase64String(imageData.ToArray()); //Get Base64
				//return new Screenshot(base64);

			}
            object screenshotObject = driver.ExecuteCdpCommand("Page.captureScreenshot", parameters);
            Dictionary<string, object> screenshotResult = screenshotObject as Dictionary<string, object>;
            string screenshotData = screenshotResult["data"] as string;

            driver.ExecuteCdpCommand("Emulation.clearDeviceMetricsOverride", new Dictionary<string, object>());
            return screenshotData;

			//var screenshot = new Screenshot(screenshotData);
            //return screenshot;
        }

        public static string FixBase64ForImage(string image)
        {
            StringBuilder sbText = new StringBuilder(image, image.Length);
            sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }

		private static SKBitmap MergeImages(IEnumerable<SKBitmap> images)
        {
			var totalWidth = 0;
			var totalHeight = 0;

			foreach (var image in images)
			{
				totalWidth = image.Width;
				totalHeight += image.Height;
			}
            var fullscreenBitmap = new SKBitmap(totalWidth, totalHeight);
            using (var canvas = new SKCanvas(fullscreenBitmap))
            {
				float currentImageYPos = 0;
				foreach (var image in images)
				{
					canvas.DrawBitmap(image, new SKPoint(0, currentImageYPos));
					currentImageYPos += image.Height;
				}
				//canvas.Save();
			}
            return fullscreenBitmap;
		}

        private static Dictionary<string, object> EvaluateDevToolsScript(ChromeDriver driver, string scriptToEvaluate)
        {
            // This code is predicated on knowing the structure of the returned
            // object as the result. In this case, we know that the object returned
            // has a "result" property which contains the actual value of the evaluated
            // script, and we expect the value of that "result" property to be an object
            // with a "value" property. Moreover, we are assuming the result will be
            // an "object" type (which translates to a C# Dictionary<string, object>).
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["returnByValue"] = true;
            parameters["expression"] = scriptToEvaluate;
            object evaluateResultObject = driver.ExecuteCdpCommand("Runtime.evaluate", parameters);
            Dictionary<string, object> evaluateResultDictionary = evaluateResultObject as Dictionary<string, object>;
            Dictionary<string, object> evaluateResult = evaluateResultDictionary["result"] as Dictionary<string, object>;
            // If we wanted to make this actually robust, we'd check the "type" property
            // of the result object before blindly casting to a dictionary.
            Dictionary<string, object> evaluateValue = evaluateResult["value"] as Dictionary<string, object>;
            return evaluateValue;
        }
    }
}

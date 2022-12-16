using CloudBeat.Kit.Common.Models;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.Extensions;
using System;

namespace CloudBeat.Kit.Common.Wrappers
{
    public class CbWebDriverWrapper
    {
        EventFiringWebDriver _driver;
        CbTestReporter _reporter;
        StepResult _startedStep;
        bool _fullPageScreenshot;
        public CbWebDriverWrapper(EventFiringWebDriver eventFiringWebDriver, CbTestReporter reporter, bool fullPageScreenshot = true)
        {
            _driver = eventFiringWebDriver;
            _reporter = reporter;
            _fullPageScreenshot = fullPageScreenshot;
        }

        private void SubscribeToWebDriverEvents()
        {
            _driver.ElementValueChanging += OnElementValueChanging;
            _driver.ElementValueChanged += OnElementValueChanged;

            _driver.FindingElement += OnFindingElement;
            _driver.FindElementCompleted += OnFindElementCompleted;

            _driver.Navigating += OnNavigating;
            _driver.Navigated += OnNavigated;

            _driver.NavigatingBack += OnNavigatingBack;
            _driver.NavigatedBack += OnNavigatedBack;

            _driver.NavigatingForward += OnNavigatingForward;
            _driver.NavigatedForward += OnNavigatedForward;

            _driver.ExceptionThrown += OnExceptionThrown;
        }

        private void OnExceptionThrown(object sender, WebDriverExceptionEventArgs e)
        {
            // close step opened by webdriver events because if exception occurs they won't be closed
            if (_startedStep != null && !_startedStep.IsFinished)
            {
                var endedStep = _reporter.EndStep(_startedStep, TestStatusEnum.Failed);
                if (endedStep != null)
                    endedStep.Failure = CbExceptionHelper.GetFailureFromException(e.ThrownException);
                _startedStep = null;
            }
            // TODO: fix the below
            /*
            try
            {
                var screenShotPath = $"{_context.FullyQualifiedTestClassName}_{_context.TestName}.png";

                Screenshot ss = null;
                if (TakeFullPageScreenshots && _driver.WrappedDriver is ChromeDriver)
                {
                    try
                    {
                        ss = ScreenshotHelper.TakeFullPageScreenshot(_driver.WrappedDriver as ChromeDriver);
                    }
                    catch (Exception sse)
                    {
                        TestContext.WriteLine("Error taking full-page screenshot");
                        TestContext.WriteLine(sse.Message + "\n" + sse.StackTrace);
                        ss = _driver.TakeScreenshot();
                    }
                }
                else
                {
                    ss = _driver.TakeScreenshot();
                }

                ss.SaveAsFile(screenShotPath);

                _context.AddResultFile(screenShotPath);
            }
            catch (Exception)
            {
                // ignored
            }
            */
        }

        private void OnNavigatedForward(object sender, WebDriverNavigationEventArgs e)
        {
            if (_startedStep != null)
                _reporter.EndStep(_startedStep);
            _startedStep = null;
            //EndStep($"Navigate forward to {e.Url}");
        }

        private void OnNavigatingForward(object sender, WebDriverNavigationEventArgs e)
        {
            _startedStep = _reporter.StartStep($"Navigate forward to {e.Url}");
        }

        private void OnNavigatedBack(object sender, WebDriverNavigationEventArgs e)
        {
            if (_startedStep != null)
                _reporter.EndStep(_startedStep);
            _startedStep = null;
            //EndStep($"Navigate back to {e.Url}");
        }

        private void OnNavigatingBack(object sender, WebDriverNavigationEventArgs e)
        {
            _startedStep = _reporter.StartStep($"Navigate back to {e.Url}");
        }

        private void OnNavigated(object sender, WebDriverNavigationEventArgs e)
        {
            long? loadEvent = null;
            long? domContentLoadedEvent = null;

            try
            {
                loadEvent = _driver.ExecuteJavaScript<long>("return (window.performance.timing.loadEventStart - window.performance.timing.navigationStart)");
                domContentLoadedEvent = _driver.ExecuteJavaScript<long>("return (window.performance.timing.domContentLoadedEventStart - window.performance.timing.navigationStart)");

                if (loadEvent < 0)
                {
                    loadEvent = null;
                }

                if (domContentLoadedEvent < 0)
                {
                    domContentLoadedEvent = null;
                }
            }
            catch { }
            if (_startedStep != null)
            {
                _reporter.EndStep(_startedStep);
                // TODO: add loadEvent and domContentLoadedEvent to the step result
            }

            _startedStep = null;
            //EndStep($"Navigate to {e.Url}", loadEvent: loadEvent, domContentLoadedEvent: domContentLoadedEvent);
        }

        private void OnNavigating(object sender, WebDriverNavigationEventArgs e)
        {
            _startedStep = _reporter.StartStep($"Navigate to {e.Url}");
        }

        private void OnFindElementCompleted(object sender, FindElementEventArgs e)
        {
            if (_startedStep != null)
                _reporter.EndStep(_startedStep);
            _startedStep = null;
            //EndStep($"Finding element {e.FindMethod}");
        }

        private void OnFindingElement(object sender, FindElementEventArgs e)
        {
            _startedStep = _reporter.StartStep($"Finding element {e.FindMethod}");
        }

        private void OnElementValueChanged(object sender, WebElementValueEventArgs e)
        {
            if (_startedStep != null)
                _reporter.EndStep(_startedStep);
            _startedStep = null;
            //EndStep($"Changing element value to {e.Value}");
        }

        private void OnElementValueChanging(object sender, WebElementValueEventArgs e)
        {
            _startedStep = _reporter.StartStep($"Changing element value to {e.Value}");
        }
    }
}

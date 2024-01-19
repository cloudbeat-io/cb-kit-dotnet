using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CloudBeat.Kit.Common;
using Microsoft.Playwright;


namespace CloudBeat.Kit.Playwright
{
	public class CbPageWrapper : IPage
	{
        readonly IPage page;
        readonly CbTestReporter reporter;


        public CbPageWrapper(IPage page, CbTestReporter reporter)
		{
            this.page = page;
            this.reporter = reporter;
		}

        [Obsolete]
        IAccessibility IPage.Accessibility => page.Accessibility;

        IBrowserContext IPage.Context => page.Context;

        IReadOnlyList<IFrame> IPage.Frames => page.Frames;

        bool IPage.IsClosed => page.IsClosed;

        IKeyboard IPage.Keyboard => page.Keyboard;

        IFrame IPage.MainFrame => page.MainFrame;

        IMouse IPage.Mouse => page.Mouse;

        IAPIRequestContext IPage.APIRequest => page.APIRequest;

        ITouchscreen IPage.Touchscreen => page.Touchscreen;

        string IPage.Url => page.Url;

        IVideo IPage.Video => page.Video;

        PageViewportSizeResult IPage.ViewportSize => page.ViewportSize;

        IReadOnlyList<IWorker> IPage.Workers => page.Workers;

        event EventHandler<IPage> IPage.Close
        {
            add
            {
                page.Close += value;
            }

            remove
            {
                page.Close -= value;
            }
        }

        event EventHandler<IConsoleMessage> IPage.Console
        {
            add
            {
                page.Console += value;
            }

            remove
            {
                page.Console -= value;
            }
        }

        event EventHandler<IPage> IPage.Crash
        {
            add
            {
                page.Crash += value;
            }

            remove
            {
                page.Crash -= value;
            }
        }

        event EventHandler<IDialog> IPage.Dialog
        {
            add
            {
                page.Dialog += value;
            }

            remove
            {
                page.Dialog -= value;
            }
        }

        event EventHandler<IPage> IPage.DOMContentLoaded
        {
            add
            {
                page.DOMContentLoaded += value;
            }

            remove
            {
                page.DOMContentLoaded -= value;
            }
        }

        event EventHandler<IDownload> IPage.Download
        {
            add
            {
                page.Download += value;
            }

            remove
            {
                page.Download -= value;
            }
        }

        event EventHandler<IFileChooser> IPage.FileChooser
        {
            add
            {
                page.FileChooser += value;
            }

            remove
            {
                page.FileChooser -= value;
            }
        }

        event EventHandler<IFrame> IPage.FrameAttached
        {
            add
            {
                page.FrameAttached += value;
            }

            remove
            {
                page.FrameAttached -= value;
            }
        }

        event EventHandler<IFrame> IPage.FrameDetached
        {
            add
            {
                page.FrameDetached += value;
            }

            remove
            {
                page.FrameDetached -= value;
            }
        }

        event EventHandler<IFrame> IPage.FrameNavigated
        {
            add
            {
                page.FrameNavigated += value;
            }

            remove
            {
                page.FrameNavigated -= value;
            }
        }

        event EventHandler<IPage> IPage.Load
        {
            add
            {
                page.Load += value;
            }

            remove
            {
                page.Load -= value;
            }
        }

        event EventHandler<string> IPage.PageError
        {
            add
            {
                page.PageError += value;
            }

            remove
            {
                page.PageError -= value;
            }
        }

        event EventHandler<IPage> IPage.Popup
        {
            add
            {
                page.Popup += value;
            }

            remove
            {
                page.Popup -= value;
            }
        }

        event EventHandler<IRequest> IPage.Request
        {
            add
            {
                page.Request += value;
            }

            remove
            {
                page.Request += value;
            }
        }

        event EventHandler<IRequest> IPage.RequestFailed
        {
            add
            {
                page.RequestFailed += value;
            }

            remove
            {
                page.RequestFailed -= value;
            }
        }

        event EventHandler<IRequest> IPage.RequestFinished
        {
            add
            {
                page.RequestFinished += value;
            }

            remove
            {
                page.RequestFinished -= value;
            }
        }

        event EventHandler<IResponse> IPage.Response
        {
            add
            {
                page.Response += value;
            }

            remove
            {
                page.Response -= value;
            }
        }

        event EventHandler<IWebSocket> IPage.WebSocket
        {
            add
            {
                page.WebSocket += value;
            }

            remove
            {
                page.WebSocket -= value;
            }
        }

        event EventHandler<IWorker> IPage.Worker
        {
            add
            {
                page.Worker += value;
            }

            remove
            {
                page.Worker -= value;
            }
        }

        Task IPage.AddInitScriptAsync(string script, string scriptPath)
        {
            return page.AddInitScriptAsync(script, scriptPath);
        }

        Task<IElementHandle> IPage.AddScriptTagAsync(PageAddScriptTagOptions options)
        {
            return page.AddScriptTagAsync(options);
        }

        Task<IElementHandle> IPage.AddStyleTagAsync(PageAddStyleTagOptions options)
        {
            return page.AddStyleTagAsync(options);
        }

        Task IPage.BringToFrontAsync()
        {
            return page.BringToFrontAsync();
        }

        Task IPage.CheckAsync(string selector, PageCheckOptions options)
        {
            return page.CheckAsync(selector, options);
        }

        // FIXME: ClickAsync is DISCOURAGED
        Task IPage.ClickAsync(string selector, PageClickOptions options)
        {
            var step = reporter?.StartStep($"Click on {selector}");
            var task = page.ClickAsync(selector, options);
            task.GetAwaiter().OnCompleted(() =>
            {
                step?.End();
            });
            return task;
        }

        Task IPage.CloseAsync(PageCloseOptions options)
        {
            return page.CloseAsync();
        }

        Task<string> IPage.ContentAsync()
        {
            return page.ContentAsync();
        }

        Task IPage.DblClickAsync(string selector, PageDblClickOptions options)
        {
            return page.DblClickAsync(selector, options);
        }

        Task IPage.DispatchEventAsync(string selector, string type, object eventInit, PageDispatchEventOptions options)
        {
            return page.DispatchEventAsync(selector, type, eventInit, options);
        }

        Task IPage.DragAndDropAsync(string source, string target, PageDragAndDropOptions options)
            => page.DragAndDropAsync(source, target, options);

        Task IPage.EmulateMediaAsync(PageEmulateMediaOptions options) => page.EmulateMediaAsync(options);

        Task<T> IPage.EvalOnSelectorAllAsync<T>(string selector, string expression, object arg)
            => page.EvalOnSelectorAllAsync<T>(selector, expression, arg);

        Task<JsonElement?> IPage.EvalOnSelectorAllAsync(string selector, string expression, object arg)
        {
            return page.EvalOnSelectorAllAsync(selector, expression, arg);
        }

        Task<T> IPage.EvalOnSelectorAsync<T>(string selector, string expression, object arg, PageEvalOnSelectorOptions options)
        {
            return page.EvalOnSelectorAsync<T>(selector, expression, arg, options);
        }

        Task<JsonElement?> IPage.EvalOnSelectorAsync(string selector, string expression, object arg)
        {
            return page.EvalOnSelectorAsync(selector, expression, arg);
        }

        Task<T> IPage.EvaluateAsync<T>(string expression, object arg)
        {
            return page.EvaluateAsync<T>(expression, arg);
        }

        Task<JsonElement?> IPage.EvaluateAsync(string expression, object arg)
        {
            return page.EvaluateAsync(expression, arg);
        }

        Task<IJSHandle> IPage.EvaluateHandleAsync(string expression, object arg)
        {
            return page.EvaluateHandleAsync(expression, arg);
        }

        Task IPage.ExposeBindingAsync(string name, Action callback, PageExposeBindingOptions options)
        {
            return page.ExposeBindingAsync(name, callback, options);
        }

        Task IPage.ExposeBindingAsync(string name, Action<BindingSource> callback)
        {
            return page.ExposeBindingAsync(name, callback);
        }

        Task IPage.ExposeBindingAsync<T>(string name, Action<BindingSource, T> callback)
        {
            return page.ExposeBindingAsync<T>(name, callback);
        }

        Task IPage.ExposeBindingAsync<TResult>(string name, Func<BindingSource, TResult> callback)
        {
            return page.ExposeBindingAsync<TResult>(name, callback);
        }

        Task IPage.ExposeBindingAsync<TResult>(string name, Func<BindingSource, IJSHandle, TResult> callback)
        {
            return page.ExposeBindingAsync<TResult>(name, callback);
        }

        Task IPage.ExposeBindingAsync<T, TResult>(string name, Func<BindingSource, T, TResult> callback)
        {
            return page.ExposeBindingAsync<T, TResult>(name, callback);
        }

        Task IPage.ExposeBindingAsync<T1, T2, TResult>(string name, Func<BindingSource, T1, T2, TResult> callback)
        {
            return page.ExposeBindingAsync<T1, T2, TResult>(name, callback);
        }

        Task IPage.ExposeBindingAsync<T1, T2, T3, TResult>(string name, Func<BindingSource, T1, T2, T3, TResult> callback)
        {
            return page.ExposeBindingAsync<T1, T2, T3, TResult>(name, callback);
        }

        Task IPage.ExposeBindingAsync<T1, T2, T3, T4, TResult>(string name, Func<BindingSource, T1, T2, T3, T4, TResult> callback)
        {
            return page.ExposeBindingAsync<T1, T2, T3, T4, TResult>(name, callback);
        }

        Task IPage.ExposeFunctionAsync(string name, Action callback)
        {
            return page.ExposeFunctionAsync(name, callback);
        }

        Task IPage.ExposeFunctionAsync<T>(string name, Action<T> callback)
        {
            return page.ExposeFunctionAsync<T>(name, callback);
        }

        Task IPage.ExposeFunctionAsync<TResult>(string name, Func<TResult> callback)
        {
            return page.ExposeFunctionAsync<TResult>(name, callback);
        }

        Task IPage.ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> callback)
        {
            return page.ExposeFunctionAsync<T, TResult>(name, callback);
        }

        Task IPage.ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> callback)
        {
            return page.ExposeFunctionAsync<T1, T2, TResult>(name, callback);
        }

        Task IPage.ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> callback)
        {
            return page.ExposeFunctionAsync<T1, T2, T3, TResult>(name, callback);
        }

        Task IPage.ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> callback)
        {
            return page.ExposeFunctionAsync<T1, T2, T3, T4, TResult>(name, callback);
        }

        Task IPage.FillAsync(string selector, string value, PageFillOptions options)
        {
            return page.FillAsync(selector, value, options);
        }

        Task IPage.FocusAsync(string selector, PageFocusOptions options)
        {
            return page.FocusAsync(selector, options);
        }

        IFrame IPage.Frame(string name)
        {
            return page.Frame(name);
        }

        IFrame IPage.FrameByUrl(string url)
        {
            return page.FrameByUrl(url);
        }

        IFrame IPage.FrameByUrl(Regex url)
        {
            return page.FrameByUrl(url);
        }

        IFrame IPage.FrameByUrl(Func<string, bool> url)
        {
            return page.FrameByUrl(url);
        }

        IFrameLocator IPage.FrameLocator(string selector)
        {
            return page.FrameLocator(selector);
        }

        Task<string> IPage.GetAttributeAsync(string selector, string name, PageGetAttributeOptions options)
        {
            return page.GetAttributeAsync(selector, name, options);
        }

        ILocator IPage.GetByAltText(string text, PageGetByAltTextOptions options)
        {
            return new CbLocatorWrapper(page.GetByAltText(text, options), reporter);
        }

        ILocator IPage.GetByAltText(Regex text, PageGetByAltTextOptions options)
        {
            return new CbLocatorWrapper(page.GetByAltText(text, options), reporter);
        }

        ILocator IPage.GetByLabel(string text, PageGetByLabelOptions options)
        {
            return new CbLocatorWrapper(page.GetByLabel(text, options), reporter);
        }

        ILocator IPage.GetByLabel(Regex text, PageGetByLabelOptions options)
        {
            return new CbLocatorWrapper(page.GetByLabel(text, options), reporter);
        }

        ILocator IPage.GetByPlaceholder(string text, PageGetByPlaceholderOptions options)
        {
            return new CbLocatorWrapper(page.GetByPlaceholder(text, options), reporter);
        }

        ILocator IPage.GetByPlaceholder(Regex text, PageGetByPlaceholderOptions options)
        {
            return new CbLocatorWrapper(page.GetByPlaceholder(text, options), reporter);
        }

        ILocator IPage.GetByRole(AriaRole role, PageGetByRoleOptions options)
        {
            return new CbLocatorWrapper(page.GetByRole(role, options), reporter);
        }

        ILocator IPage.GetByTestId(string testId)
        {
            return new CbLocatorWrapper(page.GetByTestId(testId), reporter);
        }

        ILocator IPage.GetByTestId(Regex testId)
        {
            return new CbLocatorWrapper(page.GetByTestId(testId), reporter);
        }

        ILocator IPage.GetByText(string text, PageGetByTextOptions options)
        {
            return new CbLocatorWrapper(page.GetByText(text, options), reporter);
        }

        ILocator IPage.GetByText(Regex text, PageGetByTextOptions options)
        {
            return new CbLocatorWrapper(page.GetByText(text, options), reporter);
        }

        ILocator IPage.GetByTitle(string text, PageGetByTitleOptions options)
        {
            return new CbLocatorWrapper(page.GetByTitle(text, options), reporter);
        }

        ILocator IPage.GetByTitle(Regex text, PageGetByTitleOptions options)
        {
            return new CbLocatorWrapper(page.GetByTitle(text, options), reporter);
        }

        Task<IResponse> IPage.GoBackAsync(PageGoBackOptions options)
        {
            var step = reporter?.StartStep("Navigate back");
            var task = page.GoBackAsync(options);
            task.GetAwaiter().OnCompleted(() =>
            {
                step?.End();
            });
            return task;
        }

        Task<IResponse> IPage.GoForwardAsync(PageGoForwardOptions options)
        {
            var step = reporter?.StartStep("Navigate forward");
            var task = page.GoForwardAsync(options);
            task.GetAwaiter().OnCompleted(() =>
            {
                step?.End();
            });
            return task;
        }

        Task<IResponse> IPage.GotoAsync(string url, PageGotoOptions options)
        {
            var step = reporter?.StartStep($"Navigate to {url}");
            var task = page.GotoAsync(url, options);
            task.GetAwaiter().OnCompleted(() =>
            {
                step?.End();
            });
            return task;
        }

        Task IPage.HoverAsync(string selector, PageHoverOptions options)
        {
            return page.HoverAsync(selector, options);
        }

        Task<string> IPage.InnerHTMLAsync(string selector, PageInnerHTMLOptions options)
        {
            return page.InnerHTMLAsync(selector, options);
        }

        Task<string> IPage.InnerTextAsync(string selector, PageInnerTextOptions options)
        {
            return page.InnerTextAsync(selector, options);
        }

        Task<string> IPage.InputValueAsync(string selector, PageInputValueOptions options)
        {
            return page.InputValueAsync(selector, options);
        }

        Task<bool> IPage.IsCheckedAsync(string selector, PageIsCheckedOptions options)
        {
            return page.IsCheckedAsync(selector, options);
        }

        Task<bool> IPage.IsDisabledAsync(string selector, PageIsDisabledOptions options)
        {
            return page.IsDisabledAsync(selector, options);
        }

        Task<bool> IPage.IsEditableAsync(string selector, PageIsEditableOptions options)
        {
            return page.IsEditableAsync(selector, options);
        }

        Task<bool> IPage.IsEnabledAsync(string selector, PageIsEnabledOptions options)
        {
            return page.IsEnabledAsync(selector, options);
        }

        Task<bool> IPage.IsHiddenAsync(string selector, PageIsHiddenOptions options)
        {
            return page.IsHiddenAsync(selector, options);
        }

        Task<bool> IPage.IsVisibleAsync(string selector, PageIsVisibleOptions options)
        {
            return page.IsVisibleAsync(selector, options);
        }

        ILocator IPage.Locator(string selector, PageLocatorOptions options)
        {
            // TODO: shall we add this as "find element" step or ignore it?
            var locator = page.Locator(selector, options);
            if (locator != null)
                return new CbLocatorWrapper(locator, reporter);
            return null;
        }

        Task<IPage> IPage.OpenerAsync()
        {
            return page.OpenerAsync();
        }

        Task IPage.PauseAsync()
        {
            return page.PauseAsync();
        }

        Task<byte[]> IPage.PdfAsync(PagePdfOptions options)
        {
            return page.PdfAsync(options);
        }

        Task IPage.PressAsync(string selector, string key, PagePressOptions options)
        {
            return page.PressAsync(selector, key, options);
        }

        Task<IReadOnlyList<IElementHandle>> IPage.QuerySelectorAllAsync(string selector)
        {
            return page.QuerySelectorAllAsync(selector);
        }

        Task<IElementHandle> IPage.QuerySelectorAsync(string selector, PageQuerySelectorOptions options)
        {
            return page.QuerySelectorAsync(selector, options);
        }

        Task<IResponse> IPage.ReloadAsync(PageReloadOptions options)
        {
            return page.ReloadAsync(options);
        }

        Task IPage.RouteAsync(string url, Action<IRoute> handler, PageRouteOptions options)
        {
            return page.RouteAsync(url, handler, options);
        }

        Task IPage.RouteAsync(Regex url, Action<IRoute> handler, PageRouteOptions options)
        {
            return page.RouteAsync(url, handler, options);
        }

        Task IPage.RouteAsync(Func<string, bool> url, Action<IRoute> handler, PageRouteOptions options)
        {
            return page.RouteAsync(url, handler, options);
        }

        Task IPage.RouteAsync(string url, Func<IRoute, Task> handler, PageRouteOptions options)
        {
            return page.RouteAsync(url, handler, options);
        }

        Task IPage.RouteAsync(Regex url, Func<IRoute, Task> handler, PageRouteOptions options)
        {
            return page.RouteAsync(url, handler, options);
        }

        Task IPage.RouteAsync(Func<string, bool> url, Func<IRoute, Task> handler, PageRouteOptions options)
        {
            return page.RouteAsync(url, handler, options);
        }

        Task IPage.RouteFromHARAsync(string har, PageRouteFromHAROptions options)
        {
            return page.RouteFromHARAsync(har, options);
        }

        Task<IConsoleMessage> IPage.RunAndWaitForConsoleMessageAsync(Func<Task> action, PageRunAndWaitForConsoleMessageOptions options)
        {
            return page.RunAndWaitForConsoleMessageAsync(action, options);
        }

        Task<IDownload> IPage.RunAndWaitForDownloadAsync(Func<Task> action, PageRunAndWaitForDownloadOptions options)
        {
            return page.RunAndWaitForDownloadAsync(action, options);
        }

        Task<IFileChooser> IPage.RunAndWaitForFileChooserAsync(Func<Task> action, PageRunAndWaitForFileChooserOptions options)
        {
            var step = reporter?.StartStep("Run and wait for file chooser");
            var task = page.RunAndWaitForFileChooserAsync(action, options);
            task.GetAwaiter().OnCompleted(() =>
            {
                step?.End();
            });
            return task;
        }

        [Obsolete]
        Task<IResponse> IPage.RunAndWaitForNavigationAsync(Func<Task> action, PageRunAndWaitForNavigationOptions options)
        {
            return page.RunAndWaitForNavigationAsync(action, options);
        }

        Task<IPage> IPage.RunAndWaitForPopupAsync(Func<Task> action, PageRunAndWaitForPopupOptions options)
        {
            return page.RunAndWaitForPopupAsync(action, options);
        }

        Task<IRequest> IPage.RunAndWaitForRequestAsync(Func<Task> action, string urlOrPredicate, PageRunAndWaitForRequestOptions options)
        {
            return page.RunAndWaitForRequestAsync(action, urlOrPredicate, options);
        }

        Task<IRequest> IPage.RunAndWaitForRequestAsync(Func<Task> action, Regex urlOrPredicate, PageRunAndWaitForRequestOptions options)
        {
            return page.RunAndWaitForRequestAsync(action, urlOrPredicate, options);
        }

        Task<IRequest> IPage.RunAndWaitForRequestAsync(Func<Task> action, Func<IRequest, bool> urlOrPredicate, PageRunAndWaitForRequestOptions options)
        {
            return page.RunAndWaitForRequestAsync(action, urlOrPredicate, options);
        }

        Task<IRequest> IPage.RunAndWaitForRequestFinishedAsync(Func<Task> action, PageRunAndWaitForRequestFinishedOptions options)
        {
            return page.RunAndWaitForRequestFinishedAsync(action, options);
        }

        Task<IResponse> IPage.RunAndWaitForResponseAsync(Func<Task> action, string urlOrPredicate, PageRunAndWaitForResponseOptions options)
        {
            return page.RunAndWaitForResponseAsync(action, urlOrPredicate, options);
        }

        Task<IResponse> IPage.RunAndWaitForResponseAsync(Func<Task> action, Regex urlOrPredicate, PageRunAndWaitForResponseOptions options)
        {
            return page.RunAndWaitForResponseAsync(action, urlOrPredicate, options);
        }

        Task<IResponse> IPage.RunAndWaitForResponseAsync(Func<Task> action, Func<IResponse, bool> urlOrPredicate, PageRunAndWaitForResponseOptions options)
        {
            var step = reporter?.StartStep("Run and wait for response");
            var task = page.RunAndWaitForResponseAsync(action, urlOrPredicate, options);
            task.GetAwaiter().OnCompleted(() =>
            {
                step?.End();
            });
            return task;
        }

        Task<IWebSocket> IPage.RunAndWaitForWebSocketAsync(Func<Task> action, PageRunAndWaitForWebSocketOptions options)
        {
            return page.RunAndWaitForWebSocketAsync(action, options);
        }

        Task<IWorker> IPage.RunAndWaitForWorkerAsync(Func<Task> action, PageRunAndWaitForWorkerOptions options)
        {
            return page.RunAndWaitForWorkerAsync(action, options);
        }

        Task<byte[]> IPage.ScreenshotAsync(PageScreenshotOptions options)
        {
            return page.ScreenshotAsync(options);
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, string values, PageSelectOptionOptions options)
        {
            return page.SelectOptionAsync(selector, values, options);
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, IElementHandle values, PageSelectOptionOptions options)
        {
            return page.SelectOptionAsync(selector, values, options);
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, IEnumerable<string> values, PageSelectOptionOptions options)
        {
            return page.SelectOptionAsync(selector, values, options);
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, SelectOptionValue values, PageSelectOptionOptions options)
        {
            return page.SelectOptionAsync(selector, values, options);
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, IEnumerable<IElementHandle> values, PageSelectOptionOptions options)
        {
            return page.SelectOptionAsync(selector, values, options);
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, IEnumerable<SelectOptionValue> values, PageSelectOptionOptions options)
        {
            return page.SelectOptionAsync(selector, values, options);
        }

        Task IPage.SetCheckedAsync(string selector, bool checkedState, PageSetCheckedOptions options)
        {
            return page.SetCheckedAsync(selector, checkedState, options);
        }

        Task IPage.SetContentAsync(string html, PageSetContentOptions options)
        {
            return page.SetContentAsync(html, options);
        }

        void IPage.SetDefaultNavigationTimeout(float timeout)
        {
            page.SetDefaultNavigationTimeout(timeout);
        }

        void IPage.SetDefaultTimeout(float timeout)
        {
            page.SetDefaultTimeout(timeout);
        }

        Task IPage.SetExtraHTTPHeadersAsync(IEnumerable<KeyValuePair<string, string>> headers)
        {
            return page.SetExtraHTTPHeadersAsync(headers);
        }

        Task IPage.SetInputFilesAsync(string selector, string files, PageSetInputFilesOptions options)
        {
            return page.SetInputFilesAsync(selector, files, options);
        }

        Task IPage.SetInputFilesAsync(string selector, IEnumerable<string> files, PageSetInputFilesOptions options)
        {
            return page.SetInputFilesAsync(selector, files, options);
        }

        Task IPage.SetInputFilesAsync(string selector, FilePayload files, PageSetInputFilesOptions options)
        {
            return page.SetInputFilesAsync(selector, files, options);
        }

        Task IPage.SetInputFilesAsync(string selector, IEnumerable<FilePayload> files, PageSetInputFilesOptions options)
        {
            return page.SetInputFilesAsync(selector, files, options);
        }

        Task IPage.SetViewportSizeAsync(int width, int height)
        {
            return page.SetViewportSizeAsync(width, height);
        }

        Task IPage.TapAsync(string selector, PageTapOptions options)
        {
            return page.TapAsync(selector, options);
        }

        Task<string> IPage.TextContentAsync(string selector, PageTextContentOptions options)
        {
            return page.TextContentAsync(selector, options);
        }

        Task<string> IPage.TitleAsync()
        {
            return page.TitleAsync();
        }

        [Obsolete]
        Task IPage.TypeAsync(string selector, string text, PageTypeOptions options)
        {
            var step = reporter?.StartStep($"Type \"{text}\" into {selector}");
            var task = page.TypeAsync(selector, text, options);
            task.GetAwaiter().OnCompleted(() =>
            {
                step?.End();
            });
            return task;
        }

        Task IPage.UncheckAsync(string selector, PageUncheckOptions options)
        {
            return page.UncheckAsync(selector, options);
        }

        Task IPage.UnrouteAsync(string url, Action<IRoute> handler)
        {
            return page.UnrouteAsync(url, handler);
        }

        Task IPage.UnrouteAsync(Regex url, Action<IRoute> handler)
        {
            return page.UnrouteAsync(url, handler);
        }

        Task IPage.UnrouteAsync(Func<string, bool> url, Action<IRoute> handler)
        {
            return page.UnrouteAsync(url, handler);
        }

        Task IPage.UnrouteAsync(string url, Func<IRoute, Task> handler)
        {
            return page.UnrouteAsync(url, handler);
        }

        Task IPage.UnrouteAsync(Regex url, Func<IRoute, Task> handler)
        {
            return page.UnrouteAsync(url, handler);
        }

        Task IPage.UnrouteAsync(Func<string, bool> url, Func<IRoute, Task> handler)
        {
            return page.UnrouteAsync(url, handler);
        }

        Task<IConsoleMessage> IPage.WaitForConsoleMessageAsync(PageWaitForConsoleMessageOptions options)
        {
            return page.WaitForConsoleMessageAsync(options);
        }

        Task<IDownload> IPage.WaitForDownloadAsync(PageWaitForDownloadOptions options)
        {
            return page.WaitForDownloadAsync(options);
        }

        Task<IFileChooser> IPage.WaitForFileChooserAsync(PageWaitForFileChooserOptions options)
        {
            return page.WaitForFileChooserAsync(options);
        }

        Task<IJSHandle> IPage.WaitForFunctionAsync(string expression, object arg, PageWaitForFunctionOptions options)
        {
            return page.WaitForFunctionAsync(expression, arg, options);
        }

        Task IPage.WaitForLoadStateAsync(LoadState? state, PageWaitForLoadStateOptions options)
        {
            return page.WaitForLoadStateAsync(state, options);
        }

        [Obsolete]
        Task<IResponse> IPage.WaitForNavigationAsync(PageWaitForNavigationOptions options)
        {
            return page.WaitForNavigationAsync(options);
        }

        Task<IPage> IPage.WaitForPopupAsync(PageWaitForPopupOptions options)
        {
            return page.WaitForPopupAsync(options);
        }

        Task<IRequest> IPage.WaitForRequestAsync(string urlOrPredicate, PageWaitForRequestOptions options)
        {
            return page.WaitForRequestAsync(urlOrPredicate, options);
        }

        Task<IRequest> IPage.WaitForRequestAsync(Regex urlOrPredicate, PageWaitForRequestOptions options)
        {
            return page.WaitForRequestAsync(urlOrPredicate, options);
        }

        Task<IRequest> IPage.WaitForRequestAsync(Func<IRequest, bool> urlOrPredicate, PageWaitForRequestOptions options)
        {
            return page.WaitForRequestAsync(urlOrPredicate, options);
        }

        Task<IRequest> IPage.WaitForRequestFinishedAsync(PageWaitForRequestFinishedOptions options)
        {
            return page.WaitForRequestFinishedAsync(options);
        }

        Task<IResponse> IPage.WaitForResponseAsync(string urlOrPredicate, PageWaitForResponseOptions options)
        {
            return page.WaitForResponseAsync(urlOrPredicate, options);
        }

        Task<IResponse> IPage.WaitForResponseAsync(Regex urlOrPredicate, PageWaitForResponseOptions options)
        {
            return page.WaitForResponseAsync(urlOrPredicate, options);
        }

        Task<IResponse> IPage.WaitForResponseAsync(Func<IResponse, bool> urlOrPredicate, PageWaitForResponseOptions options)
        {
            return page.WaitForResponseAsync(urlOrPredicate, options);
        }

        Task<IElementHandle> IPage.WaitForSelectorAsync(string selector, PageWaitForSelectorOptions options)
        {
            return page.WaitForSelectorAsync(selector, options);
        }

        Task IPage.WaitForTimeoutAsync(float timeout)
        {
            return page.WaitForTimeoutAsync(timeout);
        }

        Task IPage.WaitForURLAsync(string url, PageWaitForURLOptions options)
        {
            return page.WaitForURLAsync(url, options);
        }

        Task IPage.WaitForURLAsync(Regex url, PageWaitForURLOptions options)
        {
            return page.WaitForURLAsync(url, options);
        }

        Task IPage.WaitForURLAsync(Func<string, bool> url, PageWaitForURLOptions options)
        {
            return page.WaitForURLAsync(url, options);
        }

        Task<IWebSocket> IPage.WaitForWebSocketAsync(PageWaitForWebSocketOptions options)
        {
            return page.WaitForWebSocketAsync(options);
        }

        Task<IWorker> IPage.WaitForWorkerAsync(PageWaitForWorkerOptions options)
        {
            return page.WaitForWorkerAsync(options);
        }
    }
}


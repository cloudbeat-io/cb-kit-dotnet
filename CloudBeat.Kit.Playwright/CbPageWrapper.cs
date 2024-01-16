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

        Task IPage.ClickAsync(string selector, PageClickOptions options)
        {
            var step = reporter.StartStep($"Click on {selector}");
            var task = page.ClickAsync(selector, options);
            task.GetAwaiter().OnCompleted(() =>
            {
                step.End();
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
            throw new NotImplementedException();
        }

        Task<IJSHandle> IPage.EvaluateHandleAsync(string expression, object arg)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeBindingAsync(string name, Action callback, PageExposeBindingOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeBindingAsync(string name, Action<BindingSource> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeBindingAsync<T>(string name, Action<BindingSource, T> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeBindingAsync<TResult>(string name, Func<BindingSource, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeBindingAsync<TResult>(string name, Func<BindingSource, IJSHandle, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeBindingAsync<T, TResult>(string name, Func<BindingSource, T, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeBindingAsync<T1, T2, TResult>(string name, Func<BindingSource, T1, T2, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeBindingAsync<T1, T2, T3, TResult>(string name, Func<BindingSource, T1, T2, T3, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeBindingAsync<T1, T2, T3, T4, TResult>(string name, Func<BindingSource, T1, T2, T3, T4, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeFunctionAsync(string name, Action callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeFunctionAsync<T>(string name, Action<T> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeFunctionAsync<TResult>(string name, Func<TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> callback)
        {
            throw new NotImplementedException();
        }

        Task IPage.FillAsync(string selector, string value, PageFillOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.FocusAsync(string selector, PageFocusOptions options)
        {
            throw new NotImplementedException();
        }

        IFrame IPage.Frame(string name)
        {
            throw new NotImplementedException();
        }

        IFrame IPage.FrameByUrl(string url)
        {
            throw new NotImplementedException();
        }

        IFrame IPage.FrameByUrl(Regex url)
        {
            throw new NotImplementedException();
        }

        IFrame IPage.FrameByUrl(Func<string, bool> url)
        {
            throw new NotImplementedException();
        }

        IFrameLocator IPage.FrameLocator(string selector)
        {
            throw new NotImplementedException();
        }

        Task<string> IPage.GetAttributeAsync(string selector, string name, PageGetAttributeOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByAltText(string text, PageGetByAltTextOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByAltText(Regex text, PageGetByAltTextOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByLabel(string text, PageGetByLabelOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByLabel(Regex text, PageGetByLabelOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByPlaceholder(string text, PageGetByPlaceholderOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByPlaceholder(Regex text, PageGetByPlaceholderOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByRole(AriaRole role, PageGetByRoleOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByTestId(string testId)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByTestId(Regex testId)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByText(string text, PageGetByTextOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByText(Regex text, PageGetByTextOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByTitle(string text, PageGetByTitleOptions options)
        {
            throw new NotImplementedException();
        }

        ILocator IPage.GetByTitle(Regex text, PageGetByTitleOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.GoBackAsync(PageGoBackOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.GoForwardAsync(PageGoForwardOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.GotoAsync(string url, PageGotoOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.HoverAsync(string selector, PageHoverOptions options)
        {
            throw new NotImplementedException();
        }

        Task<string> IPage.InnerHTMLAsync(string selector, PageInnerHTMLOptions options)
        {
            throw new NotImplementedException();
        }

        Task<string> IPage.InnerTextAsync(string selector, PageInnerTextOptions options)
        {
            throw new NotImplementedException();
        }

        Task<string> IPage.InputValueAsync(string selector, PageInputValueOptions options)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPage.IsCheckedAsync(string selector, PageIsCheckedOptions options)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPage.IsDisabledAsync(string selector, PageIsDisabledOptions options)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPage.IsEditableAsync(string selector, PageIsEditableOptions options)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPage.IsEnabledAsync(string selector, PageIsEnabledOptions options)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPage.IsHiddenAsync(string selector, PageIsHiddenOptions options)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPage.IsVisibleAsync(string selector, PageIsVisibleOptions options)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        Task IPage.PauseAsync()
        {
            throw new NotImplementedException();
        }

        Task<byte[]> IPage.PdfAsync(PagePdfOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.PressAsync(string selector, string key, PagePressOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<IElementHandle>> IPage.QuerySelectorAllAsync(string selector)
        {
            throw new NotImplementedException();
        }

        Task<IElementHandle> IPage.QuerySelectorAsync(string selector, PageQuerySelectorOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.ReloadAsync(PageReloadOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.RouteAsync(string url, Action<IRoute> handler, PageRouteOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.RouteAsync(Regex url, Action<IRoute> handler, PageRouteOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.RouteAsync(Func<string, bool> url, Action<IRoute> handler, PageRouteOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.RouteAsync(string url, Func<IRoute, Task> handler, PageRouteOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.RouteAsync(Regex url, Func<IRoute, Task> handler, PageRouteOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.RouteAsync(Func<string, bool> url, Func<IRoute, Task> handler, PageRouteOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.RouteFromHARAsync(string har, PageRouteFromHAROptions options)
        {
            throw new NotImplementedException();
        }

        Task<IConsoleMessage> IPage.RunAndWaitForConsoleMessageAsync(Func<Task> action, PageRunAndWaitForConsoleMessageOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IDownload> IPage.RunAndWaitForDownloadAsync(Func<Task> action, PageRunAndWaitForDownloadOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IFileChooser> IPage.RunAndWaitForFileChooserAsync(Func<Task> action, PageRunAndWaitForFileChooserOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.RunAndWaitForNavigationAsync(Func<Task> action, PageRunAndWaitForNavigationOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IPage> IPage.RunAndWaitForPopupAsync(Func<Task> action, PageRunAndWaitForPopupOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IRequest> IPage.RunAndWaitForRequestAsync(Func<Task> action, string urlOrPredicate, PageRunAndWaitForRequestOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IRequest> IPage.RunAndWaitForRequestAsync(Func<Task> action, Regex urlOrPredicate, PageRunAndWaitForRequestOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IRequest> IPage.RunAndWaitForRequestAsync(Func<Task> action, Func<IRequest, bool> urlOrPredicate, PageRunAndWaitForRequestOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IRequest> IPage.RunAndWaitForRequestFinishedAsync(Func<Task> action, PageRunAndWaitForRequestFinishedOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.RunAndWaitForResponseAsync(Func<Task> action, string urlOrPredicate, PageRunAndWaitForResponseOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.RunAndWaitForResponseAsync(Func<Task> action, Regex urlOrPredicate, PageRunAndWaitForResponseOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.RunAndWaitForResponseAsync(Func<Task> action, Func<IResponse, bool> urlOrPredicate, PageRunAndWaitForResponseOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IWebSocket> IPage.RunAndWaitForWebSocketAsync(Func<Task> action, PageRunAndWaitForWebSocketOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IWorker> IPage.RunAndWaitForWorkerAsync(Func<Task> action, PageRunAndWaitForWorkerOptions options)
        {
            throw new NotImplementedException();
        }

        Task<byte[]> IPage.ScreenshotAsync(PageScreenshotOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, string values, PageSelectOptionOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, IElementHandle values, PageSelectOptionOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, IEnumerable<string> values, PageSelectOptionOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, SelectOptionValue values, PageSelectOptionOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, IEnumerable<IElementHandle> values, PageSelectOptionOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<string>> IPage.SelectOptionAsync(string selector, IEnumerable<SelectOptionValue> values, PageSelectOptionOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.SetCheckedAsync(string selector, bool checkedState, PageSetCheckedOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.SetContentAsync(string html, PageSetContentOptions options)
        {
            throw new NotImplementedException();
        }

        void IPage.SetDefaultNavigationTimeout(float timeout)
        {
            throw new NotImplementedException();
        }

        void IPage.SetDefaultTimeout(float timeout)
        {
            throw new NotImplementedException();
        }

        Task IPage.SetExtraHTTPHeadersAsync(IEnumerable<KeyValuePair<string, string>> headers)
        {
            throw new NotImplementedException();
        }

        Task IPage.SetInputFilesAsync(string selector, string files, PageSetInputFilesOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.SetInputFilesAsync(string selector, IEnumerable<string> files, PageSetInputFilesOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.SetInputFilesAsync(string selector, FilePayload files, PageSetInputFilesOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.SetInputFilesAsync(string selector, IEnumerable<FilePayload> files, PageSetInputFilesOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.SetViewportSizeAsync(int width, int height)
        {
            throw new NotImplementedException();
        }

        Task IPage.TapAsync(string selector, PageTapOptions options)
        {
            throw new NotImplementedException();
        }

        Task<string> IPage.TextContentAsync(string selector, PageTextContentOptions options)
        {
            throw new NotImplementedException();
        }

        Task<string> IPage.TitleAsync()
        {
            throw new NotImplementedException();
        }

        Task IPage.TypeAsync(string selector, string text, PageTypeOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.UncheckAsync(string selector, PageUncheckOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.UnrouteAsync(string url, Action<IRoute> handler)
        {
            throw new NotImplementedException();
        }

        Task IPage.UnrouteAsync(Regex url, Action<IRoute> handler)
        {
            throw new NotImplementedException();
        }

        Task IPage.UnrouteAsync(Func<string, bool> url, Action<IRoute> handler)
        {
            throw new NotImplementedException();
        }

        Task IPage.UnrouteAsync(string url, Func<IRoute, Task> handler)
        {
            throw new NotImplementedException();
        }

        Task IPage.UnrouteAsync(Regex url, Func<IRoute, Task> handler)
        {
            throw new NotImplementedException();
        }

        Task IPage.UnrouteAsync(Func<string, bool> url, Func<IRoute, Task> handler)
        {
            throw new NotImplementedException();
        }

        Task<IConsoleMessage> IPage.WaitForConsoleMessageAsync(PageWaitForConsoleMessageOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IDownload> IPage.WaitForDownloadAsync(PageWaitForDownloadOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IFileChooser> IPage.WaitForFileChooserAsync(PageWaitForFileChooserOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IJSHandle> IPage.WaitForFunctionAsync(string expression, object arg, PageWaitForFunctionOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.WaitForLoadStateAsync(LoadState? state, PageWaitForLoadStateOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.WaitForNavigationAsync(PageWaitForNavigationOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IPage> IPage.WaitForPopupAsync(PageWaitForPopupOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IRequest> IPage.WaitForRequestAsync(string urlOrPredicate, PageWaitForRequestOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IRequest> IPage.WaitForRequestAsync(Regex urlOrPredicate, PageWaitForRequestOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IRequest> IPage.WaitForRequestAsync(Func<IRequest, bool> urlOrPredicate, PageWaitForRequestOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IRequest> IPage.WaitForRequestFinishedAsync(PageWaitForRequestFinishedOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.WaitForResponseAsync(string urlOrPredicate, PageWaitForResponseOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.WaitForResponseAsync(Regex urlOrPredicate, PageWaitForResponseOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IResponse> IPage.WaitForResponseAsync(Func<IResponse, bool> urlOrPredicate, PageWaitForResponseOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IElementHandle> IPage.WaitForSelectorAsync(string selector, PageWaitForSelectorOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.WaitForTimeoutAsync(float timeout)
        {
            throw new NotImplementedException();
        }

        Task IPage.WaitForURLAsync(string url, PageWaitForURLOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.WaitForURLAsync(Regex url, PageWaitForURLOptions options)
        {
            throw new NotImplementedException();
        }

        Task IPage.WaitForURLAsync(Func<string, bool> url, PageWaitForURLOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IWebSocket> IPage.WaitForWebSocketAsync(PageWaitForWebSocketOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IWorker> IPage.WaitForWorkerAsync(PageWaitForWorkerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}


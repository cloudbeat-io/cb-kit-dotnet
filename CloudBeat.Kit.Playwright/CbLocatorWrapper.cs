using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CloudBeat.Kit.Common;
using Microsoft.Playwright;

namespace CloudBeat.Kit.Playwright
{
	public class CbLocatorWrapper : ILocator
    {
        protected readonly ILocator locator;
        protected readonly IPage page;
        protected readonly CbTestReporter reporter;
        protected readonly string label;

        public CbLocatorWrapper(ILocator locator, IPage page, CbTestReporter reporter)
		{
			this.locator = locator;
			this.reporter = reporter;
            try
            {
                label = Helper.GetLocatorLabel(locator, page).Result;
            }
            catch
            {
                label = ToString();
            }
        }

        public ILocator GetBaseLocator()
        {
            return locator;
        }

        public ILocator First => locator.First;

        public ILocator Last => locator.Last;

        public IPage Page => locator.Page;

        public Task<IReadOnlyList<ILocator>> AllAsync()
        {
            return locator.AllAsync();
        }

        public Task<IReadOnlyList<string>> AllInnerTextsAsync()
        {
            return locator.AllInnerTextsAsync();
        }

        public Task<IReadOnlyList<string>> AllTextContentsAsync()
        {
            return locator.AllTextContentsAsync();
        }

        public ILocator And(ILocator locator)
        {
            return this.locator.And(locator is CbLocatorWrapper ? (locator as CbLocatorWrapper).GetBaseLocator() : locator);
        }

        public Task BlurAsync(LocatorBlurOptions options = null)
        {
            return locator.BlurAsync(options);
        }

        public Task<LocatorBoundingBoxResult> BoundingBoxAsync(LocatorBoundingBoxOptions options = null)
        {
            return locator.BoundingBoxAsync(options);
        }

        public Task CheckAsync(LocatorCheckOptions options = null)
        {
            return locator.CheckAsync(options);
        }

        public Task ClearAsync(LocatorClearOptions options = null)
        {
            if (reporter == null)
                return locator.ClearAsync(options);
            var step = reporter?.StartStep($"Clear {locator}");
            var task = locator.ClearAsync(options);
            return Helper.WrapStepTask(task, step, page, reporter);
        }

        public Task ClickAsync(LocatorClickOptions options = null)
        {
            if (reporter == null)
                return locator.ClickAsync();
            var step = reporter.StartStep($"Click on {label}");
            var task = locator.ClickAsync(options);
            return Helper.WrapStepTask(task, step, page, reporter);
        }

        public Task<int> CountAsync()
        {
            return locator.CountAsync();
        }

        public Task DblClickAsync(LocatorDblClickOptions options = null)
        {
            if (reporter == null)
                return locator.DblClickAsync();
            var step = reporter.StartStep($"Double click on {label}");
            var task = locator.DblClickAsync(options);
            return Helper.WrapStepTask(task, step, page, reporter);
        }

        public Task DispatchEventAsync(string type, object eventInit = null, LocatorDispatchEventOptions options = null)
        {
            return locator.DispatchEventAsync(type, eventInit, options);
        }

        public Task DragToAsync(ILocator target, LocatorDragToOptions options = null)
        {
            return locator.DragToAsync(target, options);
        }

        public Task<IElementHandle> ElementHandleAsync(LocatorElementHandleOptions options = null)
        {
            return locator.ElementHandleAsync(options);
        }

        public Task<IReadOnlyList<IElementHandle>> ElementHandlesAsync()
        {
            return locator.ElementHandlesAsync();
        }

        public Task<T> EvaluateAllAsync<T>(string expression, object arg = null)
        {
            return locator.EvaluateAllAsync<T>(expression, arg);
        }

        public Task<T> EvaluateAsync<T>(string expression, object arg = null, LocatorEvaluateOptions options = null)
        {
            return locator.EvaluateAsync<T>(expression, arg, options);
        }

        public Task<JsonElement?> EvaluateAsync(string expression, object arg = null, LocatorEvaluateOptions options = null)
        {
            return locator.EvaluateAsync(expression, arg, options);
        }

        public Task<IJSHandle> EvaluateHandleAsync(string expression, object arg = null, LocatorEvaluateHandleOptions options = null)
        {
            return locator.EvaluateHandleAsync(expression, arg, options);
        }

        public Task FillAsync(string value, LocatorFillOptions options = null)
        {
            if (reporter == null)
                return locator.FillAsync(value, options);
            var step = reporter.StartStep($"Fill {label} with \"{value}\"");
            var task = locator.FillAsync(value, options);
            return Helper.WrapStepTask(task, step, page, reporter);
        }

        public ILocator Filter(LocatorFilterOptions options = null)
        {
            if (options?.Has is CbLocatorWrapper)
            {
                options.Has = (options.Has as CbLocatorWrapper).GetBaseLocator();
            }

            if (options?.HasNot is CbLocatorWrapper)
            {
                options.HasNot = (options.HasNot as CbLocatorWrapper).GetBaseLocator();
            }

            return locator.Filter(options);
        }

        public Task FocusAsync(LocatorFocusOptions options = null)
        {
            return locator.FocusAsync(options);
        }

        public IFrameLocator FrameLocator(string selector)
        {
            return locator.FrameLocator(selector);
        }

        public Task<string> GetAttributeAsync(string name, LocatorGetAttributeOptions options = null)
        {
            return locator.GetAttributeAsync(name, options);
        }

        public ILocator GetByAltText(string text, LocatorGetByAltTextOptions options = null)
        {
            return locator.GetByAltText(text, options);
        }

        public ILocator GetByAltText(Regex text, LocatorGetByAltTextOptions options = null)
        {
            return locator.GetByAltText(text, options);
        }

        public ILocator GetByLabel(string text, LocatorGetByLabelOptions options = null)
        {
            return locator.GetByLabel(text, options);
        }

        public ILocator GetByLabel(Regex text, LocatorGetByLabelOptions options = null)
        {
            return locator.GetByLabel(text, options);
        }

        public ILocator GetByPlaceholder(string text, LocatorGetByPlaceholderOptions options = null)
        {
            return locator.GetByPlaceholder(text, options);
        }

        public ILocator GetByPlaceholder(Regex text, LocatorGetByPlaceholderOptions options = null)
        {
            return locator.GetByPlaceholder(text, options);
        }

        public ILocator GetByRole(AriaRole role, LocatorGetByRoleOptions options = null)
        {
            return locator.GetByRole(role, options);
        }

        public ILocator GetByTestId(string testId)
        {
            return locator.GetByTestId(testId);
        }

        public ILocator GetByTestId(Regex testId)
        {
            return locator.GetByTestId(testId);
        }

        public ILocator GetByText(string text, LocatorGetByTextOptions options = null)
        {
            return locator.GetByText(text, options);
        }

        public ILocator GetByText(Regex text, LocatorGetByTextOptions options = null)
        {
            return locator.GetByText(text, options);
        }

        public ILocator GetByTitle(string text, LocatorGetByTitleOptions options = null)
        {
            return locator.GetByTitle(text, options);
        }

        public ILocator GetByTitle(Regex text, LocatorGetByTitleOptions options = null)
        {
            return locator.GetByTitle(text, options);
        }

        public Task HighlightAsync()
        {
            return locator.HighlightAsync();
        }

        public Task HoverAsync(LocatorHoverOptions options = null)
        {
            return locator.HoverAsync(options);
        }

        public Task<string> InnerHTMLAsync(LocatorInnerHTMLOptions options = null)
        {
            return locator.InnerHTMLAsync(options);
        }

        public Task<string> InnerTextAsync(LocatorInnerTextOptions options = null)
        {
            return locator.InnerTextAsync(options);
        }

        public Task<string> InputValueAsync(LocatorInputValueOptions options = null)
        {
            return locator.InputValueAsync(options);
        }

        public Task<bool> IsCheckedAsync(LocatorIsCheckedOptions options = null)
        {
            return locator.IsCheckedAsync(options);
        }

        public Task<bool> IsDisabledAsync(LocatorIsDisabledOptions options = null)
        {
            return locator.IsDisabledAsync(options);
        }

        public Task<bool> IsEditableAsync(LocatorIsEditableOptions options = null)
        {
            return locator.IsEditableAsync(options);
        }

        public Task<bool> IsEnabledAsync(LocatorIsEnabledOptions options = null)
        {
            return locator.IsEnabledAsync(options);
        }

        public Task<bool> IsHiddenAsync(LocatorIsHiddenOptions options = null)
        {
            return locator.IsHiddenAsync(options);
        }

        public Task<bool> IsVisibleAsync(LocatorIsVisibleOptions options = null)
        {
            return locator.IsVisibleAsync(options);
        }

        public ILocator Locator(string selectorOrLocator, LocatorLocatorOptions options = null)
        {
            if (options?.Has is CbLocatorWrapper)
            {
                options.Has = (options.Has as CbLocatorWrapper).GetBaseLocator();
            }

            if (options?.HasNot is CbLocatorWrapper)
            {
                options.HasNot = (options.HasNot as CbLocatorWrapper).GetBaseLocator();
            }

            return locator.Locator(selectorOrLocator, options);
        }

        public ILocator Locator(ILocator selectorOrLocator, LocatorLocatorOptions options = null)
        {
            if (options?.Has is CbLocatorWrapper)
            {
                options.Has = (options.Has as CbLocatorWrapper).GetBaseLocator();
            }

            if (options?.HasNot is CbLocatorWrapper)
            {
                options.HasNot = (options.HasNot as CbLocatorWrapper).GetBaseLocator();
            }

            return locator.Locator(selectorOrLocator, options);
        }

        public ILocator Nth(int index)
        {
            return locator.Nth(index);
        }

        public ILocator Or(ILocator locator)
        {
            return this.locator.Or(locator is CbLocatorWrapper ? (locator as CbLocatorWrapper).GetBaseLocator() : locator);
        }

        public Task PressAsync(string key, LocatorPressOptions options = null)
        {
            if (reporter == null)
                return locator.PressAsync(key, options);
            var step = reporter.StartStep($"Press {key}");
            var task = locator.PressAsync(key, options);
            return Helper.WrapStepTask(task, step, page, reporter);
        }

        public Task PressSequentiallyAsync(string text, LocatorPressSequentiallyOptions options = null)
        {
            return locator.PressSequentiallyAsync(text, options);
        }

        public Task<byte[]> ScreenshotAsync(LocatorScreenshotOptions options = null)
        {
            return locator.ScreenshotAsync(options);
        }

        public Task ScrollIntoViewIfNeededAsync(LocatorScrollIntoViewIfNeededOptions options = null)
        {
            return locator.ScrollIntoViewIfNeededAsync(options);
        }

        public Task<IReadOnlyList<string>> SelectOptionAsync(string values, LocatorSelectOptionOptions options = null)
        {
            return locator.SelectOptionAsync(values, options);
        }

        public Task<IReadOnlyList<string>> SelectOptionAsync(IElementHandle values, LocatorSelectOptionOptions options = null)
        {
            return locator.SelectOptionAsync(values, options);
        }

        public Task<IReadOnlyList<string>> SelectOptionAsync(IEnumerable<string> values, LocatorSelectOptionOptions options = null)
        {
            return locator.SelectOptionAsync(values, options);
        }

        public Task<IReadOnlyList<string>> SelectOptionAsync(SelectOptionValue values, LocatorSelectOptionOptions options = null)
        {
            return locator.SelectOptionAsync(values, options);
        }

        public Task<IReadOnlyList<string>> SelectOptionAsync(IEnumerable<IElementHandle> values, LocatorSelectOptionOptions options = null)
        {
            return locator.SelectOptionAsync(values, options);
        }

        public Task<IReadOnlyList<string>> SelectOptionAsync(IEnumerable<SelectOptionValue> values, LocatorSelectOptionOptions options = null)
        {
            return locator.SelectOptionAsync(values, options);
        }

        public Task SelectTextAsync(LocatorSelectTextOptions options = null)
        {
            return locator.SelectTextAsync(options);
        }

        public Task SetCheckedAsync(bool checkedState, LocatorSetCheckedOptions options = null)
        {
            return locator.SetCheckedAsync(checkedState, options);
        }

        public Task SetInputFilesAsync(string files, LocatorSetInputFilesOptions options = null)
        {
            return locator.SetInputFilesAsync(files, options);
        }

        public Task SetInputFilesAsync(IEnumerable<string> files, LocatorSetInputFilesOptions options = null)
        {
            return locator.SetInputFilesAsync(files, options);
        }

        public Task SetInputFilesAsync(FilePayload files, LocatorSetInputFilesOptions options = null)
        {
            return locator.SetInputFilesAsync(files, options);
        }

        public Task SetInputFilesAsync(IEnumerable<FilePayload> files, LocatorSetInputFilesOptions options = null)
        {
            return locator.SetInputFilesAsync(files, options);
        }

        public Task TapAsync(LocatorTapOptions options = null)
        {
            return locator.TapAsync(options);
        }

        public Task<string> TextContentAsync(LocatorTextContentOptions options = null)
        {
            return locator.TextContentAsync(options);
        }

        [Obsolete]
        public Task TypeAsync(string text, LocatorTypeOptions options = null)
        {
            if (reporter == null)
                return locator.TypeAsync(text, options);
            var step = reporter.StartStep($"Type \"{text}\" into {label}");
            var task = locator.TypeAsync(text, options);
            return Helper.WrapStepTask(task, step, page, reporter);
        }

        public Task UncheckAsync(LocatorUncheckOptions options = null)
        {
            return locator.UncheckAsync(options);
        }

        public Task WaitForAsync(LocatorWaitForOptions options = null)
        {
            return locator.WaitForAsync(options);
        }
    }
}


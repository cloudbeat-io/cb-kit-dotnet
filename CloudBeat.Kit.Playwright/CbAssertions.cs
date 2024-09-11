using Microsoft.Playwright;

namespace CloudBeat.Kit.Playwright
{
    /// <summary>
    /// Intended to be used instead of Microsoft.Playwright.Assertions since the later will throw when passed CbLocatorWrapper
    /// </summary>
    public static class CbAssertions
    {
        public static ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator is CbLocatorWrapper wrapper ? wrapper.GetBaseLocator() : locator);

        public static IPageAssertions Expect(IPage page) => Assertions.Expect(page is CbPageWrapper wrapper ? wrapper.GetBasePage() : page);
    }
}

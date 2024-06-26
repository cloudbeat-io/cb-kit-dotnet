using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace CloudBeat.Kit.Playwright
{
    public class CbPageTest : PageTest
    {
        public new ILocatorAssertions Expect(ILocator locator)
            => Assertions.Expect(locator is CbLocatorWrapper wrapper ? wrapper.GetBaseLocator() : locator);

        public new IPageAssertions Expect(IPage page) 
            => Assertions.Expect(page is CbPageWrapper wrapper ? wrapper.GetBasePage() : page);
    }
}

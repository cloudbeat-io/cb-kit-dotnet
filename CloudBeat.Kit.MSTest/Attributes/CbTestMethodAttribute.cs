using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestResult = Microsoft.VisualStudio.TestTools.UnitTesting.TestResult;

namespace CloudBeat.Kit.MSTest.Attributes
{
    public class CbTestMethodAttribute : TestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            if (CbMSTest.Current.IsConfigured)
                CbMSTest.StartCase(testMethod);
            TestResult[] results = base.Execute(testMethod);

            if (!CbMSTest.Current.IsConfigured)
                return results;
            CbMSTest.EndCase(testMethod, results);
            return results;
        }
    }
}


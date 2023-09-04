using CloudBeat.Kit.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace CloudBeat.Kit.MSTest
{
    public class CbMSTestContext : CbTestContext
    {
        public CbMSTestContext(CbConfig config) : base(config, config.HasMandatory() ? new CbMSTestReporter(config) : null)
        {
        }

        public new CbMSTestReporter Reporter => _reporter as CbMSTestReporter;

        public TestContext MSTestContext
        {
            get {
                return Thread.GetData(
                    Thread.GetNamedDataSlot("_MSTestContext")) as TestContext;
            }
            internal set
            {
                Thread.SetData(
                    Thread.GetNamedDataSlot("_MSTestContext"),
                    value);
            }
        }
    }
}

using CloudBeat.Kit.Common;

namespace CloudBeat.Kit.NUnit
{
    public class CbNUnitContext : CbTestContext
    {
        public CbNUnitContext(CbConfig config) : base(config, config.HasMandatory() ? new CbNUnitTestReporter(config) : null)
        {
        }

        public new CbNUnitTestReporter Reporter => _reporter as CbNUnitTestReporter;
    }
}

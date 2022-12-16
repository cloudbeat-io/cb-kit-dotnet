using CloudBeat.Kit.Common;
using System;
using System.Collections.Generic;
using System.Text;

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

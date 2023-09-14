using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBeat.Kit.Common.Models
{
	public class CaseResult : TestableResultBase
	{
        public CaseResult() : base()
        {

        }

        public CaseResult(SuiteResult parentSuite) : base()
        {
            ParentSuite = parentSuite;
        }

        public SuiteResult ParentSuite { get; private set; }
    }
}

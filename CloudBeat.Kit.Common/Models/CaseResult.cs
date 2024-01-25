using Newtonsoft.Json;

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
        [JsonIgnore]
        public SuiteResult ParentSuite { get; private set; }
    }
}

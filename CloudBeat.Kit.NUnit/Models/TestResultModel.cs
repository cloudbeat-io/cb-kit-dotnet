using System.Collections.Generic;

namespace CloudBeat.Kit.NUnit.Models
{
    public class TestResultModel
    {
        public List<StepModel> Steps { get; set; }
        public object ResultData { get; set; }
        public long? FailureReasonId { get; set; }
        public bool HasWarnings { get; set; }
    }
}

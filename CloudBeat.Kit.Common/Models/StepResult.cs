using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudBeat.Kit.Common.Models
{
    public class StepResult
	{
        protected readonly string _id;
        protected readonly StepResult _parent;
        protected readonly IList<StepResult> _steps;
        protected readonly IList<LogMessage> _logs;
        public StepResult(StepResult parent = null) : this(Guid.NewGuid().ToString(), parent)
        {            
        }
        public StepResult(string id, StepResult parent = null)
        {
            _id = id;
            _parent = parent;
            _steps = new List<StepResult>();
            _logs = new List<LogMessage>();
            StartTime = DateTime.UtcNow;
            Type = StepTypeEnum.General;
        }
        
        #region Properties

        public string Id => _id;
        public string Name { get; set; }
        public string MethodName { get; set; }
        public IList<string> Arguments { get; set; }
        
        [JsonIgnore]
        public StepResult Parent => _parent;
        public string Fqn { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StepTypeEnum Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long? Duration { get; set; }
        public TestStatusEnum? Status { get; set; }        
        public FailureResult Failure { get; set; }
        public string ScreenShot { get; set; }
        public IList<StepResult> Steps => _steps;
        public Dictionary<string, double> Stats { get; } = new Dictionary<string, double>();
        public IList<LogMessage> Logs => _logs;
        public bool IsFinished => EndTime.HasValue && Status.HasValue;

        #endregion

        #region Methods
        public StepResult AddNewStep(string name)
		{
            var newStep = new StepResult(this) { Name = name };
            _steps.Add(newStep);
            return newStep;
        }

		public void End(FailureResult failure)
		{
            End(TestStatusEnum.Failed);
            Failure = failure;
		}

		public void End(TestStatusEnum? status = null)
		{
            EndTime = DateTime.UtcNow;
            Duration = ModelHelpers.CalculateDuration(StartTime, EndTime);
            UpdateStatus(status);
		}

        private void UpdateStatus(TestStatusEnum? status = null)
        {
            if (status.HasValue)
            {
                Status = status;
                return;
            }
            // if Status property has not been previously set, calculate status based on sub-steps
            if (!Status.HasValue)
            {
                if (_steps.Any(x => x.Status == TestStatusEnum.Failed || x.Status == TestStatusEnum.Broken))
                {
                    Status = TestStatusEnum.Failed;
                }
                else if (_steps.Any(x => x.Status == TestStatusEnum.Warning))
                {
                    Status = TestStatusEnum.Warning;
                }
                else
                {
                    Status = TestStatusEnum.Passed;
                }
            }
        }
        #endregion
    }
}

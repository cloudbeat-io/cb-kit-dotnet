using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudBeat.Kit.Common.Models
{
    public class StepResult : IResultWithAttachment
    {
        protected readonly string _id;
        protected readonly StepResult _parentStep;
        protected readonly TestableResultBase _parentContainer;
        protected readonly IList<StepResult> _steps;
        protected readonly IList<LogMessage> _logs;
        protected readonly Dictionary<string, IStepExtra> _extra = new Dictionary<string, IStepExtra>();
        private static object _lock = new object();

        public StepResult(TestableResultBase parentContainer, StepResult parentStep = null) : this(Guid.NewGuid().ToString(), parentContainer, parentStep)
        {            
        }

        public StepResult(string id, TestableResultBase parentContainer, StepResult parentStep = null)
        {
            _id = id;
            _parentStep = parentStep;
            _parentContainer = parentContainer;
            // _steps = new BlockingCollection<StepResult>();
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

        public Dictionary<string, IStepExtra> Extra => _extra;

		[JsonIgnore]
        public StepResult Parent => _parentStep;
        [JsonIgnore]
        public TestableResultBase ParentContainer => _parentContainer;
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
        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();

        #endregion

        #region Methods
        public StepResult AddNewStep(string name, StepTypeEnum type = StepTypeEnum.General)
		{
            lock (_lock)
            {
                var newStep = new StepResult(this._parentContainer, this) { Name = name, Type = type };
                _steps.Add(newStep);
                return newStep;
            }
        }

		public void End(TestStatusEnum? status = null, Exception exception = null, string screenshot = null)
		{
            lock (_lock)
            {
                EndTime = DateTime.UtcNow;
                Duration = ModelHelpers.CalculateDuration(StartTime, EndTime);
                ScreenShot = screenshot;
                if (exception != null)
                {
                    Failure = CbExceptionHelper.GetFailureFromException(exception);
                    if (!status.HasValue)
                        status = TestStatusEnum.Failed;
                }
                UpdateStatus(status);
            }
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
                Status = _steps.Any(x => x.Status == TestStatusEnum.Failed || x.Status == TestStatusEnum.Broken)
                    ? TestStatusEnum.Failed : TestStatusEnum.Passed;
        }
        #endregion
    }
}

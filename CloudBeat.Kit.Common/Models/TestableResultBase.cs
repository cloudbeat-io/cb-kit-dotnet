using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudBeat.Kit.Common.Models
{
    public abstract class TestableResultBase : IResultWithAttachment
    {
		protected readonly string _id;
		protected readonly IList<StepResult> _steps;
		protected readonly IList<LogMessage> _logs;
		protected readonly Stack<StepResult> _openSteps = new Stack<StepResult>();
		protected readonly Dictionary<string, object> _testAttributes = new Dictionary<string, object>();
		protected readonly Dictionary<string, object> _context = new Dictionary<string, object>();
        private static object _stepsLock = new object();

        public TestableResultBase() : this(Guid.NewGuid().ToString()) { }
		public TestableResultBase(string id)
		{
			_id = id;			
			_steps = new List<StepResult>();
			_logs = new List<LogMessage>();
			StartTime = DateTime.UtcNow;
		}
		public string Id => _id;
		public IList<StepResult> Steps => _steps;
		public IList<LogMessage> Logs => _logs;
		public string Name { get; set; }
		public string Fqn { get; set; }
		public Dictionary<string, object> Context => _context;
		public IList<string> Arguments { get; set; }
		public Dictionary<string, object> TestAttributes => _testAttributes;
		public TestStatusEnum? Status { get; set; }
		public FailureResult Failure { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public long? Duration { get; set; }
		public long? FailureReasonId { get; set; }
		public bool HasWarnings { get; set; } = false;
        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();

        public StepResult AddNewStep(string name, StepTypeEnum type = StepTypeEnum.General)
		{
			lock (_stepsLock)
			{
                StepResult newStep;
                if (_openSteps.Count > 0)
                    newStep = _openSteps.Peek().AddNewStep(name, type);
                else
                {
                    newStep = new StepResult(this)
                    {
                        Name = name,
                        Type = type
                    };
                    _steps.Add(newStep);
                }
                _openSteps.Push(newStep);
                return newStep;
            }
		}
		public StepResult LastOpenStep => _openSteps.Count > 0 ? _openSteps.Peek() : null;

        public StepResult EndStep(
			StepResult step = null,
			TestStatusEnum? status = null,
			Exception exception = null,
			string screenshot = null)
        {
            lock (_stepsLock)
			{
                if (_openSteps.Count == 0)
                    return null;
                step ??= _openSteps.Peek();
                if (step == null)
                    return null;
                // if the step to be ended is not the last one that has been started
                // then end all the steps in the stack that were started after the specified step (e.g. stepResult)
                while (_openSteps.Count > 0 && _openSteps.Peek() != step)
                {
                    StepResult stepToBeEnded = _openSteps.Pop();
                    if (stepToBeEnded.EndTime != null)
                        stepToBeEnded.End();
                }
                if (_openSteps.Count > 0 && _openSteps.Peek() == step)
                    _openSteps.Pop();
                if (exception != null && status == null)
                    status = TestStatusEnum.Failed;
                // end the step
                step.End(status, exception, screenshot);
                // if step has failed, then mark case as failed
                if (status != null && status.Value == TestStatusEnum.Failed)
                    Status = TestStatusEnum.Failed;
                else if (status == null)
                    CalculateStatus();

                if (step.ScreenShot == null && screenshot != null)
                    step.ScreenShot = screenshot;
                return step;
            }
		}
		public void End()
        {
			End(null, null);
        }
		public void End(TestStatusEnum? status, FailureResult failure)
		{
			EndTime = DateTime.UtcNow;
			Duration = ModelHelpers.CalculateDuration(StartTime, EndTime);
			Status = status.HasValue ? status.Value : CalculateStatus();
			Failure = failure;
		}

		public StepResult FindOpenStepByName(string name)
		{
            lock (_stepsLock)
            {
                return _openSteps.Reverse().FirstOrDefault(x => x.Name == name);
            }
		}

        private TestStatusEnum CalculateStatus()
        {
            lock (_stepsLock)
            {
                if (_steps != null && _steps.Any(x => x.Status == TestStatusEnum.Failed || x.Status == TestStatusEnum.Broken))
                    return TestStatusEnum.Failed;
                return TestStatusEnum.Passed;
            }
        }
    }
}

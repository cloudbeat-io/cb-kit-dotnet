using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using CloudBeat.Kit.Common.Models.Hook;

namespace CloudBeat.Kit.Common.Models
{
    public abstract class TestableResultBase : IResultWithAttachment
    {
	    private readonly Stack<StepResult> _openSteps = new Stack<StepResult>();
        private static readonly object _stepsLock = new object();

        public TestableResultBase() : this(Guid.NewGuid().ToString()) { }
		public TestableResultBase(string id)
		{
			Id = id;			
		}
		public string Id { get; }

		public IList<StepResult> Steps { get; } = new List<StepResult>();

		public IList<StepResult> Hooks { get; } = new List<StepResult>();

		public IList<LogMessage> Logs { get; } = new List<LogMessage>();

		public Dictionary<string, object> TestAttributes { get; } = new Dictionary<string, object>();

		public string Name { get; set; }
		public string Fqn { get; set; }
		public string Owner { get; set; }
		public IList<string> Arguments { get; set; }
		public TestStatusEnum? Status { get; set; }
		public FailureResult Failure { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public long? Duration { get; set; }
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
                    Steps.Add(newStep);
                }
                _openSteps.Push(newStep);
                return newStep;
            }
		}
		public StepResult AddNewHook(string name, HookTypeEnum type)
		{
			lock (_stepsLock)
			{
				StepResult newStep = new StepResult(this)
				{
					Name = name,
					Type = StepTypeEnum.Hook
				};
				newStep.Extra.Add("hook", new HookStepExtra(type));
				Hooks.Add(newStep);
				_openSteps.Push(newStep);
				return newStep;
			}
		}

		[JsonIgnore]
		public StepResult LastOpenStep => _openSteps.Count > 0 ? _openSteps.Peek() : null;

		[JsonIgnore]
		public StepResult LastFailedStep => Steps.LastOrDefault(s => s.Status == TestStatusEnum.Failed);

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
                    if (stepToBeEnded.EndTime == null)
                        stepToBeEnded.End();
                }
                if (_openSteps.Count > 0 && _openSteps.Peek() == step)
                    _openSteps.Pop();
                if (exception != null && status == null)
                    status = TestStatusEnum.Failed;
                // end the step
                step.End(status, exception, screenshot);
                // if step has failed, then mark case as failed
                if (status is TestStatusEnum.Failed)
                    Status = TestStatusEnum.Failed;
                else if (status == null)
	                Status = CalculateHooksAndStepsStatus();

                if (step.ScreenShot == null && screenshot != null)
                    step.ScreenShot = screenshot;
                return step;
            }
		}

		public StepResult FindOpenStepByName(string name)
		{
            lock (_stepsLock)
            {
                return _openSteps.Reverse().FirstOrDefault(x => x.Name == name);
            }
		}

        protected TestStatusEnum CalculateHooksAndStepsStatus()
        {
            lock (_stepsLock)
            {
                if (Steps != null && Steps.Any(x => x.Status == TestStatusEnum.Failed || x.Status == TestStatusEnum.Broken))
                    return TestStatusEnum.Failed;
                if (Hooks != null && Hooks.Any(x => x.Status == TestStatusEnum.Failed || x.Status == TestStatusEnum.Broken))
	                return TestStatusEnum.Failed;
                return TestStatusEnum.Passed;
            }
        }

        public void Clear()
        {
            Steps.Clear();
            _openSteps.Clear();
            Hooks.Clear();
            Logs.Clear();
            Arguments?.Clear();
            Attachments?.Clear();
            StartTime = DateTime.UtcNow;
            EndTime = null;
            Duration = null;
            Status = null;
            Failure = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CloudBeat.Kit.Common.Models
{
    public abstract class TestableResultBase
    {
		protected readonly string _id;
		protected readonly IList<StepResult> _steps;
		protected readonly IList<LogMessage> _logs;
		protected readonly Stack<StepResult> _openSteps = new Stack<StepResult>();
		protected readonly Dictionary<string, object> _testAttributes = new Dictionary<string, object>();
		protected readonly Dictionary<string, object> _context = new Dictionary<string, object>();
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

		public StepResult AddNewStep(string name)
		{
			StepResult newStep;
			if (_openSteps.Count > 0)
				newStep = _openSteps.Last().AddNewStep(name);
			else
			{
				newStep = new StepResult()
				{
					Name = name
				};
				_steps.Add(newStep);
			}
			_openSteps.Push(newStep);
			return newStep;
		}
		public StepResult EndStep(string name = null, TestStatusEnum? status = null)
		{
			var targetStep = name != null ?
				_openSteps.Reverse().Where(x => x.Name == name).FirstOrDefault() :
				_openSteps.LastOrDefault();
			if (targetStep == null || targetStep.EndTime.HasValue)
				return targetStep;
			return EndStep(targetStep, status);
		}
		public StepResult EndStep(StepResult step, TestStatusEnum? status = null, string screenshot = null)
		{
			if (step == null)
				return null;
			var targetStep = step;
			// close the target step and any other step above it in the stack
			while (_openSteps.Count != 0)
			{
				var currentStep = _openSteps.Pop();
				currentStep.End(status);
				if (currentStep == targetStep)
					break;
			}
			// if step has failed, then mark case as failed
			if (status != null && status.Value == TestStatusEnum.Failed)
				this.Status = TestStatusEnum.Failed;
			if (targetStep.ScreenShot == null)
				targetStep.ScreenShot = screenshot;
			return targetStep;
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

        private TestStatusEnum CalculateStatus()
        {
			if (_steps != null && _steps.Any(x => x.Status == TestStatusEnum.Failed || x.Status == TestStatusEnum.Broken))
				return TestStatusEnum.Failed;
			return TestStatusEnum.Passed;
        }
    }
}

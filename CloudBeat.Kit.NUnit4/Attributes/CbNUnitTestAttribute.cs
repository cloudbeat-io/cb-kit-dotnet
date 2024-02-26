using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System;

namespace CloudBeat.Kit.NUnit.Attributes
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class CbNUnitTestAttribute : PropertyAttribute, ITestAction, IApplyToContext, IWrapSetUpTearDown
    {
        private readonly bool _isWrappedIntoStep;
        private static object _lock = new object();

        public CbNUnitTestAttribute(bool wrapIntoStep = true)
        {
            _isWrappedIntoStep = wrapIntoStep;
            //Debugger.Launch();
        }


        public void BeforeTest(ITest test)
        {
            var cbCtx = CbNUnit.Current;
            if (!cbCtx.IsConfigured)
                return;
            if (test.IsSuite)
                cbCtx.Reporter.StartSuite(test as TestSuite);
            else
                cbCtx.Reporter.StartCase(test as Test);

            //_allureNUnitHelper.Value = new AllureNUnitHelper(test);
            //_allureNUnitHelper.Value.StartAll(_isWrappedIntoStep);
        }

        public void AfterTest(ITest test)
        {
            var cbCtx = CbNUnit.Current;
            if (!cbCtx.IsConfigured)
                return;
            if (test.IsSuite)
                cbCtx.Reporter.EndSuite(test as TestSuite);
            else
                cbCtx.Reporter.EndCase(test as Test);
            //_allureNUnitHelper.Value.StopAll(_isWrappedIntoStep);
        }

        public void ApplyToContext(TestExecutionContext context)
        {
            lock (_lock)
			{
                var _cbContext = CbNUnit.Current;
                if (_cbContext.IsConfigured)
                {
                    _cbContext.Reporter.StartInstance();
                }
            }
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command;
        }        

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
    }
}

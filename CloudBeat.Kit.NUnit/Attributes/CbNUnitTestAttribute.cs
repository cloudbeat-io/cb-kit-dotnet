using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace CloudBeat.Kit.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class CbNUnitTestAttribute : PropertyAttribute, ITestAction, IApplyToContext, IWrapSetUpTearDown
    {
        private readonly bool _isWrappedIntoStep;
        private static object _lock = new object();
        private static readonly ConcurrentDictionary<string, bool> _startedTests = new();
        private readonly ThreadLocal<IList<SetUpFixture>> _setupFixtures = new();
        private readonly ThreadLocal<IList<SetUpFixture>> _teardownFixtures = new();

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
            // Prevent processing the same test twice
            // (BeforeTest might be called twice for the same test ID somehow)
            if (!_startedTests.TryAdd(test.Id, true))
                return;
            //if (test.IsSuite)
                //cbCtx.Reporter.StartSuite(test as TestSuite);
            if (!test.IsSuite)
                cbCtx.Reporter.StartCase(test as Test);
        }

        public void AfterTest(ITest test)
        {
            var cbCtx = CbNUnit.Current;
            if (!cbCtx.IsConfigured)
                return;
            // Prevent processing the same test twice
            // (AfterTest might be called twice for the same test ID somehow) 
            if (_startedTests.ContainsKey(test.Id) && _startedTests[test.Id] == false)
                return;
            // Indicate that the current test has ended
            if (_startedTests.ContainsKey(test.Id))
                _startedTests[test.Id] = false;
            if (test.IsSuite)
                cbCtx.Reporter.EndSuite(test as TestSuite);
            else
                cbCtx.Reporter.EndCase(test as Test);
        }

        public void ApplyToContext(TestExecutionContext context)
        {
            lock (_lock)
			{
                var cbCtx = CbNUnit.Current;
                if (cbCtx.IsConfigured)
                {
                    cbCtx.Reporter.StartInstance();
                    if (context.CurrentTest is TestFixture { IsSuite: true } testFixture) 
                        cbCtx.Reporter.StartSuite(testFixture, testFixture.Parent as TestSuite);
                    else if (context.CurrentTest is SetUpFixture { IsSuite: true } setupFixture) 
                        cbCtx.Reporter.StartSuite(setupFixture, setupFixture.Parent as TestSuite);
                    //testFixture.OneTimeSetUpMethods
                    /*if (_beforeTestCalls.Value == null)
                        _beforeTestCalls.Value = new Dictionary<string, ITest>();*/
                    if (_setupFixtures.Value == null)
                        _setupFixtures.Value = new List<SetUpFixture>();
                    if (context.CurrentTest is SetUpFixture fixture)
                        _setupFixtures.Value.Add(fixture);
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

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using System;

namespace CloudBeat.Kit.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CbNUnitHookAttribute : PropertyAttribute, IWrapSetUpTearDown
    {
        public TestCommand Wrap(TestCommand command)
        {
            return command;
        }
        
        class SetUpTearDownCommandWrapper : SetUpTearDownCommand
        {
            public SetUpTearDownCommandWrapper(TestCommand innerCommand, SetUpTearDownItem setUpTearDown) 
                : base(innerCommand, setUpTearDown)
            {
                BeforeTest = (context) =>
                {
                    base.BeforeTest.Invoke(context);
                };
                AfterTest = (context) =>
                {
                    base.AfterTest.Invoke(context);
                };
            }
        }

        /*public static TestCommand MakeTestCommand(TestMethod test)
        {
            // Command to execute test
            TestCommand command = new TestMethodCommand(test);

            // Add any wrappers to the TestMethodCommand
            foreach (IWrapTestMethod wrapper in test.Method.GetCustomAttributes<IWrapTestMethod>(true))
                command = wrapper.Wrap(command);

            // Wrap in TestActionCommand
            command = new TestActionCommand(command, null);

            // Wrap in SetUpTearDownCommand
            command = new SetUpTearDownCommand(command, new SetUpTearDownItem { });

            // Add wrappers that apply before setup and after teardown
            foreach (ICommandWrapper decorator in test.Method.GetCustomAttributes<IWrapSetUpTearDown>(true))
                command = decorator.Wrap(command);

            // Add command to set up context using attributes that implement IApplyToContext
            IApplyToContext[] changes = test.Method.GetCustomAttributes<IApplyToContext>(true);
            if (changes.Length > 0)
                command = new ApplyChangesToContextCommand(command, changes);

            return command;
        }*/

    }
}

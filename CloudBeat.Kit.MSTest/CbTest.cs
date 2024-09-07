using CloudBeat.Kit.Common.Models;
using CloudBeat.Kit.MSTest.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace CloudBeat.Kit.MSTest
{
    [TestClass]
    public abstract class CbTest : IDisposable
    {
		public TestContext TestContext { get; set; }

        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void ClassInitialize(TestContext context)
        {
            CbMSTest.SetMSTestContext(context);
        }

        [TestInitialize]
        public void TestInitialize()
		{
            CbMSTest.SetMSTestContext(TestContext);            
            if (!HasCbTestMethodAttribute(TestContext?.TestName))
            {
				var fqn = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}";
				CbMSTest.StartCase(TestContext.TestName, fqn);
			}
		}

		[TestCleanup]
		public void TestCleanup()
        {
            try 
            {
                CbMSTest.SetMSTestContext(TestContext);
                if (!HasCbTestMethodAttribute(TestContext?.TestName))
                    CbMSTest.EndCase();
            }
            catch (Exception ex) 
            {
                TestContext?.WriteLine("Error in TestCleanup: " + ex);
            }
        }

        /// <summary>
        /// Retrieves a boolean value indicating whether the code is running from CloudBeat.
        /// </summary>
        /// <returns><c>true</c> if running from CloudBeat; <c>false</c> otherwise.</returns>
        public static bool IsRunningFromCB()
        {
            return CbMSTest.IsRunningFromCB();
        }

        /// <summary>
        /// Adds name/value data pair to the test result.
        /// </summary>
        /// <param name="name">Data name.</param>
        /// <param name="data">Data value.</param>
        public void AddOutputData(string name, object data)
        {
            CbMSTest.AddOutputData(name, data, TestContext);
        }

        /// <summary>
        /// Adds name/value test attribute pair to the test result.
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="value">Attribute value</param>
        public void AddTestAttribute(string name, object value)
        {
            CbMSTest.AddTestAttribute(name, value, TestContext);
        }

        /// <summary>
        /// Sets failure reason.
        /// Could be used from cleanup methods or catch blocks to set reason for the test failure.
        /// </summary>
        /// <param name="reason">Failure reason.</param>
        public void SetFailureReason(FailureReasonEnum reason)
        {
            CbMSTest.SetFailureReason(reason, TestContext);
        }

        public void HasWarnings(bool warnings = true)
        {
            CbMSTest.HasWarnings(warnings);
        }
        
        public void Dispose()
        {
        }

		private bool HasCbTestMethodAttribute(string testMethodName)
		{
            if (string.IsNullOrEmpty(testMethodName))
                return false;

			var methodInfo = this.GetType().GetMethod(testMethodName);
			if (methodInfo == null) 
                return false;

			var cbTestMethodAttr = methodInfo.GetCustomAttribute<CbTestMethodAttribute>();
			return cbTestMethodAttr != null;
		}
	}
}

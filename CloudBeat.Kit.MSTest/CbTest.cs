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

        public bool TakeFullPageScreenshots { get; set; } = true;
        public bool IsRunningFromCB() 
        { 
            return CbMSTest.Current.IsConfigured;
        }

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
			CbMSTest.SetMSTestContext(TestContext);
			if (!HasCbTestMethodAttribute(TestContext?.TestName))
				CbMSTest.EndCase();
        }

        public void AddOutputData(string name, object data)
        {
            CbMSTest.AddOutputData(name, data, TestContext);
        }

        public void SetFailureReason(FailureReasonEnum reason)
        {
            CbMSTest.SetFailureReason(reason);
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
            if (string.IsNullOrEmpty(testMethodName)) return false;
			var methodInfo = this.GetType().GetMethod(testMethodName);
			if (methodInfo == null) return false;
			var cbTestMethodAttr = methodInfo.GetCustomAttribute<CbTestMethodAttribute>();
			return cbTestMethodAttr != null;
		}

	}
}

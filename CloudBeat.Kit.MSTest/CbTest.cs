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

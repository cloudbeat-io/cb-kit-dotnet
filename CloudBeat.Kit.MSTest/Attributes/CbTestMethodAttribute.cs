using System;
using System.Collections.Generic;
using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestResult = Microsoft.VisualStudio.TestTools.UnitTesting.TestResult;

namespace CloudBeat.Kit.MSTest.Attributes
{
	public class CbTestMethodAttribute : TestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            if (CbMSTest.Current.IsConfigured)
                CbMSTest.StartCase(testMethod);
            TestResult[] results = base.Execute(testMethod);

            if (!CbMSTest.Current.IsConfigured)
                return results;
            CbMSTest.EndCase(testMethod, results);
            //results[0].ResultFiles = new List<string>();
            //results[0].ResultFiles.Add("C:\\Users\\Administrator\\Desktop\\Code\\cb-mstest-net-example\\bin\\Debug\\net5.0\\MSTestExampleProject.UnitTest.ParameterizedMethod_case_result1.json");
            return results;
        }
    }
}


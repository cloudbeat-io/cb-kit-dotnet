using CloudBeat.Kit.Common.Models;
using CloudBeat.Kit.MSTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
    }
}

using CloudBeat.Kit.Common.Enums;
using System;

namespace CloudBeat.Kit.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CbTestModeAttribute : Attribute
    {
        public CbTestModeAttribute(CbTestModeEnum testMode)
        {
            TestMode = testMode;
        }
        public CbTestModeEnum TestMode { get; internal set; }
    }
}

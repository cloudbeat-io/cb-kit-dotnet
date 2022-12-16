using CloudBeat.Kit.Common.Enums;
using System;

namespace CloudBeat.Kit.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class CBTestModeAttribute : Attribute
    {
        public CBTestModeAttribute(CBTestModeEnum testMode)
        {
            TestMode = testMode;
        }
        public CBTestModeEnum TestMode { get; internal set; }
    }
}

using System;
using CloudBeat.Kit.Common.Enums;

namespace CloudBeat.Kit.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]		
    public class CbTestRail : Attribute
    {
		public CbTestRail(TestRailRefType type, string value)
		{
            Type = type;
            Value = value;
		}
		public TestRailRefType Type { get; internal set; }
        public string Value { get; internal set; }
    }
}


using System;
namespace CloudBeat.Kit.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]		
    public class CbLink : Attribute
    {
		public CbLink(string name, string url)
		{
            Name = name;
            Url = url;
		}
		public string Name { get; internal set; }
        public string Url { get; internal set; }
    }
}


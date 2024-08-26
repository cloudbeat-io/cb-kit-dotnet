using CloudBeat.Kit.Common.Wrappers;
using NLog.Targets;

namespace CloudBeat.Kit.NUnit.NLog
{
    [Target("CloudBeat")]
	public class CbNLogTarget : CbNLogTargetBase
	{
		public CbNLogTarget() : base(CbNUnit.Current?.Reporter)
		{
		}
	}
}

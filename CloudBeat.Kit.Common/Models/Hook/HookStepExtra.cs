using System;

namespace CloudBeat.Kit.Common.Models.Hook
{
	public class HookStepExtra : IStepExtra
	{
		public HookStepExtra(HookTypeEnum type)
		{
			Type = type;
		}
		public HookTypeEnum Type { get; set; }
	}
}

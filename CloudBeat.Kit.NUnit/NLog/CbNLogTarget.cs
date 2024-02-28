using CloudBeat.Kit.Common;
using CloudBeat.Kit.Common.Wrappers;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

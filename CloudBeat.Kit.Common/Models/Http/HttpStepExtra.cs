using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBeat.Kit.Common.Models.Http
{
	public class HttpStepExtra : IStepExtra
	{
		public RequestResult Request { get; set; }
		public ResponseResult Response { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBeat.Kit.Common.Models.Http
{
	public class RequestResult
	{
		public string Url { get; set; }
		public string Method { get; set; }
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
		public string Cookies { get; set; }
		public Dictionary<string, string> QueryParameters { get; set; }
		public string Content { get; set; }
		public string ContentType { get; set; }
		public string Version { get; set; }

	}
}

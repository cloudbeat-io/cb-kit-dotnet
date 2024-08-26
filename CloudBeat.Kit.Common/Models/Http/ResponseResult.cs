using System.Collections.Generic;

namespace CloudBeat.Kit.Common.Models.Http
{
    public class ResponseResult
	{
        public string Url { get; set; }
        public int StatusCode { get; set; }
		public string StatusText { get; set; }
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
		public string Cookies { get; set; }
		public string Content { get; set; }
		public string ContentType { get; set; }
        public long? ContentLength { get; set; }
        public string Version { get; set; }
	}
}

using System.Collections.Generic;

namespace CloudBeat.Kit.Common.Models.Http
{
    public class RequestResult
	{
		public string Url { get; set; }
		public string Method { get; set; }
        public string Path { get; set; }
        public IDictionary<string, string> QueryParams { get; set; }
        public IDictionary<string, string> FormData { get; set; }
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
		public string Cookies { get; set; }
		public string Content { get; set; }
		public string ContentType { get; set; }
        public long? ContentLength { get; set; }
        public string Version { get; set; }

	}
}

namespace CloudBeat.Kit.Common.Models
{
    public class FailureResult
	{
        public string Type { get; set; }
        public string Subtype { get; set; }
        public string Data { get; set; }
		public object Stacktrace { get; set; }
		public string Message { get; set; }
        public string Location { get; set; }
    }
}

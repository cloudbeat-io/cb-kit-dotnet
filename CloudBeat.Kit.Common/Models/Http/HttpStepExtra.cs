namespace CloudBeat.Kit.Common.Models.Http
{
    public class HttpStepExtra : IStepExtra
	{
		public RequestResult Request { get; set; }
		public ResponseResult Response { get; set; }
	}
}

namespace CloudBeat.Kit.Common
{
    public interface ICbPageSourceProvider
	{
		public bool TakePageSourceOnError { get; }
		(string Source, string MimeType) PageSource();
	}
}


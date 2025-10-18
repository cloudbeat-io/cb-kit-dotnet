namespace CloudBeat.Kit.Common.Client
{
    public class CbApiClient
	{
		public CbApiClient(CbConfig config)
		{
			RuntimeApi = new RuntimeApi(config.ApiUrl, config.ApiKey);
		}
		public IRuntimeApi RuntimeApi { get; private set; }
	}
}

using System;
using System.Net.Http;
using System.Threading;

namespace CloudBeat.Kit.Common
{
    public static class CbHttpHelper
    {
        const int MAX_DOWNLOAD_RETRIES = 10;
        const int RETRY_INTERVAL = 2000;
        private const int REQUEST_TIMEOUT = 2000; 
        public static byte[] DownloadFile(string fileUrl)
        {
            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(REQUEST_TIMEOUT);
            int retries = 0;
            while (retries < MAX_DOWNLOAD_RETRIES)
            {
                var response = client.GetAsync(fileUrl).Result;
                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsByteArrayAsync().Result;
                retries++;
                Thread.Sleep(RETRY_INTERVAL);
            }
            return null;
        }
    }
}

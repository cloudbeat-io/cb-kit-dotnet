using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CloudBeat.Kit.Common
{
    public static class CbHttpHelper
    {
        const int MAX_DOWNLOAD_RETRIES = 10;
        const int RETRY_INTERVAL = 2000;
        private const int REQUEST_TIMEOUT = 2000;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true
                }
            },
            Formatting = Formatting.None,
        };
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
        public static async Task<HttpResponseMessage> PostJsonAsync<T>(string url, string token, T data)
        {
            using var httpClient = new HttpClient();
            // Create the content to send
            var content = CreateJsonHttpContent(data);
            
            // Add any headers if needed
            httpClient.DefaultRequestHeaders.Add("User-Agent", "CB-Reporter");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            // Send the POST request
            return await httpClient.PostAsync(url, content);
        }
        
        private static HttpContent CreateJsonHttpContent(object content)
        {
            string jsonContentStr = JsonConvert.SerializeObject(content, JsonSerializerSettings);

            ByteArrayContent jsonHttpContent = new ByteArrayContent(Encoding.UTF8.GetBytes(jsonContentStr));
            jsonHttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return jsonHttpContent;
        }
    }
}

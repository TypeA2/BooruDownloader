using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using Newtonsoft.Json;

namespace BooruDownloader {
    public static class WebHelper {

        public static WebClient Client = new WebClient();

        public static string CreateApiUrl(string base_endpoint, string target_endpoint) => base_endpoint + target_endpoint;

        public static HttpWebRequest CreateRequest(string base_endpoint, string target_endpoint,
            Dictionary<string, string> data = null) {
            return CreateRequest(CreateApiUrl(base_endpoint, target_endpoint), data);
        }

        public static HttpWebRequest CreateRequest(string uri, Dictionary<string, string> data = null) {
            if (data != null) {
                UriBuilder builder = new UriBuilder(uri) { Port = -1 };
                NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);

                foreach (KeyValuePair<string, string> val in data) {
                    query[val.Key] = val.Value;
                }

                builder.Query = query.ToString();

                uri = builder.ToString();
            }
            

            HttpWebRequest request = WebRequest.CreateHttp(HttpUtility.UrlDecode(uri) ?? uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return request;
        }

        public static async Task<string> GetTextAsync(string base_endpoint, string target_endpoint,
            Dictionary<string, string> data = null) {
            return await GetTextAsync(CreateApiUrl(base_endpoint, target_endpoint), data);
        }

        public static async Task<string> GetTextAsync(string uri, Dictionary<string, string> data = null) {
            return await GetTextAsync(CreateRequest(uri, data));
        }

        public static async Task<string> GetTextAsync(HttpWebRequest request) {
            return await Client.DownloadStringTaskAsync(request.RequestUri);
        }

        public static async Task<T> GetJsonAsync<T>(string base_endpoint, string target_endpoint,
            Dictionary<string, string> data = null) {
            return await GetJsonAsync<T>(CreateApiUrl(base_endpoint, target_endpoint), data);
        }

        public static async Task<T> GetJsonAsync<T>(string uri, Dictionary<string, string> data = null) {
            return await GetJsonAsync<T>(CreateRequest(uri, data));
        }

        public static async Task<T> GetJsonAsync<T>(HttpWebRequest request) {
            return JsonConvert.DeserializeObject<T>(await GetTextAsync(request));
        }
    }
}

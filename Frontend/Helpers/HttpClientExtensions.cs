using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Frontend.Helpers
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Extension method to send PATCH requests with JSON body
        /// </summary>
        /// <param name="httpClient">Client to send the request with</param>
        /// <param name="requestUri">Target uri of the request</param>
        /// <param name="value">The object to serialize</param>
        /// <typeparam name="T">Object type</typeparam>
        /// <returns>Http response, using which JSON repsonse can be parsed</returns>
        public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient httpClient,string requestUri, T value)
        {
            var content = JsonContent.Create(value);
            return httpClient.PatchAsync(requestUri, content);
        }
    }
}
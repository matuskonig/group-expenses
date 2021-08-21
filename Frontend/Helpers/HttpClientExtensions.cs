using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Frontend.Helpers
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient httpClient,string requestUri, T value)
        {
            var content = JsonContent.Create(value);
            return httpClient.PatchAsync(requestUri, content);
        }
    }
}
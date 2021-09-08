using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Entities.Dto.General;
using Frontend.Services;

namespace Frontend.Extensions
{
    public static class AlertMessageServiceExtensions
    {
        /// <summary>
        /// Used when we know that the API response is in unsuccessful state
        /// Parse the error and show it
        /// </summary>
        /// <param name="service"></param>
        /// <param name="response"></param>
        public static async Task ShowNetworkError(this AlertMessageService service, HttpResponseMessage response)
        {
            var result = await response.Content.ReadFromJsonAsync<ProblemDetail>();
            if (result?.Title != null)
            {
#pragma warning disable 4014
                // Result is not awaited, because the error message has its own lifecycle (show, wait, dispose)
                // on which we dont want to await, so we can proceed with other things
                service.ShowErrorMessage(result.Title);
#pragma warning restore 4014
            }
        }
    }
}
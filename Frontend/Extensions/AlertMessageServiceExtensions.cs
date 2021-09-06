using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Entities.Dto.General;
using Frontend.Services;

namespace Frontend.Extensions
{
    public static class AlertMessageServiceExtensions
    {
        public static async Task ShowNetworkError(this AlertMessageService service, HttpResponseMessage response)
        {
            var result = await response.Content.ReadFromJsonAsync<ProblemDetail>();
            if (result?.Title != null)
            {
#pragma warning disable 4014
                service.ShowErrorMessage(result.Title);
#pragma warning restore 4014
            }
        }
    }
}
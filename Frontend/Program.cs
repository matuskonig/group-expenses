using System;
using System.Net.Http;
using System.Threading.Tasks;
using Frontend.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend
{
    public class Program
    {
        private static Func<IServiceProvider, HttpClient> GetHttpClientConfiguration(
            IConfiguration configuration) => _ =>
        {
            var baseAddress = configuration["BaseAddress"];
            Console.WriteLine($"{baseAddress}, {new Uri(baseAddress)}");
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress),
            };
            client.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", baseAddress);
            client.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
            return client;
        };

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.Services
                .AddScoped(GetHttpClientConfiguration(builder.Configuration))
                .AddScoped<AuthApiService>()
                .AddScoped<ITodoService, TodoService>();
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
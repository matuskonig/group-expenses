using System;
using System.Net.Http;
using System.Threading.Tasks;
using Frontend.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.Services
                .AddScoped(_ => new HttpClient
                {
                    BaseAddress = new Uri(builder.Configuration["BaseAddress"]),
                })
                .AddScoped<UserApiService>()
                .AddScoped<AuthApiService>()
                .AddScoped<GroupService>();
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
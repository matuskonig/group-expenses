using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Frontend.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Frontend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services
                .AddScoped(
                    _ => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)})
                .AddSingleton<AuthApiService>()
                .AddScoped<ITodoService>(_ => new TodoService());
            var host = builder.Build();

            await host.RunAsync();
        }
    }
}
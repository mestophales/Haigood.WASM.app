using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Haigood.WASM.app
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddHttpClient("haigoodwasmapi")
                .AddHttpMessageHandler(sp =>
                {
                var handler = sp.GetService<AuthorizationMessageHandler>()
                .ConfigureHandler(
                    authorizedUrls: new[] { "https://localhost:5004" },
                    scopes: new[] { "haigoodwasmapi" });
                return handler;
                });

            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("haigoodwasmapi"));

            builder.Services.AddOidcAuthentication(options =>
            {
                // Configure your authentication provider options here.
                // For more information, see https://aka.ms/blazor-standalone-auth
                //builder.Configuration.Bind("Local", options.ProviderOptions);
                builder.Configuration.Bind("oidc", options.ProviderOptions);
            });

            await builder.Build().RunAsync();
        }
    }
}

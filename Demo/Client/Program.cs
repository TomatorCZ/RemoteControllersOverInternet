using RemoteController;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            // Configuration
            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddScopedClient();

            await builder.Build().RunAsync();
        }
    }
}

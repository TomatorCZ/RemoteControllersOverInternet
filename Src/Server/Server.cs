using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Threading.Tasks;

namespace RemoteController
{
    public class Server
    {
        IHost _host;

        public Server()
        {
            _host = CreateHostBuilder().Build();
        }

        private IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel((options) => options.ListenAnyIP(5001));
                webBuilder.UseStartup<Startup>();        
            });
        }

        public async Task Run()
        {
            await _host.RunAsync();
        }
    }
}

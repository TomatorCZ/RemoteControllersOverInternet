using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RemoteControllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteController
{
    public class Server : IDisposable
    {
        IHost _host;
        public ClientManager Manager 
        {
            get {
                if (_host == null)
                {
                    return null;
                }
                else
                {
                    var result = _host.Services.GetService(typeof(ClientManager));
                    return result as ClientManager;
                }   
            } 
        }

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

        public async Task RunAsync()
        {
            await _host.RunAsync();
        }

        public async Task CloseAsync()
        {
            await Manager.CloseAsync();
            await _host.StopAsync();
        }

        public void Dispose()
        {
            Manager.Dispose();
            _host.StopAsync().Wait();
        }
    }
}

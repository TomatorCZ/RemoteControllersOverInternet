﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Server provides a http server and <see cref="ClientManager"> for managing clients.
    /// </summary>
    public class Server<TStartUp, TClient> : IDisposable where TStartUp : class where TClient : Player
    {
        IHost _host;

        public ClientManager<TClient> Manager 
        {
            get {
                if (_host == null)
                {
                    return null;
                }
                else
                {
                    var result = _host.Services.GetService(typeof(ClientManager<TClient>));
                    return result as ClientManager<TClient>;
                }   
            } 
        }

        /// <summary>
        /// Builds host with default settings.
        /// </summary>
        public Server() : this((options) => options.ListenAnyIP(5001)) { }

        /// <summary>
        /// Builds host with configuration options of Kester.
        /// </summary>
        public Server(Action<KestrelServerOptions> options)
        {
            _host = CreateHostBuilder(options).Build();
        }

        private IHostBuilder CreateHostBuilder(Action<KestrelServerOptions> options)
        {
            return Host.CreateDefaultBuilder().ConfigureLogging(opt => 
            {
                opt.ClearProviders();
                opt.AddConsole();
            }).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(options);
                webBuilder.UseStartup<TStartUp>();        
            });
        }

        public async Task RunAsync() => await _host.RunAsync();

        /// <summary>
        /// Tries to gracefully disconnect all users and to stop the server.
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            await Manager.CloseAsync();
            await _host.StopAsync();
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose()
        {
            Manager.Dispose();
            _host.StopAsync().Wait();
        }
    }
}

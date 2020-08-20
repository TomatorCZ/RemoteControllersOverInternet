using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteController
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddClientManager(this IServiceCollection services)
        {
            services.AddSingleton<ClientManager>();
            return services;
        }
    }
}

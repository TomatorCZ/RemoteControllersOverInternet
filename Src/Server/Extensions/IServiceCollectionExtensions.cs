using Microsoft.Extensions.DependencyInjection;

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

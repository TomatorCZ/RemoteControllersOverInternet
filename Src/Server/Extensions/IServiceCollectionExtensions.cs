using Microsoft.Extensions.DependencyInjection;

namespace RemoteController
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddClientManager<TClient>(this IServiceCollection services, IClientFactory<TClient> factory) where TClient : Player
        {
            services.AddSingleton<ClientManager<TClient>>(new ClientManager<TClient>(factory));
            return services;
        }
    }
}

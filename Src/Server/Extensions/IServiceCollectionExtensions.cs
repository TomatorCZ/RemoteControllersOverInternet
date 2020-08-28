using Microsoft.Extensions.DependencyInjection;

namespace RemoteController
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="ClientManager{TClient}"> to services. The factory is used to initialize the manager.
        /// </summary>
        public static IServiceCollection AddClientManager<TClient>(this IServiceCollection services, IClientFactory<TClient> factory) where TClient : Player
        {
            services.AddSingleton<ClientManager<TClient>>(new ClientManager<TClient>(factory));
            return services;
        }
    }
}

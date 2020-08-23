using Microsoft.Extensions.DependencyInjection;

namespace RemoteController
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedClient(this IServiceCollection services)
        {
            services.AddScoped<User>();
            return services;
        }
    }
}

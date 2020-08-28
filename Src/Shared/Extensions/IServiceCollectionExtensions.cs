using Microsoft.Extensions.DependencyInjection;

namespace RemoteController
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="User"> to services as a scoped service.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedClient(this IServiceCollection services)
        {
            services.AddScoped<User>();
            return services;
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RemoteController
{ 
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebSocketMiddleware<TClient>(this IApplicationBuilder app, string requestPath) where TClient : Player
        {  
            return app.UseMiddleware<WebSocketMiddleware<TClient>>(requestPath);
        }
    }
}

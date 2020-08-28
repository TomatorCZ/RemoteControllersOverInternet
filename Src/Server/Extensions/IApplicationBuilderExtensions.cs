using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RemoteController
{ 
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="WebSocketMiddleware{TClient}"> to the pipeline. The requestPath is used to initialize the middleware.
        /// </summary>
        public static IApplicationBuilder UseWebSocketMiddleware<TClient>(this IApplicationBuilder app, string requestPath) where TClient : Player
        {  
            return app.UseMiddleware<WebSocketMiddleware<TClient>>(requestPath);
        }
    }
}

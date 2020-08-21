using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RemoteController
{ 
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder app)
        {  
            return app.UseMiddleware<WebSocketMiddleware>(app.ApplicationServices.GetRequiredService<ClientManager>());
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RemoteController;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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

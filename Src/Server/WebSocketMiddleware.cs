using Microsoft.AspNetCore.Http;
using RemoteController;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Handles incomming upgrade requests and accepting new clients.
    /// </summary>
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ClientManager _manager;

        public WebSocketMiddleware(RequestDelegate next, ClientManager manager)
        {
            _next = next;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/ws")
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

                    await Process(webSocket);
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(httpContext);
            }
        }

        public async Task Process(WebSocket socket)
        {
            var client = _manager.AddClient(socket);
            while (client.IsConnected)
            {
                await Task.Delay(1000);
            }
        }
    }
}

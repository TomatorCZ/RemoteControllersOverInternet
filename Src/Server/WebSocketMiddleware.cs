using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
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
        private readonly ILogger<WebSocketMiddleware> _logger;

        public WebSocketMiddleware(RequestDelegate next, ClientManager manager, ILogger<WebSocketMiddleware> logger)
        {
            _next = next;
            _manager = manager;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/ws")
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                    _logger.LogInformation("Web socket accepted.");
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

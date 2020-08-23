using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Handles incomming upgrade requests and accepting new clients.
    /// </summary>
    public class WebSocketMiddleware<TClient> where TClient : Player
    {
        private readonly RequestDelegate _next;
        private readonly ClientManager<TClient> _manager;
        private readonly ILogger<WebSocketMiddleware<TClient>> _logger;
        private readonly string _requestPath;

        public WebSocketMiddleware(RequestDelegate next, ClientManager<TClient> manager, ILogger<WebSocketMiddleware<TClient>> logger, string requestPath)
        {
            _next = next;
            _manager = manager;
            _logger = logger;
            _requestPath = requestPath;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path == _requestPath)
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

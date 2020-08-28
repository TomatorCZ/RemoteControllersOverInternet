using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Handles incomming upgrade requests and accepts new clients.
    /// </summary>
    public class WebSocketMiddleware<TClient> where TClient : Player
    {
        private readonly RequestDelegate _next;
        private readonly ClientManager<TClient> _manager;
        private readonly ILogger<WebSocketMiddleware<TClient>> _logger;
        private readonly string _requestPath;

        /// <summary>
        /// The constructor. It handles a request when requestPath is equal to the request path.
        /// </summary>
        public WebSocketMiddleware(RequestDelegate next, ClientManager<TClient> manager, ILogger<WebSocketMiddleware<TClient>> logger, string requestPath)
        {
            _next = next;
            _manager = manager;
            _logger = logger;
            _requestPath = requestPath;
        }

        /// <summary>
        /// Checks request path and accepts a client. The web socket is added into the manager. The response is sent back when the client is disconnected.
        /// </summary>
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

        protected async Task Process(WebSocket socket)
        {
            var client = _manager.AddClient(socket);
            while (client.IsConnected)
            {
                await Task.Delay(1000);
            }
        }
    }
}

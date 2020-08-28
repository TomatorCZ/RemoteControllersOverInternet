using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Represents Client in a browser.
    /// </summary>
    public class User : Client
    {
        public User()
        {
            _websocket = new ClientWebSocket();
        }

        /// <summary>
        /// Connects and sends <see cref="InitialMessage">.
        /// </summary>
        public async Task ConnectAsync(Uri uri)
        {
            if (_websocket is ClientWebSocket _clientWebSocket)
            {
                _src = new CancellationTokenSource();
                await _clientWebSocket.ConnectAsync(uri, _src.Token);
            }
            else
                throw new NotImplementedException();
        }

        public async Task<ControllerEvent> ReceiveAsync() => await _messageManager.ReceiveAsync(_websocket);
    }
}

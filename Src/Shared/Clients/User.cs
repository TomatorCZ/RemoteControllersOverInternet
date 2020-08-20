using RemoteControllers;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControllers
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
        /// Connects and sends initial message.
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
    }
}

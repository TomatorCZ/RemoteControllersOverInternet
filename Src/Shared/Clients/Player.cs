using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Represents a client on server-side.
    /// </summary>
    public class Player : Client
    {
        public Guid Guid { get; }

        public Player(WebSocket socket)
        {
            Guid = Guid.NewGuid();
            _websocket = socket;
        }

        public async Task<InfoControllerEvent> ReceiveAsync() => new InfoControllerEvent(Guid, await _messageManager.ReceiveAsync(_websocket));
    }
}

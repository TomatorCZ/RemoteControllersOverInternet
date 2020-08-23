using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace RemoteController
{
    public class PlayerFactory : IClientFactory<Player>
    {
        public Player Create(WebSocket socket)
        {
            return new Player(socket);
        }
    }
}

using RemoteControllers;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace RemoteControllers
{
    /// <summary>
    /// Represents client on server-side.
    /// </summary>
    public class Player : Client
    {
        public Player(WebSocket socket)
        {
            _websocket = socket;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace RemoteController
{
    public interface IClientFactory<TClient> where TClient:Player
    {
        TClient Create(WebSocket socket);
    }
}

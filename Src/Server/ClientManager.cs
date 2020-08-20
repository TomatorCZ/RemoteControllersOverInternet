using Microsoft.AspNetCore.Http;
using RemoteControllers;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Manages all websocket connections with clients.
    /// </summary>
    public class ClientManager : IDisposable
    {
        public List<Player> Clients { get; }
        public List<Task<ControllerEvent>> _events;

        public ClientManager()
        {
            Clients = new List<Player>();
            _events = new List<Task<ControllerEvent>>();
        }

        public Client AddClient(WebSocket socket)
        {
            var newClient = new Player(socket);
            Clients.Add(newClient);
            _events.Add(newClient.ReceiveAsync());
            return newClient;
        }

        public async Task<InfoControllerEvent> RecieveEventAsync()
        {
            while (Clients.Count == 0)
            {
                await Task.Delay(25);
            }

            var result = await new Task<InfoControllerEvent>(() =>
            {
                int index = Task.WaitAny(_events.ToArray());

                var result = _events[index];
                _events[index] = Clients[index].ReceiveAsync();
                return new InfoControllerEvent(Clients[index], result.Result);
            });

            return result;
        }

        public async Task CloseAsync()
        {
            foreach (var client in Clients)
                await client.CloseAsync();
        }

        public void Dispose()
        {
            foreach (var client in Clients)
                client.Dispose();
        }
    }
}

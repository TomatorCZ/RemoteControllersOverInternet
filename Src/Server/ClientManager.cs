using Microsoft.AspNetCore.Http;
using RemoteController;
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
        private List<Player> _clients;
        private List<Task<ControllerEvent>> _events;


        public ClientManager()
        {
            _clients = new List<Player>();
            _events = new List<Task<ControllerEvent>>();
        }

        #region Add/Remove/Get/Count
        /// <summary>
        /// Adds a new client and starts to recieve his messages.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public Player AddClient(WebSocket socket)
        {
            var newClient = new Player(socket);
            _clients.Add(newClient);
            _events.Add(newClient.ReceiveAsync());

            return newClient;
        }

        /// <summary>
        /// Disconnects client.
        /// </summary>
        public async Task RemoveClient(int index)
        {
            if (_clients.Count < index && index >= 0)
            {
                await _clients[index].CloseAsync();

                _clients.RemoveAt(index);
            }
        }

        /// <summary>
        /// Gets a client with the index or null, if index is invalid.
        /// </summary>
        public Player GetClient(int index)
        {
            if (_clients.Count < index && index >= 0)
                return _clients[index];
            else
                return null;
        }

        public int ClientsCount() => _clients.Count;
        #endregion

        public async Task<InfoControllerEvent> RecieveEventAsync()
        {
            //Wait for first user.
            while (_clients.Count == 0)
                await Task.Delay(25);

            var result = await Wait();

            return result;
        }

        private async Task<InfoControllerEvent> Wait()
        {
            int index = Task.WaitAny(_events.ToArray());

            var result = await _events[index];
            _events[index] = _clients[index].ReceiveAsync();
            return new InfoControllerEvent(_clients[index], result);
        }

        public async Task CloseAsync()
        {
            foreach (var client in _clients)
                await client.CloseAsync();
        }

        public void Dispose()
        {
            foreach (var client in _clients)
                client.Dispose();
        }
    }
}

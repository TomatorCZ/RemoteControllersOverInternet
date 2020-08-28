using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Manages all websocket connections with clients.
    /// </summary>
    public class ClientManager<TClient> : IDisposable where TClient : Player
    {
        public Dictionary<Guid, TClient> Players { get; private set; }
        private List<Task<InfoControllerEvent>> _events;
        private object _playersLock = new object();
        private CancellationTokenSource _src = new CancellationTokenSource();
        private IClientFactory<TClient> _clientFactory;

        public bool IsDisposed { get; private set; } = false;
        public bool RemoveClientsAfterDisconnect { get; } = true;
        public event Action<TClient> OnClientDisconnect;

        public ClientManager(IClientFactory<TClient> clientFactory)
        {
            Players = new Dictionary<Guid, TClient>();
            _events = new List<Task<InfoControllerEvent>>();
            _clientFactory = clientFactory;
        }

        #region Add/Remove/Get/Count
        /// <summary>
        /// Adds a new client and starts to recieve his messages.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public TClient AddClient(WebSocket socket)
        {
            var newClient = _clientFactory.Create(socket);
            
            lock (_playersLock)
            {
                Players.Add(newClient.Guid, newClient);
                _events.Add(newClient.ReceiveAsync());
                _src.Cancel();
            }

            return newClient;
        }

        /// <summary>
        /// Disconnects a client with the id.
        /// </summary>
        public async Task RemoveClient(Guid id)
        {
            if (Players.TryGetValue(id, out TClient player))
            {
                await Players[id].CloseAsync();

                lock (_playersLock)
                    Players.Remove(id);
            }
        }

        /// <summary>
        /// Returns a client with the id or null, if index is invalid.
        /// </summary>
        public TClient GetClient(Guid id)
        {
            lock (_playersLock)
                return Players.ContainsKey(id) ? Players[id] : null;
        }

        public int ClientsCount()
        { 
            lock(_playersLock)
                return Players.Count;
        }
        #endregion

        /// <summary>
        /// Waits until an event is recieved by some client and it is returned with information about this client.
        /// </summary>
        /// <returns></returns>
        public async Task<InfoControllerEvent> RecieveEventAsync()
        {
            InfoControllerEvent result = default;

            while (true)
            {
                // Waits for first user.
                while (_events.Count == 0)
                    await Task.Delay(25);

                int index = -1;
                try
                {
                     index = Task.WaitAny(_events.ToArray(), _src.Token);
                }
                catch (OperationCanceledException)
                {
                    _src = new CancellationTokenSource();
                    continue;
                }
                
                result = await _events[index];

                if (GetClient(result.Sender).IsConnected)
                {
                    _events[index] = GetClient(result.Sender).ReceiveAsync();
                    break;
                }
                else
                {
                    lock (_playersLock)
                        _events.RemoveAt(index);

                    OnClientDisconnect?.Invoke(GetClient(result.Sender));

                    if (RemoveClientsAfterDisconnect)
                        await RemoveClient(result.Sender);
                }
            }

            return result;
        }

        /// <summary>
        /// Sends the event to the client. The id of event has to be contained in a configuration message, which is sent to the client.
        /// </summary>
        public async Task<bool> SendAsync(Guid id, ControllerEvent @event)
        {
            if (Players.TryGetValue(id, out TClient player))
            {
                await player.SendAsync(@event);
                return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Disconnects all clients.
        /// </summary>
        public async Task CloseAsync()
        {
            foreach (var client in Players)
            {
                await client.Value.CloseAsync();
                client.Value.Dispose();
            }
                
            
            lock(_playersLock)
                Players = new Dictionary<Guid, TClient>();
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var client in Players)
                client.Value.Dispose();
            _src.Dispose();

            IsDisposed = true;
        }
    }
}

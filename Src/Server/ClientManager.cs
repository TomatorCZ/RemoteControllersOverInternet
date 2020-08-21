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
    public class ClientManager : IDisposable
    {
        private Dictionary<Guid, Player> _players;
        private List<Task<InfoControllerEvent>> _events;
        private object _playersLock = new object();
        private CancellationTokenSource _src = new CancellationTokenSource();

        public bool IsDisposed { get; private set; } = false;
        public bool RemoveClientsAfterDisconnect { get; } = true;

        public event Action<Player> OnClientDisconnect;

        public ClientManager()
        {
            _players = new Dictionary<Guid, Player>();
            _events = new List<Task<InfoControllerEvent>>();
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
            
            lock (_playersLock)
            {
                _players.Add(newClient.Guid, newClient);
                _events.Add(newClient.ReceiveAsync());
                _src.Cancel();
            }

            return newClient;
        }

        /// <summary>
        /// Disconnects client.
        /// </summary>
        public async Task RemoveClient(Guid id)
        {
            if (_players.TryGetValue(id, out Player player))
            {
                await _players[id].CloseAsync();

                lock (_playersLock)
                    _players.Remove(id);
            }
        }

        /// <summary>
        /// Gets a client with the index or null, if index is invalid.
        /// </summary>
        public Player GetClient(Guid id)
        {
            lock (_playersLock)
                return _players.ContainsKey(id) ? _players[id] : null;
        }

        public int ClientsCount()
        { 
            lock(_playersLock)
                return _players.Count;
        }
        #endregion

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

        public async Task<bool> SendAsync(Guid id, ControllerEvent @event)
        {
            if (_players.TryGetValue(id, out Player player))
            {
                await player.SendAsync(@event);
                return true;
            }
            else
            {
                return true;
            }
        }

        public async Task CloseAsync()
        {
            foreach (var client in _players)
                await client.Value.CloseAsync();
            
            lock(_playersLock)
                _players = new Dictionary<Guid, Player>();
        }

        public void Dispose()
        {
            foreach (var client in _players)
                client.Value.Dispose();
            _src.Dispose();

            IsDisposed = true;
        }
    }
}

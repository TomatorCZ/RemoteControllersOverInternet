using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Represents endpoint of connection.
    /// </summary>
    public abstract class Client : IDisposable
    {
        protected WebSocket _websocket;
        protected MessageManager _messageManager;
        protected CancellationTokenSource _src;

        public bool IsDisposed {get; private set;} = false;
        public bool IsConnected { get => _websocket.State == WebSocketState.Open; }

        public Client()
        {
            _messageManager = new MessageManager(Encoding.UTF8);
        }

        #region Send/Recieve

        /// <summary>
        /// Sends an event. Event and it's id have to be contained in a configuration message, which is sent to server/client.
        /// </summary>
        public async Task<bool> SendAsync(ControllerEvent eventArgs) => await _messageManager.SendAsync(_websocket, eventArgs);

        /// <summary>
        /// Sends a initial message.
        /// </summary>
        public async Task<bool> SendAsync(InitialMessage msg) => await _messageManager.SendGreetingsAsync(_websocket);

        /// <summary>
        /// Sends a configuration message.
        /// </summary>
        public async Task<bool> SendAsync(ConfigurationMessage msg)
        {
            if (msg == null)
                throw new ArgumentNullException(nameof(msg));

            return await _messageManager.SendConfigurationAsync(_websocket, msg);
        } 
        #endregion

        /// <summary>
        /// Closes the web socket and <see cref="MessageManager"> with all running tasks.
        /// </summary>
        public async Task CloseAsync()
        {
            _src?.Cancel();

            _src = new CancellationTokenSource();

            if (_websocket.State == WebSocketState.Open)
                await _websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, _src.Token);
            
            _messageManager.Close();
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        public virtual void Dispose()
        {
            _src?.Cancel();
            _src?.Dispose();
            _websocket.Abort();
            _messageManager.Dispose();
            IsDisposed = true;
        }
    }
}

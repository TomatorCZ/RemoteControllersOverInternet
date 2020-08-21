using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteController
{
    public abstract class Client : IDisposable
    {
        public WebSocket _websocket;
        protected MessageManager _messageManager;
        protected CancellationTokenSource _src;

        public bool IsDisposed {get; private set;} = false;
        public bool IsConnected { get => _websocket.State == WebSocketState.Open; }

        public Client()
        {
            _messageManager = new MessageManager(Encoding.UTF8);
        }

        #region Send/Recieve
        public async Task<bool> SendAsync(ControllerEvent eventArgs) => await _messageManager.SendAsync(_websocket, eventArgs);

        public async Task<bool> SendAsync(InitialMessage msg) => await _messageManager.SendGreetingsAsync(_websocket);

        /// <summary>
        /// Sends configuration message.
        /// </summary>
        public async Task<bool> SendAsync(ConfigurationMessage msg)
        {
            if (msg == null)
                throw new ArgumentNullException(nameof(msg));

            return await _messageManager.SendConfigurationAsync(_websocket, msg);
        } 
        #endregion

        public async Task CloseAsync()
        {
            if (_src == null)
                _src = new CancellationTokenSource();
            else
                _src?.Cancel();

            if (_websocket.State == WebSocketState.Open)
                await _websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, _src.Token);
            _messageManager.Dispose();
        }

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

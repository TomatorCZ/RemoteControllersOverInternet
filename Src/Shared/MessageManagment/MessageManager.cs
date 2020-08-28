using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Represents a current state of <see cref="MessageManager"/>. e.g. When the manager is in Configuration state, it accepts only configuration messages. 
    /// </summary>
    public enum MessageManagerState
    {
        Initial = 1,
        Configuration = 2,
        Controllers = 3
    }

    /// <summary>
    /// The manager provides encoding, decoding, configuring messages which are sent and received by client or server.
    /// </summary>
    public class MessageManager : IDisposable
    {
        protected Encoding _encoding;
        protected CancellationTokenSource _src;
        protected MessageManagerState _state;
        protected ControllersEventManager _eventManager;
        protected ArraySegment<byte> _buffer;
        protected bool IsEventChannel = false;

        public bool IsDisposed { get; private set; } = false;
        
        /// <summary>
        /// It is called when a close message is received.
        /// </summary>
        public event Action OnCloseMessage;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="encoding">Encoding is used for encoding and decoding messages.</param>
        public MessageManager(Encoding encoding)
        {
            _encoding = encoding;
            _src = new CancellationTokenSource();
            _state = MessageManagerState.Initial;
            _buffer = WebSocket.CreateServerBuffer(10000);
        }

        #region Send

        /// <summary>
        /// Sends an initial message at beginning of communication.
        /// </summary>
        public async Task<bool> SendGreetingsAsync(WebSocket socket)
        {
            try
            {
                await socket.SendAsync(new ArraySegment<byte>(new ChangeStateMessage(MessageManagerState.Initial).Encode(_encoding)), WebSocketMessageType.Binary, true, _src.Token);
                await socket.SendAsync(new ArraySegment<byte>(new InitialMessage().Encode(_encoding)), WebSocketMessageType.Binary, true, _src.Token);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends a configuration message which sets binding between ids and types of event.
        /// </summary>
        public async Task<bool> SendConfigurationAsync(WebSocket socket, ConfigurationMessage msg)
        {
            try
            {
                IsEventChannel = false;
                await socket.SendAsync(new ArraySegment<byte>(new ChangeStateMessage(MessageManagerState.Configuration).Encode(_encoding)), WebSocketMessageType.Binary, true, _src.Token);                
                await socket.SendAsync(new ArraySegment<byte>(msg.Encode(_encoding)), WebSocketMessageType.Binary, true, _src.Token);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends an event to other endpoint. Id of event has to be noticed in <see cref="ConfigurationMessage">.
        /// </summary>
        public async Task<bool> SendAsync(WebSocket socket, ControllerEvent eventArgs)
        {
            try
            {
                if (!IsEventChannel)
                {
                    IsEventChannel = true;
                    await socket.SendAsync(new ArraySegment<byte>(new ChangeStateMessage(MessageManagerState.Controllers).Encode(_encoding)), WebSocketMessageType.Binary, true, _src.Token);
                }
                await socket.SendAsync(new ArraySegment<byte>(eventArgs.Encode(_encoding)), WebSocketMessageType.Binary, true, _src.Token);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Receive
        protected async Task<byte[]> ReceiveRawDataAsync(WebSocket socket)
        {
            // Recieve message
            using (var memory = new MemoryStream())
            {
                WebSocketReceiveResult result = null;
                do
                {
                    result = await socket.ReceiveAsync(_buffer, _src.Token);
                    memory.Write(_buffer.Array, 0, result.Count);

                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    return null;
                
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Recieves messages and handles them until the first event is recieved. The event is decoded and returned.
        /// </summary>
        public virtual async Task<ControllerEvent> ReceiveAsync(WebSocket socket)
        {
            byte[] message = await ReceiveRawDataAsync(socket);
            HandleChangeState(message);

            // Handles messages until controllers message mode starts.
            while (_state != MessageManagerState.Controllers && message != null)
            {
                if (!HandleChangeState(message))
                {
                    switch (_state)
                    {
                        case MessageManagerState.Configuration:
                            HandleConfiguration(message);
                            break;
                        case MessageManagerState.Initial:
                            HandleInitialMessage(message);
                            break;
                        default:
                            throw new Exception();
                    }
                }

                message = await ReceiveRawDataAsync(socket);
            }

            if (message == null)
            {
                OnCloseMessage?.Invoke();
                return null;
            }

            return HandleControllerEvent(message);
        }

        protected void HandleInitialMessage(byte[] data)
        {
            if (InitialMessage.TryDecode(data, _encoding, out InitialMessage msg))
            {
                return;
            }
            else
            {
                throw new InvalidDataException("Invalid initial message.");
            }
        }

        protected bool HandleChangeState(byte[] data)
        {
            if (ChangeStateMessage.TryDecode(data,_encoding,out ChangeStateMessage msg))
            {
                _state = msg.State;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected ControllerEvent HandleControllerEvent(byte[] data)
        {
            if (data == null || data.Length < 1)
                return null;

            if (_eventManager == null)
                throw new ConfigurationExpectedException();

            return _eventManager.GetEvent(data[0], data);
        }

        protected void HandleConfiguration(byte[] data)
        {
            if (ConfigurationMessage.TryDecode(data, _encoding, out ConfigurationMessage msg))
            {
                _eventManager = new ControllersEventManager(msg, _encoding);
            }
            else
            {
                throw new InvalidDataException("Invalid configuration message.");
            }
        }

        /// <summary>
        /// Calls cancel on all running tasks.
        /// </summary>
        public virtual void Close()
        {
            _src?.Cancel();
            _src = new CancellationTokenSource();
        }

        /// <summary>
        /// Cancels running tasks and disposes a token source.
        /// </summary>
        public virtual void Dispose()
        {
            _src?.Cancel();
            _src?.Dispose();
            IsDisposed = true;
        }
        #endregion

    }
}

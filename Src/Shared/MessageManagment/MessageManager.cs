using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteController
{
    /// <summary>
    /// Represents a state of connection.
    /// </summary>
    public enum MessageManagerState
    {
        Initial = 1,
        Configuration = 2,
        Controllers = 3
    }

    public class MessageManager : IDisposable
    {
        Encoding _encoding;
        CancellationTokenSource _src;
        MessageManagerState _state;
        ControllersEventManager _eventManager;
        byte[] _buffer;
        bool IsEventChannel = false;

        public bool IsDisposed { get; private set; } = false;
        public event Action OnCloseMessage;

        public MessageManager(Encoding encoding)
        {
            _encoding = encoding;
            _src = new CancellationTokenSource();
            _state = MessageManagerState.Initial;
            _buffer = new byte[1024];
        }

        #region Send
        public async Task SendGreetingsAsync(WebSocket socket)
        {
            await socket.SendAsync(new ArraySegment<byte>(new InitialMessage().Encode(_encoding)), WebSocketMessageType.Text, true, _src.Token);
        }

        public async Task SendConfigurationAsync(WebSocket socket, ConfigurationMessage msg)
        {
            await socket.SendAsync(new ArraySegment<byte>(new ChangeStateMessage(MessageManagerState.Configuration).Encode(_encoding)), WebSocketMessageType.Text, true, _src.Token);
            await socket.SendAsync(new ArraySegment<byte>(msg.Encode(_encoding)), WebSocketMessageType.Text, true, _src.Token);
        }

        public async Task SendAsync(WebSocket socket, ControllerEvent eventArgs)
        {
            if (!IsEventChannel)
            {
                IsEventChannel = true;
                await socket.SendAsync(new ArraySegment<byte>(new ChangeStateMessage(MessageManagerState.Controllers).Encode(_encoding)), WebSocketMessageType.Text, true, _src.Token);
            }
            await socket.SendAsync(new ArraySegment<byte>(eventArgs.Encode(_encoding)), WebSocketMessageType.Text, true, _src.Token);
        }
        #endregion

        #region Receive
        protected async Task<byte[]> ReceiveRawDataAsync(WebSocket socket)
        {
            var buffer = WebSocket.CreateServerBuffer(10000);

            // Recieve message
            using (var memory = new MemoryStream())
            {
                WebSocketReceiveResult result = null;
                do
                {
                    result = await socket.ReceiveAsync(buffer, _src.Token);

                    memory.Write(buffer.Array, 0, result.Count);

                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    return null;

                return memory.ToArray();
            }
        }

        public async Task<ControllerEvent> ReceiveAsync(WebSocket socket)
        {
            byte[] message = await ReceiveRawDataAsync(socket);

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
                Console.WriteLine("InitialMessage arrived");
                //TODO: Something..
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        protected bool HandleChangeState(byte[] data)
        {
            if (ChangeStateMessage.TryDecode(data,_encoding,out ChangeStateMessage msg))
            {
                Console.WriteLine("ChangeStateMessage arrived");
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

            return _eventManager.GetEvent(data[0], data);
        }

        protected void HandleConfiguration(byte[] data)
        {
            if (ConfigurationMessage.TryDecode(data, _encoding, out ConfigurationMessage msg))
            {
                Console.WriteLine("ConfigurationMessage arrived");
                _eventManager = new ControllersEventManager(msg);
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        public void Close()
        {
            _src?.Cancel();
        }

        public void Dispose()
        {
            _src?.Cancel();
            _src?.Dispose();
            IsDisposed = true;
        }
        #endregion

    }
}

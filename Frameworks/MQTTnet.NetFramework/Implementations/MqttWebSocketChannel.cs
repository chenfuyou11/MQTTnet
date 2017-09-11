﻿using MQTTnet.Core.Channel;
using MQTTnet.Core.Client;
using MQTTnet.Core.Exceptions;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MQTTnet.Implementations
{
    public sealed class MqttWebSocketChannel : IMqttCommunicationChannel, IDisposable
    {
        private ClientWebSocket _webSocket = new ClientWebSocket();
        
        public Stream Stream { get; private set; }

        public async Task ConnectAsync(MqttClientOptions options)
        {
            _webSocket = null;

            try
            {
                _webSocket = new ClientWebSocket();
                await _webSocket.ConnectAsync(new Uri(options.Server), CancellationToken.None);

                Stream = new WebSocketStream(_webSocket);
            }
            catch (WebSocketException exception)
            {
                throw new MqttCommunicationException(exception);
            }
        }

        public Task DisconnectAsync()
        {
            Stream = null;
            return _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        public void Dispose()
        {
            _webSocket?.Dispose();
        }
    }
}
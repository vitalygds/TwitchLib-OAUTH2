using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

namespace Twitch
{
    internal sealed class TwitchClientProvider : IChatProvider
    {
        private readonly object _gate = new object();
        public event Action<IChatProvider, ChatMessage> OnMessageReceived;
        private readonly List<string> _activeUsers;
        private readonly Client _client;
        private bool _wasConnected;
        public bool IsConnected => _client.IsConnected;

        public IReadOnlyList<string> ActiveUsers
        {
            get
            {
                lock (_gate)
                {
                    return _activeUsers;
                }
            }
        }

        public TwitchClientProvider()
        {
            _activeUsers = new List<string>();
            _client = CreateClient();
        }

        private Client CreateClient()
        {
            Client client = new Client();
            client.OnMessageReceived += ChatMessageReceived;
            client.OnUserJoined += UserJoined;
            client.OnUserLeft += UserLeft;
            return client;
        }

        public async UniTask<bool> ConnectAsync(CancellationToken token, ConnectionCredentials credentials, string channel)
        {
            if (!_client.IsInitialized)
            {
                _client.Initialize(credentials, channel);
                return await _client.ConnectAsync().AsUniTask().AttachExternalCancellation(token);
            }

            if (!_client.IsConnected)
            {
                _client.SetConnectionCredentials(credentials);
                await _client.ConnectAsync().AsUniTask().AttachExternalCancellation(token);
                await _client.JoinChannelAsync(channel);
                return _client.IsConnected;
            }

            return true;
        }

        public async UniTask DisconnectAsync(CancellationToken token)
        {
            if (_client.IsInitialized)
                await _client.DisconnectAsync().AsUniTask().AttachExternalCancellation(token);
        }


        private Task UserLeft(object sender, OnUserLeftArgs e)
        {
            lock (_gate)
            {
                _activeUsers.Remove(e.Username);
            }

            return Task.CompletedTask;
        }

        private Task UserJoined(object sender, OnUserJoinedArgs e)
        {
            lock (_gate)
            {
                _activeUsers.Add(e.Username);
            }

            return Task.CompletedTask;
        }

        private Task ChatMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            lock (_gate)
            {
                _activeUsers.Add(e.ChatMessage.Username);
            }

            OnMessageReceived?.Invoke(this, e.ChatMessage);
            return Task.CompletedTask;
        }
    }
}
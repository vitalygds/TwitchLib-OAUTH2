using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TwitchLib.Client.Models;
using UnityEngine;

namespace Twitch
{
    internal sealed class TwitchStreamingService
    {
        private readonly TwitchClientProvider _clientProvider;
        private readonly TwitchOAuth _auth;
        private bool _logged;

        public TwitchStreamingService(TwitchClientProvider clientProvider, TwitchConfig config)
        {
            _clientProvider = clientProvider;
            _auth = new TwitchOAuth(config.ClientId, config.Scopes);
        }
        
        public void PingInitialization() => PingLogAsync().Forget();

        public void ToggleLogin(CancellationToken token)
        {
            ToggleLoginAsync(token).Forget();
        }

        private async UniTask PingLogAsync() => await InitializeState(default);

        private async UniTask ToggleLoginAsync(CancellationToken token)
        {
            bool logged = await IsTokenValid(token);
            if (logged)
            {
                LogoutAsync(token).Forget();
            }
            else
            {
                LogInAsync(token).Forget();
            }
        }

        private async UniTask LogoutAsync(CancellationToken token)
        {
            if (_auth != null)
            {
                await _clientProvider.DisconnectAsync(token);
                TwitchOAuth.ClearToken();
            }

            _logged = false;
        }

        private async UniTask LogInAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested || _auth == null)
                return;
            bool initialized = await _auth.TryInitializeNewToken(token);
            if (initialized)
                await InitializeState(token);
        }

        private async UniTask<bool> InitializeClient(CancellationToken token)
        {
            if (_auth != null)
            {
                UserInfo myUserInfo = await TwitchOAuth.GetUserInfo(token);
                if (myUserInfo.Valid)
                {
                    ConnectionCredentials credentials;
                    try
                    {
                        credentials = new ConnectionCredentials(myUserInfo.Login, myUserInfo.OAuthToken);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        return false;
                    }

                    return await _clientProvider.ConnectAsync(token, credentials, myUserInfo.Login);
                }
            }

            return false;
        }

        private async UniTask InitializeState(CancellationToken token)
        {
            bool logged = false;
            if (!token.IsCancellationRequested)
            {
                logged = await IsTokenValid(token);
                if (logged)
                {
                    logged &= await InitializeClient(token);
                }
            }

            _logged = logged;
        }

        private async UniTask<bool> IsTokenValid(CancellationToken token)
        {
            if (_auth == null)
                return false;
            return await TwitchOAuth.CheckCurrentToken(token);
        }
    }
}
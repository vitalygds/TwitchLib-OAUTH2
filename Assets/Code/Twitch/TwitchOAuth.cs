using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Twitch
{
    internal sealed class TwitchOAuth
    {
        private const string TokenKey = "TwitchOAUTHToken";
        private const string TwitchContentType = "application/x-www-form-urlencoded";
        private const string DeviceURL = "https://id.twitch.tv/oauth2/device";
        private const string TokenURL = "https://id.twitch.tv/oauth2/token";
        private const string ActivateURL = "https://www.twitch.tv/activate?public=true&device-code={0}";
        private const string ValidateURL = "https://id.twitch.tv/oauth2/validate";
        private const int MillisecondsRequestTick = 500;
        private readonly string _clientId;
        private readonly string _scope;
        private string _authToken;

        public TwitchOAuth(string clientId, IEnumerable<string> scopes)
        {
            _clientId = clientId;
            _scope = string.Join("+", scopes);
        }

        public static void ClearToken()
        {
            PlayerPrefs.DeleteKey(TokenKey);
        }

        public static async UniTask<bool> CheckCurrentToken(CancellationToken token)
        {
            return (await GetUserInfo(token)).Valid;
        }

        public static async UniTask<UserInfo> GetUserInfo(CancellationToken token)
        {
            if (PlayerPrefs.HasKey(TokenKey))
            {
                string oauthToken = PlayerPrefs.GetString(TokenKey);
                TokenValidateResponse response = await CheckToken(token, oauthToken);
                if (response != null && !string.IsNullOrEmpty(response.Login))
                {
                    return new UserInfo(oauthToken, response.Login);
                }
            }

            return new UserInfo();
        }

        public async UniTask<bool> TryInitializeNewToken(CancellationToken token)
        {
            DeviceCodeResponse codeResponse = await RequestDeviceOAuth2(token, _clientId, _scope);
            if (codeResponse != null)
                Application.OpenURL(string.Format(ActivateURL, codeResponse.UserCode));
            else
                return false;
            TokenResponse tokenResponse = await RequestToken(token, _clientId, _scope, codeResponse);
            if (tokenResponse == null)
                return false;
            PlayerPrefs.SetString(TokenKey, tokenResponse.Token);
            return true;
        }

        private static HttpClient CreateClient()
        {
            HttpClient client = new HttpClient {Timeout = TimeSpan.FromMinutes(5)};
            return client;
        }

        private static async UniTask<TokenResponse> RequestToken(CancellationToken token, string clientId, string scope, DeviceCodeResponse code)
        {
            string content =
                $"client_id={clientId}&scopes={scope}&device_code={code.DeviceCode}&grant_type=urn:ietf:params:oauth:grant-type:device_code";
            using HttpClient client = CreateClient();
            return await POST<TokenResponse>(token, client, TokenURL, content, TwitchContentType);
        }

        private static async UniTask<TokenValidateResponse> CheckToken(CancellationToken token, string oauthToken)
        {
            using HttpClient client = CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {oauthToken}");
            return await GET<TokenValidateResponse>(token, client, ValidateURL);
        }

        private static async UniTask<DeviceCodeResponse> RequestDeviceOAuth2(CancellationToken token, string clientId, string scope)
        {
            string content = $"client_id={clientId}&scopes={scope}";
            using HttpClient client = CreateClient();
            return await POST<DeviceCodeResponse>(token, client, DeviceURL, content, TwitchContentType);
        }

        private static async UniTask<T> POST<T>(CancellationToken token, HttpClient client, string url, string content, string contentType)
        {
            StringContent request = new StringContent(content);
            request.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return await POST<T>(token, client, url, request);
        }

        private static async UniTask<T> POST<T>(CancellationToken token, HttpClient client, string url, StringContent request)
        {
            try
            {
                HttpResponseMessage response;
                bool keepPost;
                do
                {
                    response = await client.PostAsync(url, request, token);
                    keepPost = await AwaitIfBadRequest(token, response);
                }
                while (keepPost && !token.IsCancellationRequested);

                return await DeserializeObject<T>(response);
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"{ex}, inner -> {ex.InnerException}");
                return default;
            }
        }

        private static async UniTask<T> GET<T>(CancellationToken token, HttpClient client, string url)
        {
            try
            {
                HttpResponseMessage response;
                bool keepPost;
                do
                {
                    response = await client.GetAsync(url, token);
                    keepPost = await AwaitIfBadRequest(token, response);
                }
                while (keepPost && !token.IsCancellationRequested);

                return await DeserializeObject<T>(response);
            }
            catch (HttpRequestException ex)
            {
                Debug.LogWarning($"{ex}, inner -> {ex.InnerException}");
                return default;
            }
        }

        private static async UniTask<bool> AwaitIfBadRequest(CancellationToken token, HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    if (!token.IsCancellationRequested)
                        await UniTask.Delay(MillisecondsRequestTick, true, cancellationToken: token);
                    return true;
                case HttpStatusCode.OK:
                    return false;
                default:
                    string responseStr = await response.Content.ReadAsStringAsync().AsUniTask();
                    throw new HttpRequestException("Request is invalid", new Exception(responseStr));
            }
        }

        private static async UniTask<T> DeserializeObject<T>(HttpResponseMessage response)
        {
            string responseStr = await response.Content.ReadAsStringAsync().AsUniTask();
            return JsonConvert.DeserializeObject<T>(responseStr);
        }
    }
}
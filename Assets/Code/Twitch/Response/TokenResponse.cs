using Newtonsoft.Json;

namespace Twitch
{
    internal sealed class TokenResponse : TwitchResponse
    {
        [JsonProperty("access_token")] public string Token;
        [JsonProperty("expires_in")] public int ExpiresIn;
        [JsonProperty("refresh_token")] public string RefreshToken;
        [JsonProperty("scope")] public string[] Scope;
        [JsonProperty("token_type")] public string TokenType;
    }
}
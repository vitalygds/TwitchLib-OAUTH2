using Newtonsoft.Json;

namespace Twitch
{
    internal sealed class TokenValidateResponse : TwitchResponse
    {
        [JsonProperty("client_id")] public string ClientID;
        [JsonProperty("login")] public string Login;
        [JsonProperty("scopes")] public string[] Scopes;
        [JsonProperty("user_id")] public int UserID;
        [JsonProperty("expires_in")] public int ExpiresIn;
    }
}
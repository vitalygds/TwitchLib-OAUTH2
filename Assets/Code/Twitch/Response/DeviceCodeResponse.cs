using Newtonsoft.Json;

namespace Twitch
{
    internal sealed class DeviceCodeResponse : TwitchResponse
    {
        [JsonProperty("device_code")] public string DeviceCode;
        [JsonProperty("expires_in")] public int ExpiresIn;
        [JsonProperty("interval")] public int Interval;
        [JsonProperty("user_code")] public string UserCode;
        [JsonProperty("verification_uri")] public string VerificationUri;
    }
}
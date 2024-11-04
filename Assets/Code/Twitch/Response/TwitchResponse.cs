using Newtonsoft.Json;

namespace Twitch
{
    internal class TwitchResponse
    {
        [JsonProperty("status")] public string Status;
        [JsonProperty("message")] public string Message;
    }
}
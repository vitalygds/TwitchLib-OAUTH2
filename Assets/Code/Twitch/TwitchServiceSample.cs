using UnityEngine;

namespace Twitch
{
    public sealed class TwitchServiceSample : MonoBehaviour
    {
        [SerializeField] private TwitchConfig _config;
        private TwitchStreamingService _service;
        
        private void Start()
        {
            _service = new TwitchStreamingService(new TwitchClientProvider(), _config);
            _service.ToggleLogin(default);
        }
    }
}
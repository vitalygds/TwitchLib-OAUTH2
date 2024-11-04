using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Twitch
{
    [CreateAssetMenu(menuName = "Twitch/Config", fileName = "TwitchConfig", order = 0)]
    internal sealed class TwitchConfig : ScriptableObject
    {
        [SerializeField] private string[] _scopes;
        [SerializeField] private string _clientId = "";
        public string ClientId => _clientId;
        public IReadOnlyList<string> Scopes => _scopes;


#if UNITY_EDITOR
        [ContextMenu("Set full scope", false)]
        private void SetFullScope()
        {
            _scopes = GetScopes();
        }

        private static string[] GetScopes()
        {
            return new[]
            {
                TwitchOAuthScopes.Bits.Read.Scope, TwitchOAuthScopes.Channel.ManageBroadcast.Scope, TwitchOAuthScopes.Channel.ManagePolls.Scope,
                TwitchOAuthScopes.Channel.ManagePredictions.Scope, TwitchOAuthScopes.Channel.ManageRedemptions.Scope,
                TwitchOAuthScopes.Channel.ReadHypeTrain.Scope, TwitchOAuthScopes.Clips.Edit.Scope, TwitchOAuthScopes.User.ReadSubscriptions.Scope,
                TwitchOAuthScopes.Chat.Read.Scope, TwitchOAuthScopes.Chat.Edit.Scope,
            };
        }
#endif
    }
}
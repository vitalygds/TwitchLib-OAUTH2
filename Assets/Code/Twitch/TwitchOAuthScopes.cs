namespace Twitch
{
    internal sealed class TwitchOAuthScopes
    {
        public string Scope { get; }

        private TwitchOAuthScopes(string scope) => this.Scope = scope;

        public static class Channel
        {
            public static TwitchOAuthScopes ManagePolls => new TwitchOAuthScopes("channel:manage:polls");

            public static TwitchOAuthScopes ManagePredictions => new TwitchOAuthScopes("channel:manage:predictions");

            public static TwitchOAuthScopes ManageBroadcast => new TwitchOAuthScopes("channel:manage:broadcast");

            public static TwitchOAuthScopes ManageRedemptions => new TwitchOAuthScopes("channel:manage:redemptions");

            public static TwitchOAuthScopes ReadHypeTrain => new TwitchOAuthScopes("channel:read:hype_train");
        }

        public static class Chat
        {
            public static TwitchOAuthScopes Read => new TwitchOAuthScopes("chat:read");
            public static TwitchOAuthScopes Edit => new TwitchOAuthScopes("chat:edit");
        }

        public static class Clips
        {
            public static TwitchOAuthScopes Edit => new TwitchOAuthScopes("clips:edit");
        }

        public static class User
        {
            public static TwitchOAuthScopes ReadSubscriptions => new TwitchOAuthScopes("user:read:subscriptions");
        }

        public static class Bits
        {
            public static TwitchOAuthScopes Read => new TwitchOAuthScopes("bits:read");
        }
    }
}
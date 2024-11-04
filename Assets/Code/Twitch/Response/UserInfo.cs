namespace Twitch
{
    internal sealed class UserInfo
    {
        public readonly bool Valid;
        public readonly string OAuthToken;
        public readonly string Login;

        public UserInfo(string oAuthToken, string login)
        {
            Valid = true;
            OAuthToken = oAuthToken;
            Login = login;
        }
        public UserInfo()
        {
            Valid = false;
            OAuthToken = string.Empty;
            Login = string.Empty;
        }
        
    }
}
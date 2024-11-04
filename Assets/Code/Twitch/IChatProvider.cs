using System;
using TwitchLib.Client.Models;

namespace Twitch
{
    public interface IChatProvider
    {
        event Action<IChatProvider, ChatMessage> OnMessageReceived;
    }
}
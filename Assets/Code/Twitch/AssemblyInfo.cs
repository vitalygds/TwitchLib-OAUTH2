#if UNITY_EDITOR
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Twitch.Editor")]
[assembly: InternalsVisibleTo("Tests.PlayMode")]
[assembly: InternalsVisibleTo("Tests.EditorMode")]
#endif
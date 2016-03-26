using System.Collections.Generic;
using HotStats.ReplayParser;

namespace HotStats.Messaging.Messages
{
    public class GameModeChangedMessage
    {
        public GameModeChangedMessage(List<GameMode> gamemodes)
        {
            GameModes = gamemodes;
        }

        public List<GameMode> GameModes { get; private set; }
    }
}
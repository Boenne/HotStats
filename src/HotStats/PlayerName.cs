using System.Linq;
using HotStats.Properties;

namespace HotStats
{
    public class PlayerName
    {
        private static readonly string[] PlayerNames = Settings.Default.PlayerName.Split(';');

        public static bool Matches(string playerName)
        {
            return PlayerNames.Any(x => x == playerName);
        }
    }
}
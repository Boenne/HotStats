using System.Linq;
using HotStats.Properties;

namespace HotStats
{
    public class PlayerName
    {
        public static bool Matches(string playerName)
        {
            return Settings.Default.PlayerName.Split(';').Any(x => x == playerName);
        }
    }
}
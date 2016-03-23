namespace HotStats.Messaging.Messages
{
    public class HeroSelectedMessage
    {
        public string Hero { get; private set; }
        public string PlayerName { get; private set; }

        public HeroSelectedMessage(string hero, string playerName)
        {
            Hero = hero;
            PlayerName = playerName;
        }
    }
}
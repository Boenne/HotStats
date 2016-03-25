namespace HotStats.Messaging.Messages
{
    public class HeroSelectedMessage
    {
        public string Hero { get; private set; }

        public HeroSelectedMessage(string hero)
        {
            Hero = hero;
        }
    }
}
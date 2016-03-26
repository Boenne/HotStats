namespace HotStats.Messaging.Messages
{
    public class PlayerNameHasBeenSetMessage
    {
        public PlayerNameHasBeenSetMessage(string playerName)
        {
            PlayerName = playerName;
        }

        public string PlayerName { get; private set; }
    }
}
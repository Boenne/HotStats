namespace HotStats.Messaging.Messages
{
    public class SetPlayerNameMessage
    {
        public SetPlayerNameMessage(string playerName)
        {
            PlayerName = playerName;
        }

        public string PlayerName { get; private set; }
    }
}
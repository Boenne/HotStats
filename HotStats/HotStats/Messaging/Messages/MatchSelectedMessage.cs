using System;

namespace HotStats.Messaging.Messages
{
    public class MatchSelectedMessage
    {
        public MatchSelectedMessage(DateTime timestamp)
        {
            Timestamp = timestamp;
        }

        public DateTime Timestamp { get; private set; }
    }
}
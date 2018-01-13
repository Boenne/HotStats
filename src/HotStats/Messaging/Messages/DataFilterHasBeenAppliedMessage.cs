namespace HotStats.Messaging.Messages
{
    public class DataFilterHasBeenAppliedMessage
    {
        //TODO
        //This won't work.
        // If HeroSelected = true but another filter (e.g. map selected) has been applied the event is still dicarded
        public bool HeroSelected { get; set; }
    }
}
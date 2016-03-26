using System;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class MatchDetailsViewModel : IMatchDetailsViewModel
    {
        private readonly IReplayRepository replayRepository;

        public MatchDetailsViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.replayRepository = replayRepository;
            messenger.Register<MatchSelectedMessage>(this, message => GetDetails(message.Timestamp));
        }

        public void GetDetails(DateTime timestamp)
        {
        }
    }
}
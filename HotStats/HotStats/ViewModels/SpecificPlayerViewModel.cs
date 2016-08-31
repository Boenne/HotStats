using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotStats.Messaging;
using HotStats.Services.Interfaces;

namespace HotStats.ViewModels
{
    public class SpecificPlayerViewModel
    {
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;

        public SpecificPlayerViewModel(IReplayRepository replayRepository, IMessenger messenger)
        {
            this.replayRepository = replayRepository;
            this.messenger = messenger;
        }

        public void GetDataAsync()
        {
            
        }

        public void GetData()
        {
            var replays = replayRepository.GetFilteredReplays();
            foreach (var replay in replays)
            {
                
            }
        }

    }
}
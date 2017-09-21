using System.Collections.Generic;
using Heroes.ReplayParser;
using HotStats.Services.Interfaces;

namespace HotStats.Services
{
    public class ReplayRepository : IReplayRepository
    {
        private IList<Replay> filteredReplays;
        private IList<Replay> replays;

        public IList<Replay> GetReplays()
        {
            return replays;
        }

        public IList<Replay> GetFilteredReplays()
        {
            return filteredReplays;
        }

        public void SaveReplays(IList<Replay> replays)
        {
            this.replays = replays;
        }

        public void SaveFilteredReplays(IList<Replay> filteredReplays)
        {
            this.filteredReplays = filteredReplays;
        }
    }
}
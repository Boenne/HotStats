using System.Collections.Generic;
using Heroes.ReplayParser;
using HotStats.Services.Interfaces;

namespace HotStats.Services
{
    public class ReplayRepository : IReplayRepository
    {
        private IEnumerable<Replay> filteredReplays;
        private IEnumerable<Replay> replays;

        public IEnumerable<Replay> GetReplays()
        {
            return replays;
        }

        public IEnumerable<Replay> GetFilteredReplays()
        {
            return filteredReplays;
        }

        public void SaveReplays(IEnumerable<Replay> replays)
        {
            this.replays = replays;
        }

        public void SaveFilteredReplays(IEnumerable<Replay> filteredReplays)
        {
            this.filteredReplays = filteredReplays;
        }
    }
}
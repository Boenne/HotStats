using System.Collections.Generic;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;

namespace HotStats.Services
{
    public class ReplayRepository : IReplayRepository
    {
        private List<Replay> replays;

        public List<Replay> GetReplays()
        {
            return replays;
        }

        public void SaveReplays(List<Replay> replays)
        {
            this.replays = replays;
        }
    }
}
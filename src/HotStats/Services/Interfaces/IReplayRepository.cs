using System.Collections.Generic;
using Heroes.ReplayParser;

namespace HotStats.Services.Interfaces
{
    public interface IReplayRepository
    {
        IList<Replay> GetReplays();
        IList<Replay> GetFilteredReplays();
        void SaveReplays(IList<Replay> replays);
        void SaveFilteredReplays(IList<Replay> filteredReplays);
    }
}
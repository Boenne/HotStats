using System.Collections.Generic;
using HotStats.ReplayParser;

namespace HotStats.Services.Interfaces
{
    public interface IReplayRepository
    {
        List<Replay> GetReplays();
        void SaveReplays(List<Replay> replays);
    }
}
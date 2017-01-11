﻿using System.Collections.Generic;
using Heroes.ReplayParser;

namespace HotStats.Services.Interfaces
{
    public interface IReplayRepository
    {
        IEnumerable<Replay> GetReplays();
        IEnumerable<Replay> GetFilteredReplays();
        void SaveReplays(IEnumerable<Replay> replays);
        void SaveFilteredReplays(IEnumerable<Replay> filteredReplays);
    }
}
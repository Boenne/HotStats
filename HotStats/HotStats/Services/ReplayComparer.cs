using System.Collections.Generic;
using Heroes.ReplayParser;

namespace HotStats.Services
{
    public class ReplayComparer : IEqualityComparer<Replay>
    {
        public bool Equals(Replay x, Replay y)
        {
            return x.Timestamp == y.Timestamp;
        }

        public int GetHashCode(Replay obj)
        {
            return obj.Timestamp.GetHashCode();
        }
    }
}
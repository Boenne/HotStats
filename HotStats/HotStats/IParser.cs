using System.Threading.Tasks;
using HotStats.ReplayParser;

namespace HotStats
{
    public interface IParser
    {
        Task<Replay> ParseAsync(string path);
    }
}
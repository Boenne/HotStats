using System.Threading.Tasks;
using HotStats.ReplayParser;

namespace HotStats.Services.Interfaces
{
    public interface IParser
    {
        Task<Replay> ParseAsync(string path);
    }
}
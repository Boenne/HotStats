using System.Threading.Tasks;
using Heroes.ReplayParser;

namespace HotStats.Services.Interfaces
{
    public interface IParser
    {
        Task<Replay> ParseAsync(string path);
    }
}
using System.Diagnostics;
using System.Threading.Tasks;
using Heroes.ReplayParser;
using HotStats.Services.Interfaces;

namespace HotStats.Services
{
    public class Parser : IParser
    {
        public Task<Replay> ParseAsync(string path)
        {
            return Task.Factory.StartNew(() => Parse(path));
        }

        public Replay Parse(string path)
        {
            var replayParseResult = DataParser.ParseReplay(path, false, false, false);

            if (replayParseResult.Item1 != DataParser.ReplayParseResult.Success) return null;
            var replay = replayParseResult.Item2;
            return replay;
        }
    }
}
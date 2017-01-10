using System;
using System.Threading.Tasks;
using System.Windows;
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
            Replay replay = null;
            try
            {
                // Ignore errors can be set to true if you want to attempt to parse currently unsupported replays
                var replayParseResult = DataParser.ParseReplay(path, false, false);

                if (replayParseResult.Item1 != DataParser.ReplayParseResult.Success) return null;
                replay = replayParseResult.Item2;
                return replay;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return replay;
        }
    }
}
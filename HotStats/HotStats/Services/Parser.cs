using System;
using System.Threading.Tasks;
using System.Windows;
using HotStats.ReplayParser;
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
            // Use temp directory for MpqLib directory permissions requirements
            //var tmpPath = Path.GetTempFileName();
            //File.Copy(path, tmpPath, true);
            Replay replay = null;
            try
            {
                // Attempt to parse the replay
                // Ignore errors can be set to true if you want to attempt to parse currently unsupported replays, such as 'VS AI' or 'PTR Region' replays
                var replayParseResult = DataParser.ParseReplay(path, false, false);

                // If successful, the Replay object now has all currently available information
                if (replayParseResult.Item1 == DataParser.ReplayParseResult.Success)
                {
                    replay = replayParseResult.Item2;
                    return replay;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                //if (File.Exists(tmpPath))
                //    File.Delete(tmpPath);
            }
            return replay;
        }
    }
}
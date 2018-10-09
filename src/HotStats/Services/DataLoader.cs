using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Heroes.ReplayParser;
using HotStats.Services.Interfaces;
using Newtonsoft.Json;

namespace HotStats.Services
{
    public class DataLoader : IDataLoader
    {
        private readonly IParser parser;
        private readonly IReplayRepository replayRepository;

        public DataLoader(IParser parser, IReplayRepository replayRepository)
        {
            this.parser = parser;
            this.replayRepository = replayRepository;
        }

        public async Task LoadDataAsync(Func<int, Task> preLoopAction = null, Func<long, Task> forEachAction = null)
        {
            var heroesAccountsFolder = new DirectoryInfo(FilePaths.MyDocuments);

            var replayFiles = heroesAccountsFolder.GetFiles("*.StormReplay", SearchOption.AllDirectories);

            var replays = await GetReplaysFromDataFile();
            if (preLoopAction != null)
                await preLoopAction(replays.Count);

            var watch = Stopwatch.StartNew();

            foreach (var replayFile in replayFiles)
            {
                //If replay is not already in the data file
                if (replays.All(x => x.FileCreationDate != replayFile.CreationTime && x.FileName != replayFile.Name))
                {
                    var replay = await parser.ParseAsync(replayFile.FullName);
                    if (replay == null) continue;
                    replay.FileCreationDate = replayFile.CreationTime;
                    replay.FileName = replayFile.Name;
                    replay.ClientListByUserID = null;
                    replay.ClientListByWorkingSetSlotID = null;
                    replays.Add(replay);
                }
                if (forEachAction != null)
                    await forEachAction(watch.ElapsedMilliseconds);
            }
            watch.Stop();

            await Task.Run(() =>
            {
                replayRepository.SaveReplays(replays);
                var json = JsonConvert.SerializeObject(replays);
                File.WriteAllText(FilePaths.Data, json);
            });
        }

        public Task<List<Replay>> GetReplaysFromDataFile()
        {
            return Task.Run(() =>
            {
                if (!File.Exists(FilePaths.Data)) return new List<Replay>();
                var replays = JsonConvert.DeserializeObject<List<Replay>>(File.ReadAllText(FilePaths.Data));
                return replays;
            });
        }
    }

    public interface IDataLoader
    {
        Task LoadDataAsync(Func<int, Task> preLoopAction = null, Func<long, Task> forEachAction = null);
        Task<List<Replay>> GetReplaysFromDataFile();
    }
}
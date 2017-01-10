using System;
using System.Collections.Generic;
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

        public async Task LoadDataAsync()
        {
            var heroesAccountsFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Heroes of the Storm\Accounts");
            var heroesAccountsFolder = new DirectoryInfo(heroesAccountsFolderPath);

            var replayFiles = heroesAccountsFolder.GetFiles("*.StormReplay", SearchOption.AllDirectories);

            var replays = await GetReplaysFromDataFile();

            foreach (var replayFile in replayFiles)
            {
                //If replay is already in the data file
                if (!replays.All(x => x.FileCreationDate != replayFile.CreationTime && x.FileName != replayFile.Name))
                    continue;
                var replay = await parser.ParseAsync(replayFile.FullName);
                if (replay == null) continue;
                replay.FileCreationDate = replayFile.CreationTime;
                replay.FileName = replayFile.Name;
                replay.ClientListByUserID = null;
                replay.ClientListByWorkingSetSlotID = null;
                replays.Add(replay);
            }
            replayRepository.SaveReplays(replays);
            var json = JsonConvert.SerializeObject(replays);
            File.WriteAllText(Environment.CurrentDirectory + "/data.txt", json);
        }

        public Task<List<Replay>> GetReplaysFromDataFile()
        {
            return Task.Factory.StartNew(() =>
            {
                var path = Environment.CurrentDirectory + "/data.txt";
                if (!File.Exists(path)) return new List<Replay>();
                var replays = JsonConvert.DeserializeObject<List<Replay>>(File.ReadAllText(path));
                return replays;
            });
        }
    }

    public interface IDataLoader
    {
        Task LoadDataAsync();
    }
}
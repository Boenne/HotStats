using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Heroes.ReplayParser;
using HotStats.Messaging;
using HotStats.Navigation;
using HotStats.Services;
using HotStats.Services.Interfaces;
using HotStats.Wrappers;
using Newtonsoft.Json;

namespace HotStats.ViewModels
{
    public class LoadDataViewModel : ViewModelBase, ILoadDataViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IHeroDataRepository heroDataRepository;
        private readonly INavigationService navigationService;
        private readonly IParser parser;
        private readonly IReplayRepository replayRepository;
        private bool anyFilesToProcess = true;
        private long approxTimeLeft;
        private long elapsedTime;
        private int fileCount;
        private int filesProcessed;

        public LoadDataViewModel(IParser parser, IReplayRepository replayRepository,
            INavigationService navigationService, IMessenger messenger,
            IDispatcherWrapper dispatcherWrapper, IHeroDataRepository heroDataRepository)
            : base(messenger)
        {
            this.parser = parser;
            this.replayRepository = replayRepository;
            this.navigationService = navigationService;
            this.dispatcherWrapper = dispatcherWrapper;
            this.heroDataRepository = heroDataRepository;
        }

        public RelayCommand LoadedCommand => new RelayCommand(() => LoadData());

        public bool AnyFilesToProcess
        {
            get => anyFilesToProcess;
            set { Set(() => AnyFilesToProcess, ref anyFilesToProcess, value); }
        }

        public int FilesProcessed
        {
            get => filesProcessed;
            set { Set(() => FilesProcessed, ref filesProcessed, value); }
        }

        public int FileCount
        {
            get => fileCount;
            set { Set(() => FileCount, ref fileCount, value); }
        }

        public long ElapsedTime
        {
            get => elapsedTime;
            set { Set(() => ElapsedTime, ref elapsedTime, value); }
        }

        public long ApproxTimeLeft
        {
            get => approxTimeLeft;
            set { Set(() => ApproxTimeLeft, ref approxTimeLeft, value); }
        }

        public async Task LoadData()
        {
            var heroesAccountsFolder = new DirectoryInfo(FilePaths.MyDocuments);

            List<Replay> replays;

            if (heroesAccountsFolder.Exists)
            {
                replays = await GetReplaysFromDataFile();
                var replayFiles = heroesAccountsFolder.GetFiles("*.StormReplay", SearchOption.AllDirectories);

                await dispatcherWrapper.BeginInvoke(() => FileCount = replayFiles.Length);

                var watch = Stopwatch.StartNew();
                foreach (var replayFile in replayFiles)
                {
                    if (replays.All(x =>
                        x.FileCreationDate != replayFile.CreationTime && x.FileName != replayFile.Name))
                    {
                        var replay = await parser.ParseAsync(replayFile.FullName);
                        if (replay != null)
                        {
                            replay.FileCreationDate = replayFile.CreationTime;
                            replay.FileName = replayFile.Name;
                            replay.ClientListByUserID = null;
                            replay.ClientListByWorkingSetSlotID = null;
                            replays.Add(replay);
                        }
                    }
                    await dispatcherWrapper.BeginInvoke(() =>
                    {
                        FilesProcessed++;
                        ElapsedTime = watch.ElapsedMilliseconds;
                        ApproxTimeLeft = ElapsedTime / FilesProcessed * (FileCount - FilesProcessed);
                    });
                }
                watch.Stop();
            }
            else
            {
                AnyFilesToProcess = false;
                replays = await GetReplaysFromDataFile();
            }

            SaveReplays(replays);
            LoadHeroData();

            navigationService.NavigateTo(NavigationFrames.DownloadPortraits);
        }

        public void SaveReplays(List<Replay> replays)
        {
            replayRepository.SaveReplays(replays);
            var json = JsonConvert.SerializeObject(replays);
            if (!Directory.Exists(FilePaths.DataDir))
                Directory.CreateDirectory(FilePaths.DataDir);
            File.WriteAllText(FilePaths.Data, json);
        }

        public void LoadHeroData()
        {
            if (!File.Exists(FilePaths.HeroData)) return;
            var heroDataJson = File.ReadAllText(FilePaths.HeroData);
            var heroData = JsonConvert.DeserializeObject<List<Hero>>(heroDataJson);
            heroDataRepository.SaveData(heroData);
        }

        public Task<List<Replay>> GetReplaysFromDataFile()
        {
            return Task.Factory.StartNew(() =>
            {
                if (!File.Exists(FilePaths.Data)) return new List<Replay>();
                var replays = JsonConvert.DeserializeObject<List<Replay>>(File.ReadAllText(FilePaths.Data));
                return replays;
            });
        }
    }

    public interface ILoadDataViewModel
    {
        RelayCommand LoadedCommand { get; }
        bool AnyFilesToProcess { get; set; }
        int FilesProcessed { get; set; }
        int FileCount { get; set; }
        long ElapsedTime { get; set; }
        long ApproxTimeLeft { get; set; }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Navigation;
using HotStats.Properties;
using HotStats.Services;
using HotStats.Services.Interfaces;
using HotStats.Wrappers;
using Newtonsoft.Json;

namespace HotStats.ViewModels
{
    public class LoadDataViewModel : ViewModelBase, ILoadDataViewModel
    {
        private readonly IDataLoader dataLoader;
        private readonly IInternetConnectivityService internetConnectivityService;
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IHeroDataRepository heroDataRepository;
        private readonly INavigationService navigationService;
        private readonly IReplayRepository replayRepository;
        private bool anyFilesToProcess = true;
        private long approxTimeLeft;
        private long elapsedTime;
        private int fileCount;
        private int filesProcessed;

        public LoadDataViewModel(IReplayRepository replayRepository,
            INavigationService navigationService, IMessenger messenger,
            IDispatcherWrapper dispatcherWrapper, IHeroDataRepository heroDataRepository,
            IDataLoader dataLoader, IInternetConnectivityService internetConnectivityService)
            : base(messenger)
        {
            this.replayRepository = replayRepository;
            this.navigationService = navigationService;
            this.dispatcherWrapper = dispatcherWrapper;
            this.heroDataRepository = heroDataRepository;
            this.dataLoader = dataLoader;
            this.internetConnectivityService = internetConnectivityService;
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
            CreateDataDirectory();
            var heroesAccountsFolder = new DirectoryInfo(FilePaths.MyDocuments);

            if (heroesAccountsFolder.Exists)
            {
                await dataLoader.LoadDataAsync(
                    replayCount => dispatcherWrapper.BeginInvoke(() => FileCount = replayCount),
                    time =>
                        dispatcherWrapper.BeginInvoke(() =>
                        {
                            FilesProcessed++;
                            ElapsedTime = time;
                            ApproxTimeLeft = ElapsedTime / FilesProcessed * (FileCount - FilesProcessed);
                        })
                );
            }
            else
            {
                AnyFilesToProcess = false;
                var replays = await dataLoader.GetReplaysFromDataFile();
                replayRepository.SaveReplays(replays);
            }

            await LoadHeroData();

            navigationService.NavigateTo(internetConnectivityService.IsOnline()
                ? NavigationFrames.CheckVersion
                : NavigationFrames.MainPage);
        }

        public void CreateDataDirectory()
        {
            if (!Directory.Exists(FilePaths.DataDir))
                Directory.CreateDirectory(FilePaths.DataDir);
        }

        public Task LoadHeroData()
        {
            return Task.Run(() =>
            {
                if (!File.Exists(FilePaths.HeroData)) return;
                var heroDataJson = File.ReadAllText(FilePaths.HeroData);
                var heroData = JsonConvert.DeserializeObject<List<Hero>>(heroDataJson);
                heroDataRepository.SaveData(heroData);
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
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Heroes.ReplayParser;
using HotStats.Messaging;
using HotStats.Navigation;
using HotStats.Services.Interfaces;
using HotStats.Wrappers;
using Newtonsoft.Json;

namespace HotStats.ViewModels
{
    public class LoadDataViewModel : ViewModelBase, ILoadDataViewModel
    {
        private readonly INavigationService navigationService;
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IParser parser;
        private readonly IReplayRepository replayRepository;
        private bool anyFilesToProcess = true;
        private long approxTimeLeft;
        private long elapsedTime;
        private int fileCount;
        private int filesProcessed;

        public LoadDataViewModel(IParser parser, IReplayRepository replayRepository,
            INavigationService navigationService, IMessenger messenger,
            IDispatcherWrapper dispatcherWrapper)
            : base(messenger)
        {
            this.parser = parser;
            this.replayRepository = replayRepository;
            this.navigationService = navigationService;
            this.dispatcherWrapper = dispatcherWrapper;
        }

        public RelayCommand LoadedCommand => new RelayCommand(async () => await Loaded());
        
        public bool AnyFilesToProcess
        {
            get { return anyFilesToProcess; }
            set { Set(() => AnyFilesToProcess, ref anyFilesToProcess, value); }
        }

        public int FilesProcessed
        {
            get { return filesProcessed; }
            set { Set(() => FilesProcessed, ref filesProcessed, value); }
        }

        public int FileCount
        {
            get { return fileCount; }
            set { Set(() => FileCount, ref fileCount, value); }
        }

        public long ElapsedTime
        {
            get { return elapsedTime; }
            set { Set(() => ElapsedTime, ref elapsedTime, value); }
        }

        public long ApproxTimeLeft
        {
            get { return approxTimeLeft; }
            set { Set(() => ApproxTimeLeft, ref approxTimeLeft, value); }
        }

        public async Task Loaded()
        {
            await LoadData();
        }

        public async Task LoadData()
        {
            FilesProcessed = 0;
            ElapsedTime = 0;
            ApproxTimeLeft = 0;

            var heroesAccountsFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Heroes of the Storm\Accounts");
            var heroesAccountsFolder = new DirectoryInfo(heroesAccountsFolderPath);

            List<Replay> replays;

            if (heroesAccountsFolder.Exists)
            {
                replays = await GetReplaysFromDataFile();
                var replayFiles = heroesAccountsFolder.GetFiles("*.StormReplay", SearchOption.AllDirectories);
                
                await dispatcherWrapper.BeginInvoke(() => FileCount = replayFiles.Length);

                var watch = Stopwatch.StartNew();
                foreach (var replayFile in replayFiles)
                {
                    watch.Restart();
                    if (replays.All(x => x.FileCreationDate != replayFile.CreationTime && x.FileName != replayFile.Name))
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
                    await dispatcherWrapper.BeginInvoke(() => FilesProcessed++);
                    watch.Stop();
                    await dispatcherWrapper.BeginInvoke(() => ApproxTimeLeft = ElapsedTime / FilesProcessed * (FileCount - FilesProcessed));
                    await dispatcherWrapper.BeginInvoke(() => ElapsedTime += watch.ElapsedMilliseconds);
                }
            }
            else
            {
                AnyFilesToProcess = false;
                replays = await GetReplaysFromDataFile();
            }
            replayRepository.SaveReplays(replays);
            var json = JsonConvert.SerializeObject(replays);
            File.WriteAllText(Environment.CurrentDirectory + "/data.txt", json);
            navigationService.NavigateTo(NavigationFrames.DownloadPortraits);
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
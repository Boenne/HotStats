using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
using HotStats.ReplayParser;
using HotStats.Services;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;
using Newtonsoft.Json;

namespace HotStats.ViewModels
{
    public class LoadDataViewModel : ObservableObject, ILoadDataViewModel
    {
        private readonly IMessenger messenger;
        private readonly IParser parser;
        private readonly IReplayRepository replayRepository;
        private long approxTimeLeft;
        private long elapsedTime;
        private int fileCount;
        private int filesProcessed;
        private bool isLoading;
        private string playerName;
        private bool playerNameIsSet;

        public LoadDataViewModel(IParser parser, IMessenger messenger, IReplayRepository replayRepository)
        {
            this.parser = parser;
            this.messenger = messenger;
            this.replayRepository = replayRepository;
        }

        public ICommand LoadDataCommand => new RelayCommand(LoadData);

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                OnPropertyChanged();
            }
        }

        public int FilesProcessed
        {
            get { return filesProcessed; }
            set
            {
                filesProcessed = value;
                OnPropertyChanged();
            }
        }

        public int FileCount
        {
            get { return fileCount; }
            set
            {
                fileCount = value;
                OnPropertyChanged();
            }
        }

        public long ElapsedTime
        {
            get { return elapsedTime; }
            set
            {
                elapsedTime = value;
                OnPropertyChanged();
            }
        }

        public long ApproxTimeLeft
        {
            get { return approxTimeLeft; }
            set
            {
                approxTimeLeft = value;
                OnPropertyChanged();
            }
        }

        public string PlayerName
        {
            get { return playerName; }
            set
            {
                playerName = value;
                OnPropertyChanged();
            }
        }

        public bool PlayerNameIsSet
        {
            get { return playerNameIsSet; }
            set
            {
                playerNameIsSet = value;
                OnPropertyChanged();
            }
        }

        public ICommand SetPlayerNameCommand => new RelayCommand(SetPlayerName);
        public ICommand LoadedCommand => new RelayCommand(StartUp);

        public async void LoadData()
        {
            IsLoading = true;
            FilesProcessed = 0;
            ElapsedTime = 0;
            ApproxTimeLeft = 0;

            var heroesAccountsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Heroes of the Storm\Accounts");
            var replayFiles = Directory.GetFiles(heroesAccountsFolder, "*.StormReplay", SearchOption.AllDirectories);

            FileCount = replayFiles.Length;

            var replays = new List<Replay>();
            var watch = Stopwatch.StartNew();
            foreach (var replayFile in replayFiles)
            {
                watch.Restart();

                var replay = await parser.ParseAsync(replayFile);
                if (replay != null)
                {
                    replay.ClientList = null;
                    replays.Add(replay);
                }
                FilesProcessed++;
                watch.Stop();
                ElapsedTime += watch.ElapsedMilliseconds;
                ApproxTimeLeft = (ElapsedTime / FilesProcessed) * (FileCount - FilesProcessed);
            }
            replays = MergeReplays(replays);
            replayRepository.SaveReplays(replays);
            var json = JsonConvert.SerializeObject(replays);
            File.WriteAllText(Environment.CurrentDirectory + "/data.txt", json);
        }

        public List<Replay> MergeReplays(List<Replay> replays)
        {
            var path = Environment.CurrentDirectory + "/data.txt";
            if (!File.Exists(path)) return replays;
            var existingReplays = JsonConvert.DeserializeObject<List<Replay>>(File.ReadAllText(path));
            var enumerable = replays.Union(existingReplays, new ReplayComparer()).ToList();
            return enumerable;
        }

        public void SetPlayerName()
        {
            if (string.IsNullOrEmpty(PlayerName)) return;
            PlayerNameIsSet = true;
            Settings.Default.PlayerName = PlayerName;
            Settings.Default.Save();
            messenger.Send(new PlayerNameHasBeenSetMessage(PlayerName));
        }

        public void StartUp()
        {
            PlayerName = Settings.Default.PlayerName;
        }
    }
}
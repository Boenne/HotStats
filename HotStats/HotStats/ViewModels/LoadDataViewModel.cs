using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;
using Newtonsoft.Json;

namespace HotStats.ViewModels
{
    public class LoadDataViewModel : ObservableObject, ILoadDataViewModel
    {
        private readonly IParser parser;
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private long approxTimeLeft;
        private long elapsedTime;
        private int fileCount;
        private int filesProcessed;
        private bool isLoading;

        public LoadDataViewModel(IParser parser, IMessenger messenger, IReplayRepository replayRepository)
        {
            this.parser = parser;
            this.messenger = messenger;
            this.replayRepository = replayRepository;
        }

        public ICommand LoadDataCommand => new DelegateCommand(LoadData);

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
                    replays.Add(replay);
                }
                FilesProcessed++;

                watch.Stop();
                ElapsedTime += watch.ElapsedMilliseconds;
                ApproxTimeLeft = (ElapsedTime / FilesProcessed) * (FileCount - FilesProcessed);
            }
            replayRepository.SaveReplays(replays);
            var json = JsonConvert.SerializeObject(replays);
            File.WriteAllText(Environment.CurrentDirectory + "/data.txt", json);

            messenger.Send(new DataHasBeenLoadedMessage());
        }
    }
}
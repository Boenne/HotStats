﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
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
        private readonly IReplayRepository replayRepository;
        private readonly INavigationService navigationService;
        private long approxTimeLeft;
        private long elapsedTime;
        private int fileCount;
        private int filesProcessed;
        private bool isLoading;

        public LoadDataViewModel(IParser parser, IMessenger messenger, IReplayRepository replayRepository, INavigationService navigationService)
        {
            this.parser = parser;
            this.replayRepository = replayRepository;
            this.navigationService = navigationService;
            messenger.Register<RefreshDataMessage>(this, async message =>
            {
                await LoadData();
                messenger.Send(new DataHasBeenRefreshedMessage());
            });
        }

        public ICommand LoadDataCommand => new RelayCommand(async () => await LoadData());

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

        public async Task LoadData()
        {
            IsLoading = true;
            FilesProcessed = 0;
            ElapsedTime = 0;
            ApproxTimeLeft = 0;

            var heroesAccountsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Heroes of the Storm\Accounts");
            var heroesAccountsFolder = new DirectoryInfo(heroesAccountsFolderPath);

            var replayFiles = heroesAccountsFolder.GetFiles("*.StormReplay", SearchOption.AllDirectories);

            FileCount = replayFiles.Length;

            var replays = await GetReplaysFromDataFile();
            
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
                FilesProcessed++;
                watch.Stop();
                ElapsedTime += watch.ElapsedMilliseconds;
                ApproxTimeLeft = (ElapsedTime / FilesProcessed) * (FileCount - FilesProcessed);
            }
            replayRepository.SaveReplays(replays);
            var json = JsonConvert.SerializeObject(replays);
            File.WriteAllText(Environment.CurrentDirectory + "/data.txt", json);
            navigationService.NavigateTo("SetPlayerName");
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
}
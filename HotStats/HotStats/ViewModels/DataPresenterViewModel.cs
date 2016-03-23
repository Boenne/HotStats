using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;
using HotStats.Wrappers;
using Newtonsoft.Json;

namespace HotStats.ViewModels
{
    public class DataPresenterViewModel : ObservableObject, IDataPresenterViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private IEnumerable<IGrouping<string, MatchViewModel>> matches;
        private string playerName;
        private bool playerNameIsSet;
        private bool presentingData;

        public DataPresenterViewModel(IMessenger messenger, IDispatcherWrapper dispatcherWrapper,
            IReplayRepository replayRepository)
        {
            this.messenger = messenger;
            this.dispatcherWrapper = dispatcherWrapper;
            this.replayRepository = replayRepository;
            messenger.Register<DataHasBeenLoadedMessage>(this, message => { LoadDataAsync(); });
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

        public bool PresentingData
        {
            get { return presentingData; }
            set
            {
                presentingData = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<IGrouping<string, MatchViewModel>> Matches
        {
            get { return matches; }
            set
            {
                matches = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadedCommand => new DelegateCommand(StartUp);
        public ICommand SetPlayerNameCommand => new DelegateCommand(SetPlayerName);
        public ICommand HeroSelectedCommand => new RelayCommand<string>(SelectHero);

        public void StartUp()
        {
            PlayerName = Settings.Default.PlayerName;
        }

        public void SelectHero(string hero)
        {
            messenger.Send(new HeroSelectedMessage(hero, PlayerName));
        }

        public Task LoadDataAsync()
        {
            return Task.Factory.StartNew(LoadData);
        }

        public async void LoadData()
        {
            dispatcherWrapper.BeginInvoke(() => PresentingData = true);
            var replays = replayRepository.GetReplays();
            if (replays == null)
            {
                var path = Environment.CurrentDirectory + "/data.txt";
                if (!File.Exists(path) || !PlayerNameIsSet) return;
                replays = JsonConvert.DeserializeObject<List<Replay>>(File.ReadAllText(path));
                replayRepository.SaveReplays(replays);
            }
            var matchList = new List<MatchViewModel>();

            foreach (var replay in replays.Where(x => x != null))
            {
                var match = await CreateMatchViewModelAsync(replay);
                if (match != null) matchList.Add(match);
            }
            Matches = matchList.GroupBy(x => x.Hero);
            dispatcherWrapper.BeginInvoke(() => PresentingData = false);
        }

        public void SetPlayerName()
        {
            if (string.IsNullOrEmpty(PlayerName)) return;
            PlayerNameIsSet = true;
            Settings.Default.PlayerName = PlayerName;
            Settings.Default.Save();
            LoadDataAsync();
        }

        public Task<MatchViewModel> CreateMatchViewModelAsync(Replay replay)
        {
            return Task.Factory.StartNew(() =>
            {
                var player = replay.Players.FirstOrDefault(x => x.Name.ToLower() == PlayerName.ToLower());
                if (player == null) return null;
                return new MatchViewModel
                {
                    Hero = player.Character,
                    GameMode = replay.GameMode,
                    Map = replay.Map,
                    HeroDamage = player.ScoreResult.HeroDamage,
                    SiegeDamage = player.ScoreResult.SiegeDamage,
                    Healing = player.ScoreResult.Healing,
                    DamageTaken = player.ScoreResult.DamageTaken,
                    Winner = player.IsWinner,
                    TimeStamp = replay.Timestamp,
                    TakeDowns = player.ScoreResult.SoloKills,
                    Assists = player.ScoreResult.Assists,
                    Deaths = player.ScoreResult.Deaths,
                    GameLength = replay.ReplayLength,
                    ExpContribution = player.ScoreResult.ExperienceContribution
                };
            });
        }
    }
}
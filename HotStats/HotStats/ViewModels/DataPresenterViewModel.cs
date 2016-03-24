using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
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
        private bool dataPresented;
        private IEnumerable<IGrouping<string, MatchViewModel>> matches;
        private string playerName;
        private bool presentingData;

        public DataPresenterViewModel(IMessenger messenger, IDispatcherWrapper dispatcherWrapper,
            IReplayRepository replayRepository)
        {
            this.messenger = messenger;
            this.dispatcherWrapper = dispatcherWrapper;
            this.replayRepository = replayRepository;
            messenger.Register<SetPlayerNameMessage>(this, message =>
            {
                playerName = message.PlayerName;
                LoadDataAsync();
            });
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

        public bool DataPresented
        {
            get { return dataPresented; }
            set
            {
                dataPresented = value;
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

        public ICommand HeroSelectedCommand => new RelayCommand<string>(SelectHero);

        public void SelectHero(string hero)
        {
            messenger.Send(new HeroSelectedMessage(hero, playerName));
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
                if (!File.Exists(path) || string.IsNullOrEmpty(playerName)) return;
                replays = JsonConvert.DeserializeObject<List<Replay>>(File.ReadAllText(path));
                replayRepository.SaveReplays(replays);
            }
            var matchList = new List<MatchViewModel>();

            foreach (var replay in replays.Where(x => x != null))
            {
                var match = await CreateMatchViewModelAsync(replay);
                if (match != null) matchList.Add(match);
            }
            Matches = matchList.GroupBy(x => x.Hero).OrderByDescending(x => x.Count());
            dispatcherWrapper.BeginInvoke(() =>
            {
                PresentingData = false;
                DataPresented = true;
            });
        }

        public Task<MatchViewModel> CreateMatchViewModelAsync(Replay replay)
        {
            return Task.Factory.StartNew(() =>
            {
                var player = replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower());
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class MatchesViewModel : ObservableObject, IMatchesViewModel
    {
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private List<GameMode> gameModes = new List<GameMode> {GameMode.QuickMatch, GameMode.HeroLeague, GameMode.UnrankedDraft};
        private string hero;
        private bool heroSelected;
        private List<MatchViewModel> matches;
        private string playerName;
        private bool playerNameSet;
        private DateTime selectedDateFilter;

        public MatchesViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                HeroSelected = true;
                hero = message.Hero;
                LoadDataAsync();
            });
            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                HeroSelected = false;
                LoadDataAsync();
            });
            messenger.Register<PlayerNameHasBeenSetMessage>(this, message =>
            {
                PlayerNameSet = true;
                playerName = message.PlayerName;
                LoadDataAsync();
            });
            messenger.Register<GameModeChangedMessage>(this, message =>
            {
                gameModes = message.GameModes;
                LoadDataAsync();
            });
            messenger.Register<DateFilterSelectedMessage>(this, message =>
            {
                selectedDateFilter = message.Date;
                LoadDataAsync();
            });
        }

        public bool HeroSelected
        {
            get { return heroSelected; }
            set
            {
                heroSelected = value;
                OnPropertyChanged();
            }
        }

        public bool PlayerNameSet
        {
            get { return playerNameSet; }
            set
            {
                playerNameSet = value;
                OnPropertyChanged();
            }
        }

        public List<MatchViewModel> Matches
        {
            get { return matches; }
            set
            {
                matches = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectMatchCommand => new RelayCommand<MatchViewModel>(SelectMatch);

        public void SelectMatch(MatchViewModel matchViewModel)
        {
            if (matchViewModel == null) return;
            messenger.Send(new MatchSelectedMessage(matchViewModel.TimeStamp));
        }

        public Task LoadDataAsync()
        {
            return Task.Factory.StartNew(LoadData);
        }

        public async void LoadData()
        {
            var replays = replayRepository.GetReplays().Where(x => gameModes.Contains(x.GameMode));
            var matchList = new List<MatchViewModel>();

            foreach (var replay in replays.Where(x => x.Timestamp >= selectedDateFilter))
            {
                var match = await CreateMatchViewModelAsync(replay);
                if (match != null) matchList.Add(match);
            }
            Matches = matchList.OrderByDescending(x => x.TimeStamp).ToList();
        }

        public Task<MatchViewModel> CreateMatchViewModelAsync(Replay replay)
        {
            return Task.Factory.StartNew(() =>
            {
                var player = HeroSelected
                    ? replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower() && x.Character == hero)
                    : replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower());
                
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
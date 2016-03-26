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
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class MatchesViewModel : ObservableObject, IMatchesViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private string hero;
        private bool heroSelected;
        private List<MatchViewModel> matches;
        private string playerName;

        public MatchesViewModel(IMessenger messenger, IDispatcherWrapper dispatcherWrapper,
            IReplayRepository replayRepository)
        {
            this.messenger = messenger;
            this.dispatcherWrapper = dispatcherWrapper;
            this.replayRepository = replayRepository;
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                HeroSelected = true;
                hero = message.Hero;
                LoadDataAsync();
            });
            messenger.Register<HeroDeselectedMessage>(this, message => HeroSelected = false);
            messenger.Register<SetPlayerNameMessage>(this, message => { playerName = message.PlayerName; });
        }

        public ICommand HeroSelectedCommand => new RelayCommand<string>(SelectHero);

        public bool HeroSelected
        {
            get { return heroSelected; }
            set
            {
                heroSelected = value;
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

        public void SelectHero(string hero)
        {
            messenger.Send(new HeroSelectedMessage(hero));
        }

        public Task LoadDataAsync()
        {
            return Task.Factory.StartNew(LoadData);
        }

        public async void LoadData()
        {
            var replays = replayRepository.GetReplays();
            var matchList = new List<MatchViewModel>();

            foreach (var replay in replays.Where(x => x != null))
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
                var player =
                    replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower() && x.Character == hero);
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
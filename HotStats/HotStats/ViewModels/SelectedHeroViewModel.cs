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
    public class SelectedHeroViewModel : ObservableObject, ISelectedHeroViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private List<GameMode> gameModes = new List<GameMode> {GameMode.QuickMatch, GameMode.HeroLeague};
        private string hero;
        private string playerName;
        private ITotalStatsViewModel totalStatsViewModel;

        public SelectedHeroViewModel(IMessenger messenger, IReplayRepository replayRepository,
            IDispatcherWrapper dispatcherWrapper)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;
            this.dispatcherWrapper = dispatcherWrapper;
            messenger.Register<PlayerNameHasBeenSetMessage>(this, message => playerName = message.PlayerName);
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                Hero = message.Hero;
                CalculateStatsAsync();
            });
            messenger.Register<GameModeChangedMessage>(this, message =>
            {
                gameModes = message.GameModes;
                CalculateStatsAsync();
            });
        }

        public string Hero
        {
            get { return hero; }
            set
            {
                hero = value;
                OnPropertyChanged();
            }
        }

        public ITotalStatsViewModel TotalStatsViewModel
        {
            get { return totalStatsViewModel; }
            set
            {
                totalStatsViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeselectHeroCommand => new RelayCommand(DeselectHero);

        public void DeselectHero()
        {
            messenger.Send(new HeroDeselectedMessage());
        }

        public void CalculateStatsAsync()
        {
            Task.Factory.StartNew(CalculateStats);
        }

        public void CalculateStats()
        {
            var replays = replayRepository.GetReplays().Where(x => gameModes.Contains(x.GameMode));
            var tempTotalStats = new TotalStatsViewModel();
            foreach (var replay in replays)
            {
                var player =
                    replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower() && x.Character == hero);
                if (player == null) continue;
                tempTotalStats.Games++;
                switch (replay.GameMode)
                {
                    case GameMode.QuickMatch:
                        tempTotalStats.QuickMatches++;
                        break;
                    case GameMode.HeroLeague:
                        tempTotalStats.RankedGames++;
                        break;
                }
                if (!player.HasScoreResult()) continue;
                tempTotalStats.Takedowns += player.ScoreResult.SoloKills;
                tempTotalStats.Deaths += player.ScoreResult.Deaths;
                tempTotalStats.Assists += player.ScoreResult.Assists;
                tempTotalStats.HeroDamage += player.ScoreResult.HeroDamage;
                tempTotalStats.SiegeDamage += player.ScoreResult.SiegeDamage;
                tempTotalStats.Healing += player.ScoreResult.Healing ?? 0;
                tempTotalStats.DamageTaken += player.ScoreResult.DamageTaken ?? 0;
                tempTotalStats.ExpContribution += player.ScoreResult.ExperienceContribution;
            }
            dispatcherWrapper.BeginInvoke(() => TotalStatsViewModel = tempTotalStats);
        }
    }
}
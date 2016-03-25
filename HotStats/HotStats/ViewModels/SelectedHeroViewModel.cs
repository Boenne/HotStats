using System.Linq;
using System.Threading.Tasks;
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
        private readonly IReplayRepository replayRepository;
        private readonly IDispatcherWrapper dispatcherWrapper;
        private string hero;
        private bool heroSelected;
        private ITotalStatsViewModel totalStatsViewModel;

        public SelectedHeroViewModel(IMessenger messenger, IReplayRepository replayRepository, IDispatcherWrapper dispatcherWrapper)
        {
            this.replayRepository = replayRepository;
            this.dispatcherWrapper = dispatcherWrapper;
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                Hero = message.Hero;
                HeroSelected = true;
                CalculateStatsAsync(message.Hero, message.PlayerName);
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

        public void CalculateStatsAsync(string hero, string playerName)
        {
            Task.Factory.StartNew(() => CalculateStats(hero, playerName));
        }

        public void CalculateStats(string hero, string playerName)
        {
            var replays = replayRepository.GetReplays();
            var tempTotalStats = new TotalStatsViewModel();
            foreach (var replay in replays)
            {
                var player = replay.Players.FirstOrDefault(x => x.Name == playerName && x.Character == hero);
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
using System;
using System.Collections.Generic;
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
    public class AverageStatsViewModel : ObservableObject, IAverageStatsViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IReplayRepository replayRepository;
        private List<AverageViewModel> averageViewModels;
        private List<GameMode> gameModes = new List<GameMode> {GameMode.QuickMatch, GameMode.HeroLeague};
        private string hero;
        private string playerName;
        private bool playerNameIsSet;

        public AverageStatsViewModel(IMessenger messenger, IReplayRepository replayRepository,
            IDispatcherWrapper dispatcherWrapper)
        {
            this.replayRepository = replayRepository;
            this.dispatcherWrapper = dispatcherWrapper;
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                hero = message.Hero;
                CalculateAverageStatsAsync();
            });
            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                hero = string.Empty;
                CalculateAverageStatsAsync();
            });
            messenger.Register<PlayerNameHasBeenSetMessage>(this, message =>
            {
                PlayerNameIsSet = true;
                playerName = message.PlayerName;
                hero = string.Empty;
                CalculateAverageStatsAsync();
            });
            messenger.Register<GameModeChangedMessage>(this, message =>
            {
                gameModes = message.GameModes;
                CalculateAverageStatsAsync();
            });
        }

        public List<AverageViewModel> AverageViewModels
        {
            get { return averageViewModels; }
            set
            {
                averageViewModels = value;
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

        public void CalculateAverageStatsAsync()
        {
            Task.Factory.StartNew(CalculateAverageStats);
        }

        public void CalculateAverageStats()
        {
            var replays = replayRepository.GetReplays().Where(x => gameModes.Contains(x.GameMode));

            var wins = 0;
            var losses = 0;
            var winsWithScoreResults = 0;
            var lossesWithScoreResults = 0;
            var totalAverageViewModel = new AverageViewModel {Title = "All"};
            var lossesAverageViewModel = new AverageViewModel {Title = "Losses"};
            var winsAverageViewModel = new AverageViewModel {Title = "Wins"};
            var filteredReplays = !string.IsNullOrEmpty(hero)
                ? replays.Where(x => x.Players.Any(y => y.Character == hero && y.Name.ToLower() == playerName.ToLower()))
                : replays.Where(x => x.Players.Any(y => y.Name.ToLower() == playerName.ToLower()));
            foreach (var replay in filteredReplays)
            {
                var player = replay.Players.First(x => x.Name.ToLower() == playerName.ToLower());
                if (player.IsWinner)
                {
                    wins++;
                    if (player.HasScoreResult())
                        winsWithScoreResults++;
                    winsAverageViewModel = CreateAverageViewModel(replay, player, winsAverageViewModel, wins,
                        winsWithScoreResults);
                }
                else
                {
                    losses++;
                    if (player.HasScoreResult())
                        lossesWithScoreResults++;
                    lossesAverageViewModel = CreateAverageViewModel(replay, player, lossesAverageViewModel, losses,
                        lossesWithScoreResults);
                }
                totalAverageViewModel = CreateAverageViewModel(replay, player, totalAverageViewModel, wins + losses,
                    winsWithScoreResults + lossesWithScoreResults);
            }

            CalculateAverages(totalAverageViewModel);
            CalculateAverages(winsAverageViewModel);
            CalculateAverages(lossesAverageViewModel);

            dispatcherWrapper.BeginInvoke(() =>
            {
                AverageViewModels =
                    new List<AverageViewModel> {totalAverageViewModel, winsAverageViewModel, lossesAverageViewModel};
            });
        }

        public void CalculateAverages(AverageViewModel averageViewModel)
        {
            if (averageViewModel.GamesWithScoreResults == 0)
                averageViewModel.GamesWithScoreResults++;
            if (averageViewModel.Games == 0)
                averageViewModel.Games++;
            averageViewModel.Assists = averageViewModel.Assists / averageViewModel.GamesWithScoreResults;
            averageViewModel.TakeDowns = averageViewModel.TakeDowns / averageViewModel.GamesWithScoreResults;
            averageViewModel.Deaths = averageViewModel.Deaths / averageViewModel.GamesWithScoreResults;
            averageViewModel.ExpContribution = averageViewModel.ExpContribution / averageViewModel.GamesWithScoreResults;
            averageViewModel.HeroDamage = averageViewModel.HeroDamage / averageViewModel.GamesWithScoreResults;
            averageViewModel.SiegeDamage = averageViewModel.SiegeDamage / averageViewModel.GamesWithScoreResults;
            averageViewModel.Healing = averageViewModel.Healing / averageViewModel.GamesWithScoreResults;
            averageViewModel.DamageTaken = averageViewModel.DamageTaken / averageViewModel.GamesWithScoreResults;
            averageViewModel.GameLength = averageViewModel.GameLength / averageViewModel.Games;
        }

        public AverageViewModel CreateAverageViewModel(Replay replay, Player player, AverageViewModel averageViewModel,
            int count, int replaysWithScoreResults)
        {
            var takeDowns = player.HasScoreResult()
                ? averageViewModel.TakeDowns + player.ScoreResult.SoloKills
                : averageViewModel.TakeDowns;
            var assits = player.HasScoreResult()
                ? averageViewModel.Assists + player.ScoreResult.Assists
                : averageViewModel.Assists;
            var deaths = player.HasScoreResult()
                ? averageViewModel.Deaths + player.ScoreResult.Deaths
                : averageViewModel.Deaths;
            var heroDamge = player.HasScoreResult()
                ? averageViewModel.HeroDamage + player.ScoreResult.HeroDamage
                : averageViewModel.HeroDamage;
            var siegeDamage = player.HasScoreResult()
                ? averageViewModel.SiegeDamage + player.ScoreResult.SiegeDamage
                : averageViewModel.SiegeDamage;
            var expContribution = player.HasScoreResult()
                ? averageViewModel.ExpContribution + player.ScoreResult.ExperienceContribution
                : averageViewModel.ExpContribution;
            var damageTaken = player.HasScoreResult() && player.ScoreResult.DamageTaken.HasValue
                ? averageViewModel.DamageTaken + player.ScoreResult.DamageTaken.Value
                : averageViewModel.DamageTaken;
            var healing = player.HasScoreResult() && player.ScoreResult.Healing.HasValue
                ? averageViewModel.Healing + player.ScoreResult.Healing.Value
                : averageViewModel.Healing;
            return new AverageViewModel
            {
                Title = averageViewModel.Title,
                Assists = assits,
                Deaths = deaths,
                ExpContribution = expContribution,
                GameLength = averageViewModel.GameLength + Convert.ToInt64(replay.ReplayLength.TotalMilliseconds),
                Games = count,
                HeroDamage = heroDamge,
                SiegeDamage = siegeDamage,
                TakeDowns = takeDowns,
                DamageTaken = damageTaken,
                Healing = healing,
                GamesWithScoreResults =
                    player.HasScoreResult() ? replaysWithScoreResults : averageViewModel.GamesWithScoreResults
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heroes.ReplayParser;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Services.Interfaces;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class AverageStatsViewModel : ViewModelBase, IAverageStatsViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IReplayRepository replayRepository;
        private List<AverageViewModel> averageViewModels;
        private double winPercentage;

        public AverageStatsViewModel(IMessenger messenger, IReplayRepository replayRepository,
            IDispatcherWrapper dispatcherWrapper) : base(messenger)
        {
            this.replayRepository = replayRepository;
            this.dispatcherWrapper = dispatcherWrapper;
            messenger.Register<DataFilterHasBeenAppliedMessage>(this, message => CalculateAverageStats());
        }

        public List<AverageViewModel> AverageViewModels
        {
            get { return averageViewModels; }
            set { Set(() => AverageViewModels, ref averageViewModels, value); }
        }

        public double WinPercentage
        {
            get { return winPercentage; }
            set { Set(() => WinPercentage, ref winPercentage, value); }
        }

        public async Task CalculateAverageStats()
        {
            var replays = replayRepository.GetFilteredReplays();
            var wins = 0;
            var losses = 0;
            var winsWithScoreResults = 0;
            var lossesWithScoreResults = 0;
            var totalAverageViewModel = new AverageViewModel {Title = "All"};
            var lossesAverageViewModel = new AverageViewModel {Title = "Losses"};
            var winsAverageViewModel = new AverageViewModel {Title = "Wins"};
            foreach (var replay in replays)
            {
                var player = replay.Players.First(x => PlayerName.Matches(x.Name.ToLower()));
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
            await dispatcherWrapper.BeginInvoke(() =>
            {
                AverageViewModels =
                    new List<AverageViewModel> {totalAverageViewModel, winsAverageViewModel, lossesAverageViewModel};
                WinPercentage = (double) wins/(wins + losses)*100;
            });
        }

        public void CalculateAverages(AverageViewModel averageViewModel)
        {
            if (averageViewModel.Games == 0 || averageViewModel.GamesWithScoreResults == 0)
                return;
            averageViewModel.Assists = averageViewModel.Assists/averageViewModel.GamesWithScoreResults;
            averageViewModel.TakeDowns = averageViewModel.TakeDowns/averageViewModel.GamesWithScoreResults;
            averageViewModel.Deaths = averageViewModel.Deaths/averageViewModel.GamesWithScoreResults;
            averageViewModel.ExpContribution = averageViewModel.ExpContribution/averageViewModel.GamesWithScoreResults;
            averageViewModel.HeroDamage = averageViewModel.HeroDamage/averageViewModel.GamesWithScoreResults;
            averageViewModel.SiegeDamage = averageViewModel.SiegeDamage/averageViewModel.GamesWithScoreResults;
            averageViewModel.Healing = averageViewModel.Healing/averageViewModel.GamesWithScoreResults;
            averageViewModel.DamageTaken = averageViewModel.DamageTaken/averageViewModel.GamesWithScoreResults;
            averageViewModel.GameLength = averageViewModel.GameLength/averageViewModel.Games;
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

    public interface IAverageStatsViewModel
    {
        List<AverageViewModel> AverageViewModels { get; set; }
        double WinPercentage { get; set; }
    }
}
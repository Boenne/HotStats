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
    public class MatchDetailsViewModel : ViewModelBase, IMatchDetailsViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IReplayRepository replayRepository;
        private List<PlayerViewModel> players;
        private TeamViewModel team1;
        private TeamViewModel team2;

        public MatchDetailsViewModel(IMessenger messenger,
            IReplayRepository replayRepository,
            IDispatcherWrapper dispatcherWrapper) : base(messenger)
        {
            this.replayRepository = replayRepository;
            this.dispatcherWrapper = dispatcherWrapper;
            messenger.Register<MatchSelectedMessage>(this, message => GetDetails(message.Timestamp));
        }

        public List<PlayerViewModel> Players
        {
            get { return players; }
            set { Set(() => Players, ref players, value); }
        }

        public TeamViewModel Team1
        {
            get { return team1; }
            set { Set(() => Team1, ref team1, value); }
        }

        public TeamViewModel Team2
        {
            get { return team2; }
            set { Set(() => Team2, ref team2, value); }
        }

        public async Task GetDetails(DateTime timestamp)
        {
            var replay = replayRepository.GetFilteredReplays().FirstOrDefault(x => x.Timestamp == timestamp);
            if (replay == null) return;
            var playerViewModels = replay.Players.Select(x => new PlayerViewModel
            {
                Assists = x.ScoreResult.Assists,
                DamageTaken = x.ScoreResult.DamageTaken,
                Deaths = x.ScoreResult.Deaths,
                ExpContribution = x.ScoreResult.ExperienceContribution,
                Healing = x.ScoreResult.Healing,
                Hero = x.Character,
                HeroDamage = x.ScoreResult.HeroDamage,
                PlayerName = x.Name,
                SiegeDamage = x.ScoreResult.SiegeDamage,
                TakeDowns = x.ScoreResult.SoloKills,
                Winner = x.IsWinner
            }).ToList();
            await dispatcherWrapper.BeginInvoke(() => Players = playerViewModels);

            var team1Temp = new TeamViewModel
            {
                Level = replay.TeamLevels[0].Count,
                Takedowns = GetTakedowns(replay, 0),
                Winner = replay.Players.First(x => x.Team == 0).IsWinner
            };
            var team2Temp = new TeamViewModel
            {
                Level = replay.TeamLevels[1].Count,
                Takedowns = GetTakedowns(replay, 1),
                Winner = replay.Players.First(x => x.Team == 1).IsWinner
            };

            await dispatcherWrapper.BeginInvoke(() =>
            {
                Team1 = team1Temp;
                Team2 = team2Temp;
            });
        }

        public int GetTakedowns(Replay replay, int team)
        {
            var takedowns =
                replay.Players.Where(x => x.Team == team).Sum(x => x.HasScoreResult() ? x.ScoreResult.SoloKills : 0);
            return takedowns;
        }
    }

    public interface IMatchDetailsViewModel
    {
        List<PlayerViewModel> Players { get; set; }
        TeamViewModel Team1 { get; set; }
        TeamViewModel Team2 { get; set; }
    }
}
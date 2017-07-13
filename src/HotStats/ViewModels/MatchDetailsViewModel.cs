using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heroes.ReplayParser;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
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
        private string playerName = Settings.Default.PlayerName;

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
                DamageTaken = new Stat
                {
                    Value = x.ScoreResult.DamageTaken
                },
                Deaths = x.ScoreResult.Deaths,
                ExpContribution = new Stat
                {
                    Value = x.ScoreResult.ExperienceContribution
                },
                Healing = new Stat
                {
                    Value = x.ScoreResult.Healing
                },
                Hero = x.Character,
                HeroDamage = new Stat
                {
                    Value = x.ScoreResult.HeroDamage
                },
                Player = new PlayerVM
                {
                    Name = x.Name,
                    IsMe = x.Name.ToLower() == playerName
                },
                SiegeDamage = new Stat
                {
                    Value = x.ScoreResult.SiegeDamage
                },
                TakeDowns = x.ScoreResult.SoloKills,
                Winner = x.IsWinner,
                Team = x.Team
            }).ToList();

            SetHighest(playerViewModels, 0);
            SetHighest(playerViewModels, 1);

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

        public void SetHighest(List<PlayerViewModel> playerViewModels, int team)
        {
            SetHighestForTeam(playerViewModels, team, x => x.HeroDamage.Value ?? 0, "HeroDamage");
            SetHighestForTeam(playerViewModels, team, x => x.SiegeDamage.Value ?? 0, "SiegeDamage");
            SetHighestForTeam(playerViewModels, team, x => x.Healing.Value ?? 0, "Healing");
            SetHighestForTeam(playerViewModels, team, x => x.DamageTaken.Value ?? 0, "DamageTaken");
            SetHighestForTeam(playerViewModels, team, x => x.ExpContribution.Value ?? 0, "ExpContribution");
        }

        public void SetHighestForTeam(List<PlayerViewModel> playerViewModels, int team,
            Func<PlayerViewModel, int> orderFunc, string propertyName)
        {
            var playerViewModel = playerViewModels.Where(x => x.Team == team).OrderByDescending(orderFunc).First();
            ((Stat) playerViewModel.GetType().GetProperty(propertyName).GetValue(playerViewModel)).IsHighest = true;
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
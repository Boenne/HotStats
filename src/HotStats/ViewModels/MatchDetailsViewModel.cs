using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }
    }

    public interface IMatchDetailsViewModel
    {
        List<PlayerViewModel> Players { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Heroes.ReplayParser;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
using HotStats.Services.Interfaces;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class MatchesViewModel : ViewModelBase, IMatchesViewModel
    {
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private readonly IDispatcherWrapper dispatcherWrapper;
        private List<MatchViewModel> matches;
        private readonly string playerName = Settings.Default.PlayerName;

        public MatchesViewModel(IMessenger messenger, 
            IReplayRepository replayRepository,
            IDispatcherWrapper dispatcherWrapper) : base(messenger)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;
            this.dispatcherWrapper = dispatcherWrapper;
            messenger.Register<DataFilterHasBeenAppliedMessage>(this, message => LoadData());
        }

        public List<MatchViewModel> Matches
        {
            get { return matches; }
            set { Set(() => Matches, ref matches, value); }
        }

        public RelayCommand<MatchViewModel> SelectMatchCommand => new RelayCommand<MatchViewModel>(SelectMatch);

        public void SelectMatch(MatchViewModel matchViewModel)
        {
            if (matchViewModel == null) return;
            messenger.Send(new MatchSelectedMessage(matchViewModel.TimeStamp));
        }

        public async Task LoadData()
        {
            var replays = replayRepository.GetFilteredReplays();
            var matchList = new List<MatchViewModel>();

            foreach (var replay in replays)
            {
                var match = await CreateMatchViewModelAsync(replay);
                if (match != null) matchList.Add(match);
            }
            await dispatcherWrapper.BeginInvoke(() => Matches = matchList.OrderByDescending(x => x.TimeStamp).ToList());
        }

        public Task<MatchViewModel> CreateMatchViewModelAsync(Replay replay)
        {
            return Task.Factory.StartNew(() =>
            {
                var player = replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName);

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

    public interface IMatchesViewModel
    {
        List<MatchViewModel> Matches { get; set; }
        RelayCommand<MatchViewModel> SelectMatchCommand { get; }
    }
}
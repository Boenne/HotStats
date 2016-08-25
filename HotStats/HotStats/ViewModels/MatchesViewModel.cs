using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class MatchesViewModel : ViewModelBase, IMatchesViewModel
    {
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private List<MatchViewModel> matches;
        private string playerName;

        public MatchesViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;
            messenger.Register<PlayerNameHasBeenSetMessage>(this, message =>
            {
                playerName = message.PlayerName;
                LoadDataAsync();
            });
            messenger.Register<DataFilterHasBeenAppliedMessage>(this, message => { LoadDataAsync(); });
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

        public Task LoadDataAsync()
        {
            return Task.Factory.StartNew(LoadData);
        }

        public async void LoadData()
        {
            var replays = replayRepository.GetFilteredReplays();
            var matchList = new List<MatchViewModel>();

            foreach (var replay in replays)
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
                var player = replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower());

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
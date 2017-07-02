using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
using HotStats.Services.Interfaces;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class OpponentsAndTeammatesViewModel : ViewModelBase, IOpponentsAndTeammatesViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly string playerName = Settings.Default.PlayerName;
        private readonly IReplayRepository replayRepository;
        private List<OpponentOrTeammateViewModel> opponents;
        private List<OpponentOrTeammateViewModel> teammates;

        public OpponentsAndTeammatesViewModel(IMessenger messenger,
            IReplayRepository replayRepository,
            IDispatcherWrapper dispatcherWrapper)
            : base(messenger)
        {
            this.replayRepository = replayRepository;
            this.dispatcherWrapper = dispatcherWrapper;
            messenger.Register<DataFilterHasBeenAppliedMessage>(this, message => LoadData());
        }

        public List<OpponentOrTeammateViewModel> Opponents
        {
            get { return opponents; }
            set { Set(() => Opponents, ref opponents, value); }
        }

        public List<OpponentOrTeammateViewModel> Teammates
        {
            get { return teammates; }
            set { Set(() => Teammates, ref teammates, value); }
        }

        public async Task LoadData()
        {
            var replays = replayRepository.GetFilteredReplays();
            var wins = new Dictionary<string, int>();
            var losses = new Dictionary<string, int>();
            var opponentWins = new Dictionary<string, int>();
            var opponentLosses = new Dictionary<string, int>();
            foreach (var replay in replays)
            {
                var me = replay.Players.First(x => x.Name.ToLower() == playerName);

                foreach (var player in replay.Players.Where(x => x.Team != me.Team))
                {
                    Increment(me.IsWinner ? opponentLosses : opponentWins, player.Character);
                }
                foreach (var player in replay.Players.Where(x => x.Team == me.Team && x.Name.ToLower() != playerName))
                {
                    Increment(me.IsWinner ? wins : losses, player.Character);
                }
            }

            var teammateMatches = JoinDictionaries(losses, wins);
            var opponentMatches = JoinDictionaries(opponentLosses, opponentWins);

            var teammateViewModels =
                teammateMatches.Select(x => CreateViewModel(x.Key, wins, x.Value)).OrderByDescending(x => x.Games).ToList();
            var opponentViewModels =
                opponentMatches.Select(x => CreateViewModel(x.Key, opponentWins, x.Value)).OrderByDescending(x => x.Games).ToList();

            await dispatcherWrapper.BeginInvoke(() =>
            {
                Opponents = opponentViewModels;
                Teammates = teammateViewModels;
            });
        }

        public Dictionary<string, int> JoinDictionaries(Dictionary<string, int> dict1, Dictionary<string, int> dict2)
        {
            var result = new Dictionary<string, int>();
            foreach (var key in dict1.Keys)
            {
                var value = dict1[key];
                if (dict2.ContainsKey(key))
                    value += dict2[key];
                result.Add(key, value);
            }
            return result;
        }

        public OpponentOrTeammateViewModel CreateViewModel(string hero, Dictionary<string, int> wins, int games)
        {
            var opponentOrTeammateViewModel = new OpponentOrTeammateViewModel
            {
                Hero = hero,
                WonPercentage = CalculatePercentage(wins, hero, games),
                Games = games
            };

            opponentOrTeammateViewModel.LostPercentage = 100.0 - opponentOrTeammateViewModel.WonPercentage;
            return opponentOrTeammateViewModel;
        }

        public double CalculatePercentage(Dictionary<string, int> dict, string key, int games)
        {
            if (!dict.ContainsKey(key)) return 0.0;
            return (double) dict[key]/games*100;
        }

        public void Increment(Dictionary<string, int> dict, string key)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, 1);
            else
                dict[key]++;
        }
    }

    public interface IOpponentsAndTeammatesViewModel
    {
        List<OpponentOrTeammateViewModel> Opponents { get; set; }
        List<OpponentOrTeammateViewModel> Teammates { get; set; }
    }
}
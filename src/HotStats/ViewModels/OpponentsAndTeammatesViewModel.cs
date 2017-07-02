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
        private readonly string playerName = Settings.Default.PlayerName;
        private readonly IReplayRepository replayRepository;
        private readonly IDispatcherWrapper dispatcherWrapper;
        private List<OpponentViewModel> opponents;
        private List<OpponentViewModel> teammates;

        public OpponentsAndTeammatesViewModel(IMessenger messenger, 
            IReplayRepository replayRepository,
            IDispatcherWrapper dispatcherWrapper)
            : base(messenger)
        {
            this.replayRepository = replayRepository;
            this.dispatcherWrapper = dispatcherWrapper;
            messenger.Register<DataFilterHasBeenAppliedMessage>(this, message => LoadData());
        }

        public List<OpponentViewModel> Opponents
        {
            get { return opponents; }
            set { Set(() => Opponents, ref opponents, value); }
        }

        public List<OpponentViewModel> Teammates
        {
            get { return teammates; }
            set { Set(() => Teammates, ref teammates, value); }
        }

        public void LoadData()
        {
            FindOpponents(false);
            FindOpponents(true);
        }

        public async Task FindOpponents(bool findOpponents)
        {
            var replays = replayRepository.GetFilteredReplays();
            var wins = new Dictionary<string, int>();
            var losses = new Dictionary<string, int>();
            foreach (var replay in replays)
            {
                var me = replay.Players.First(x => x.Name.ToLower() == playerName);

                foreach (
                    var opponent in
                        replay.Players.Where(
                            x =>
                                findOpponents
                                    ? x.Team != me.Team
                                    : x.Team == me.Team && x.Name.ToLower() != playerName))
                {
                    Increment(me.IsWinner ? wins : losses, opponent.Character);
                }
            }
            var players = losses.Union(wins)
                .ToLookup(pair => pair.Key, pair => pair.Value)
                .ToDictionary(x => x.Key, x => x.First()).Keys;
            var viewModels = players.Select(opponent => new OpponentViewModel
            {
                Hero = opponent,
                LostPercentage = CalculatePercentage(losses, opponent, GetValueFromDictionary(wins, opponent)),
                WonPercentage = CalculatePercentage(wins, opponent, GetValueFromDictionary(losses, opponent)),
                Games = GetValueFromDictionary(losses, opponent) + GetValueFromDictionary(wins, opponent)
            }).OrderByDescending(x => x.Games).ToList();
            if (findOpponents)
                await dispatcherWrapper.BeginInvoke(() => Opponents = viewModels);
            else
                await dispatcherWrapper.BeginInvoke(() => Teammates = viewModels);
        }

        public double CalculatePercentage(Dictionary<string, int> dict, string key, int games)
        {
            if (!dict.ContainsKey(key)) return 0.0;
            return (double) dict[key]/(dict[key] + games)*100;
        }

        public void Increment(Dictionary<string, int> dict, string key)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, 1);
            else
                dict[key]++;
        }

        public int GetValueFromDictionary(Dictionary<string, int> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key] : 0;
        }
    }

    public interface IOpponentsAndTeammatesViewModel
    {
        List<OpponentViewModel> Opponents { get; set; }
        List<OpponentViewModel> Teammates { get; set; }
    }
}
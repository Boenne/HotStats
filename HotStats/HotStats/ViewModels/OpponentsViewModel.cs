using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class OpponentsViewModel : ObservableObject, IOpponentsViewModel
    {
        private readonly IReplayRepository replayRepository;
        private bool playerNameIsSet;
        private List<OpponentViewModel> opponents;
        private string playerName;
        private List<OpponentViewModel> teammates;

        public OpponentsViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.replayRepository = replayRepository;
            messenger.Register<HeroSelectedMessage>(this, message => FindOpponentsAsync(message.Hero));
            messenger.Register<HeroDeselectedMessage>(this, message => FindOpponentsAsync(string.Empty));
            messenger.Register<SetPlayerNameMessage>(this, message =>
            {
                PlayerNameIsSet = true;
                playerName = message.PlayerName;
            });
            messenger.Register<DataHasBeenLoadedMessage>(this, message => FindOpponentsAsync(string.Empty));
        }

        public List<OpponentViewModel> Opponents
        {
            get { return opponents; }
            set
            {
                opponents = value;
                OnPropertyChanged();
            }
        }

        public List<OpponentViewModel> Teammates
        {
            get { return teammates; }
            set
            {
                teammates = value;
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

        public void FindOpponentsAsync(string hero)
        {
            Task.Factory.StartNew(() => FindOpponents(hero, true));
            Task.Factory.StartNew(() => FindOpponents(hero, false));
        }

        public void FindOpponents(string hero, bool findOpponents)
        {
            var replays = replayRepository.GetReplays();
            var wins = new Dictionary<string, int>();
            var losses = new Dictionary<string, int>();
            var filteredReplays = !string.IsNullOrEmpty(hero)
                ? replays.Where(x => x.Players.Any(y => y.Character == hero && y.Name == playerName))
                : replays.Where(x => x.Players.Any(y => y.Name == playerName));
            foreach (var replay in filteredReplays)
            {
                var me = replay.Players.First(x => x.Name == playerName);
                
                foreach (var opponent in replay.Players.Where(x => findOpponents ? x.Team != me.Team : x.Team == me.Team && x.Name != playerName))
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
                Opponents = viewModels;
            else
                Teammates = viewModels;

        }

        public double CalculatePercentage(Dictionary<string, int> dict, string key, int games)
        {
            if (!dict.ContainsKey(key)) return 0.0;
            return (double) dict[key] / (dict[key] + games) * 100;
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
}
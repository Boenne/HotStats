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
        private bool heroSelected;
        private List<OpponentViewModel> opponents;

        public OpponentsViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.replayRepository = replayRepository;
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                HeroSelected = true;
                FindOpponentsAsync(message.Hero, message.PlayerName);
            });
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

        public bool HeroSelected
        {
            get { return heroSelected; }
            set
            {
                heroSelected = value;
                OnPropertyChanged();
            }
        }

        public void FindOpponentsAsync(string hero, string playerName)
        {
            Task.Factory.StartNew(() => FindOpponents(hero, playerName));
        }

        public void FindOpponents(string hero, string playerName)
        {
            var replays = replayRepository.GetReplays();
            var wins = new Dictionary<string, int>();
            var losses = new Dictionary<string, int>();
            foreach (var replay in replays.Where(x => x.Players.Any(y => y.Character == hero && y.Name == playerName)))
            {
                var me = replay.Players.First(x => x.Name == playerName);
                foreach (var opponent in replay.Players.Where(x => x.Team != me.Team))
                {
                    Increment(me.IsWinner ? wins : losses, opponent.Character);
                }
            }
            var allOpponents = losses.Union(wins)
                .ToLookup(pair => pair.Key, pair => pair.Value)
                .ToDictionary(x => x.Key, x => x.First()).Keys;
            Opponents = allOpponents.Select(opponent => new OpponentViewModel
            {
                Hero = opponent,
                LostPercentage = CalculatePercentage(losses, opponent, GetValueFromDictionary(wins, opponent)),
                WonPercentage = CalculatePercentage(wins, opponent, GetValueFromDictionary(losses, opponent)),
                Games = GetValueFromDictionary(losses, opponent) + GetValueFromDictionary(wins, opponent)
            }).OrderByDescending(x => x.Games).ToList();
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
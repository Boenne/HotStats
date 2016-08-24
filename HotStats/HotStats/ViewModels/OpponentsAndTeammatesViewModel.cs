using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class OpponentsAndTeammatesViewModel : ObservableObject, IOpponentsAndTeammatesViewModel
    {
        private readonly IReplayRepository replayRepository;
        private List<GameMode> gameModes = new List<GameMode> {GameMode.QuickMatch, GameMode.HeroLeague, GameMode.UnrankedDraft};
        private string hero;
        private List<OpponentViewModel> opponents;
        private string playerName;
        private DateTime selectedDateFilter;
        private List<OpponentViewModel> teammates;

        public OpponentsAndTeammatesViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.replayRepository = replayRepository;
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                hero = message.Hero;
                FindOpponentsAsync();
            });
            messenger.Register<DateFilterSelectedMessage>(this, message =>
            {
                selectedDateFilter = message.Date;
                FindOpponentsAsync();
            });
            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                hero = string.Empty;
                FindOpponentsAsync();
            });
            messenger.Register<PlayerNameHasBeenSetMessage>(this, message =>
            {
                playerName = message.PlayerName;
                FindOpponentsAsync();
            });
            messenger.Register<GameModeChangedMessage>(this, message =>
            {
                gameModes = message.GameModes;
                FindOpponentsAsync();
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

        public List<OpponentViewModel> Teammates
        {
            get { return teammates; }
            set
            {
                teammates = value;
                OnPropertyChanged();
            }
        }

        public void FindOpponentsAsync()
        {
            Task.Factory.StartNew(() => FindOpponents(true));
            Task.Factory.StartNew(() => FindOpponents(false));
        }

        public void FindOpponents(bool findOpponents)
        {
            var replays = replayRepository.GetReplays().Where(x => gameModes.Contains(x.GameMode) && x.Timestamp >= selectedDateFilter);
            var wins = new Dictionary<string, int>();
            var losses = new Dictionary<string, int>();
            var filteredReplays = !string.IsNullOrEmpty(hero)
                ? replays.Where(x => x.Players.Any(y => y.Character == hero && y.Name.ToLower() == playerName.ToLower()))
                : replays.Where(x => x.Players.Any(y => y.Name.ToLower() == playerName.ToLower()));
            foreach (var replay in filteredReplays)
            {
                var me = replay.Players.First(x => x.Name.ToLower() == playerName.ToLower());

                foreach (
                    var opponent in
                        replay.Players.Where(
                            x =>
                                findOpponents
                                    ? x.Team != me.Team
                                    : x.Team == me.Team && x.Name.ToLower() != playerName.ToLower()))
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
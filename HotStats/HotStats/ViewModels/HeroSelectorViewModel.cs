﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class HeroSelectorViewModel : ObservableObject, IHeroSelectorViewModel
    {
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private List<GameMode> gameModes = new List<GameMode> {GameMode.QuickMatch, GameMode.HeroLeague, GameMode.UnrankedDraft};
        private List<string> heroes;
        private string playerName;
        private bool playerNameIsSet;
        private bool showHeroLeague = true;
        private bool showQuickMatches = true;
        private bool showUnranked = true;

        public HeroSelectorViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;
            messenger.Register<PlayerNameHasBeenSetMessage>(this, message =>
            {
                playerName = message.PlayerName;
                GetHeroesAsync();
            });
        }

        public List<string> Heroes
        {
            get { return heroes; }
            set
            {
                heroes = value;
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

        public bool ShowHeroLeague
        {
            get { return showHeroLeague; }
            set
            {
                showHeroLeague = value;
                OnPropertyChanged();
                ChangeGameMode();
            }
        }

        public bool ShowQuickMatches
        {
            get { return showQuickMatches; }
            set
            {
                showQuickMatches = value;
                OnPropertyChanged();
                ChangeGameMode();
            }
        }

        public bool ShowUnranked
        {
            get { return showUnranked; }
            set
            {
                showUnranked = value;
                OnPropertyChanged();
                ChangeGameMode();
            }
        }

        public ICommand SelectHeroCommand => new RelayCommand<string>(SelectHero);

        public void SelectHero(string hero)
        {
            messenger.Send(new HeroSelectedMessage(hero));
        }

        public void ChangeGameMode()
        {
            gameModes = new List<GameMode> {GameMode.QuickMatch, GameMode.HeroLeague, GameMode.UnrankedDraft};
            if (!ShowHeroLeague)
                gameModes.Remove(GameMode.HeroLeague);
            if (!ShowQuickMatches)
                gameModes.Remove(GameMode.QuickMatch);
            if (!ShowUnranked)
                gameModes.Remove(GameMode.UnrankedDraft);
            messenger.Send(new GameModeChangedMessage(gameModes));
            GetHeroesAsync();
        }

        public void GetHeroesAsync()
        {
            Task.Factory.StartNew(GetHeroes);
        }

        public void GetHeroes()
        {
            PlayerNameIsSet = true;
            var replays = replayRepository.GetReplays();
            var result = new Dictionary<string, int>();
            foreach (var replay in replays.Where(x => gameModes.Contains(x.GameMode)))
            {
                var player = replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower());
                if (player == null) continue;
                if (result.ContainsKey(player.Character))
                    result[player.Character]++;
                else
                    result.Add(player.Character, 1);
            }
            Heroes = result.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }
    }
}
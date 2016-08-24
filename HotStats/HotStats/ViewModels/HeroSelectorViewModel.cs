using System;
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
    public class HeroSelectorViewModel : ViewModelBase, IHeroSelectorViewModel
    {
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private DateTime dateFilter;
        private DateTime earliestDate;

        private List<GameMode> gameModes = new List<GameMode>
        {
            GameMode.QuickMatch,
            GameMode.HeroLeague,
            GameMode.UnrankedDraft
        };

        private List<string> heroes;
        private string playerName;
        private bool showHeroLeague = true;
        private bool showQuickMatches = true;
        private bool showUnranked = true;
        private DateTime todaysDate;

        public HeroSelectorViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;

            messenger.Register<PlayerNameHasBeenSetMessage>(this, message =>
            {
                playerName = message.PlayerName;
                SetupDatePicker();
                GetHeroesAsync();
            });
            messenger.Register<DataHasBeenRefreshedMessage>(this, message => { GetHeroesAsync(); });
        }

        public RelayCommand<string> SelectHeroCommand => new RelayCommand<string>(SelectHero);
        public RelayCommand RemoveDateFilterCommand => new RelayCommand(RemoveDateFilte);
        public RelayCommand ReloadDataCommand => new RelayCommand(ReloadData);

        public List<string> Heroes
        {
            get { return heroes; }
            set { Set(() => Heroes, ref heroes, value); }
        }

        public bool ShowHeroLeague
        {
            get { return showHeroLeague; }
            set
            {
                Set(() => ShowHeroLeague, ref showHeroLeague, value);
                ChangeGameMode();
            }
        }

        public bool ShowQuickMatches
        {
            get { return showQuickMatches; }
            set
            {
                Set(() => ShowQuickMatches, ref showQuickMatches, value);
                ChangeGameMode();
            }
        }

        public bool ShowUnranked
        {
            get { return showUnranked; }
            set
            {
                Set(() => ShowUnranked, ref showUnranked, value);
                ChangeGameMode();
            }
        }

        public DateTime DateFilter
        {
            get { return dateFilter; }
            set
            {
                Set(() => DateFilter, ref dateFilter, value);
                GetHeroesAsync();
                messenger.Send(new DateFilterSelectedMessage {Date = DateFilter});
            }
        }

        public DateTime EarliestDate
        {
            get { return earliestDate; }
            set { Set(() => EarliestDate, ref earliestDate, value); }
        }

        public DateTime TodaysDate
        {
            get { return todaysDate; }
            set { Set(() => TodaysDate, ref todaysDate, value); }
        }

        public void SetupDatePicker()
        {
            var date = DateTime.Now;
            TodaysDate = date;
            foreach (var replay in replayRepository.GetReplays())
            {
                if (replay.Timestamp < date)
                    date = replay.Timestamp;
            }
            EarliestDate = date;
            DateFilter = EarliestDate;
        }

        public void ReloadData()
        {
            messenger.Send(new RefreshDataMessage());
        }

        public void RemoveDateFilte()
        {
            DateFilter = EarliestDate;
            messenger.Send(new DateFilterSelectedMessage {Date = EarliestDate});
        }

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
            var replays = replayRepository.GetReplays();
            var result = new Dictionary<string, int>();
            foreach (var replay in replays.Where(x => gameModes.Contains(x.GameMode) && x.Timestamp >= DateFilter))
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
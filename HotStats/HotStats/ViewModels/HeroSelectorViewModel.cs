using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ReplayParser;
using HotStats.Services;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class HeroSelectorViewModel : ViewModelBase, IHeroSelectorViewModel
    {
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private readonly IDataLoader dataLoader;
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
        private string selectedHero;
        private bool showHeroLeague = true;
        private bool showQuickMatches = true;
        private bool showUnranked = true;
        private DateTime todaysDate;

        public HeroSelectorViewModel(IMessenger messenger, IReplayRepository replayRepository, IDataLoader dataLoader)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;
            this.dataLoader = dataLoader;

            messenger.Register<PlayerNameHasBeenSetMessage>(this, message =>
            {
                playerName = message.PlayerName;
                SetupDatePicker();
                GetHeroesAsync();
            });
            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                selectedHero = null;
                FilterReplays();
            });
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
                FilterReplays();
                GetHeroesAsync();
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

        public async void ReloadData()
        {
            await dataLoader.LoadDataAsync();
            FilterReplays();
        }

        public void RemoveDateFilte()
        {
            FilterReplays();
        }

        public void SelectHero(string hero)
        {
            selectedHero = hero;
            FilterReplays();
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
            FilterReplays();
            GetHeroesAsync();
        }

        public void FilterReplays()
        {
            var replays = replayRepository.GetReplays().Where(x => gameModes.Contains(x.GameMode) && x.Timestamp >= DateFilter);
            replays = selectedHero != null
                ? replays.Where(x => x.Players.Any(y => y.Character == selectedHero && y.Name.ToLower() == playerName.ToLower()))
                : replays;
            replayRepository.SaveFilteredReplays(replays);
            messenger.Send(new DataFilterHasBeenAppliedMessage());
        }

        public void GetHeroesAsync()
        {
            Task.Factory.StartNew(GetHeroes);
        }

        public void GetHeroes()
        {
            var replays = replayRepository.GetReplays().Where(x => gameModes.Contains(x.GameMode) && x.Timestamp >= DateFilter);
            var result = new Dictionary<string, int>();
            foreach (var replay in replays)
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
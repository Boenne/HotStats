using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Heroes.ReplayParser;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
using HotStats.Services;
using HotStats.Services.Interfaces;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class HeroSelectorViewModel : ViewModelBase, IHeroSelectorViewModel
    {
        private readonly IDataLoader dataLoader;
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IMessenger messenger;
        private readonly string playerName = Settings.Default.PlayerName;
        private readonly IReplayRepository replayRepository;
        private DateTime earliestDate;

        private List<GameMode> gameModes = new List<GameMode>
        {
            GameMode.QuickMatch,
            GameMode.HeroLeague,
            GameMode.UnrankedDraft,
            GameMode.TeamLeague
        };

        private List<string> heroes;
        private bool initializing = true;
        private List<string> maps;
        private List<SeasonViewModel> seasons;
        private string selectedHero;
        private string selectedMap = "All";
        private SeasonViewModel selectedSeason;
        private bool showHeroLeague = true;
        private bool showQuickMatches = true;
        private bool showUnranked = true;
        private bool showTeamLeague = true;

        public HeroSelectorViewModel(IMessenger messenger, 
            IReplayRepository replayRepository, 
            IDataLoader dataLoader,
            IDispatcherWrapper dispatcherWrapper)
            : base(messenger)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;
            this.dataLoader = dataLoader;
            this.dispatcherWrapper = dispatcherWrapper;

            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                selectedHero = null;
                FilterReplays();
            });
        }

        public RelayCommand LoadedCommand => new RelayCommand(async () => await Initialize());
        public RelayCommand<string> SelectHeroCommand => new RelayCommand<string>(SelectHero);
        public RelayCommand ReloadDataCommand => new RelayCommand(ReloadData);

        public List<string> Heroes
        {
            get { return heroes; }
            set { Set(() => Heroes, ref heroes, value); }
        }

        public List<string> Maps
        {
            get { return maps; }
            set { Set(() => Maps, ref maps, value); }
        }

        public List<SeasonViewModel> Seasons
        {
            get { return seasons; }
            set { Set(() => Seasons, ref seasons, value); }
        }

        public SeasonViewModel SelectedSeason
        {
            get { return selectedSeason; }
            set
            {
                Set(() => SelectedSeason, ref selectedSeason, value);
                if (initializing) return;
                FilterReplays();
                GetHeroes();
            }
        }

        public string SelectedMap
        {
            get { return selectedMap; }
            set
            {
                Set(() => SelectedMap, ref selectedMap, value);
                if (initializing) return;
                FilterReplays();
                GetHeroes();
            }
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

        public bool ShowTeamLeague
        {
            get { return showTeamLeague; }
            set
            {
                Set(() => ShowTeamLeague, ref showTeamLeague, value);
                ChangeGameMode();
            }
        }

        public void SetEarliestDate()
        {
            var date = DateTime.Now;
            foreach (var replay in replayRepository.GetReplays())
            {
                if (replay.Timestamp < date)
                    date = replay.Timestamp;
            }
            earliestDate = date;
        }

        public void GetMaps()
        {
            var temp = new List<string> {"All"};
            foreach (var replay in replayRepository.GetReplays())
            {
                if (temp.Contains(replay.Map)) continue;
                temp.Add(replay.Map);
            }
            Maps = temp;
        }

        public void RemoveUnplayedSeasons()
        {
            Seasons.RemoveAll(season =>
                !replayRepository.GetReplays().Any(x => x.Timestamp >= season.Start && x.Timestamp <= season.End));
        }

        public async Task Initialize()
        {
            SetEarliestDate();
            Seasons = new List<SeasonViewModel>
            {
                new SeasonViewModel
                {
                    Season = "All",
                    Start = earliestDate,
                    End = DateTime.Now
                },
                new SeasonViewModel
                {
                    Season = "Pre season",
                    Start = earliestDate,
                    End = new DateTime(2016, 06, 13)
                },
                new SeasonViewModel
                {
                    Season = "1",
                    Start = new DateTime(2016, 06, 14),
                    End = new DateTime(2016, 09, 12)
                },
                new SeasonViewModel
                {
                    Season = "2",
                    Start = new DateTime(2016, 09, 13),
                    End = new DateTime(2016, 12, 13)
                },
                new SeasonViewModel
                {
                    Season = "3",
                    Start = new DateTime(2016, 12, 14),
                    End = new DateTime(2017, 03, 15)
                },
                new SeasonViewModel
                {
                    Season = "4",
                    Start = new DateTime(2017, 03, 16),
                    End = new DateTime(2017, 06, 13)
                },
                new SeasonViewModel
                {
                    Season = "5",
                    Start = new DateTime(2017, 06, 14),
                    End = new DateTime(2017, 09, 05)
                },
                new SeasonViewModel
                {
                    Season = "6",
                    Start = new DateTime(2017, 09, 06),
                    End = DateTime.Now
                }
            };
            RemoveUnplayedSeasons();
            SelectedSeason = Seasons.First();
            GetMaps();

            FilterReplays();
            await GetHeroes();
            initializing = false;
        }

        public async void ReloadData()
        {
            await dataLoader.LoadDataAsync();
            GetMaps();
            FilterReplays();
            await GetHeroes();
        }

        public void SelectHero(string hero)
        {
            selectedHero = hero;
            FilterReplays();
            messenger.Send(new HeroSelectedMessage(hero));
        }

        public async Task ChangeGameMode()
        {
            gameModes = new List<GameMode> {GameMode.QuickMatch, GameMode.HeroLeague, GameMode.UnrankedDraft, GameMode.TeamLeague};
            if (!ShowHeroLeague)
                gameModes.Remove(GameMode.HeroLeague);
            if (!ShowQuickMatches)
                gameModes.Remove(GameMode.QuickMatch);
            if (!ShowUnranked)
                gameModes.Remove(GameMode.UnrankedDraft);
            if (!ShowTeamLeague)
                gameModes.Remove(GameMode.TeamLeague);
            FilterReplays();
            await GetHeroes();
        }

        public void FilterReplays()
        {
            var replays = GetFilteredReplays();
            replays = selectedHero != null
                ? replays.Where(
                    x => x.Players.Any(y => y.Character == selectedHero && y.Name.ToLower() == playerName))
                : replays;
            replayRepository.SaveFilteredReplays(replays.ToList());
            messenger.Send(new DataFilterHasBeenAppliedMessage());
        }

        public IEnumerable<Replay> GetFilteredReplays()
        {
            var replays = SelectedSeason.Season == "All"
                ? replayRepository.GetReplays().Where(x => gameModes.Contains(x.GameMode) &&
                                                           x.Timestamp >= earliestDate &&
                                                           x.Players.Any(y => y.Name.ToLower() == playerName))
                : replayRepository.GetReplays()
                    .Where(
                        x =>
                            gameModes.Contains(x.GameMode) &&
                            x.Timestamp >= SelectedSeason.Start &&
                            x.Timestamp <= SelectedSeason.End &&
                            x.Players.Any(y => y.Name.ToLower() == playerName));
            replays = SelectedMap != "All"
                ? replays.Where(x => x.Map == SelectedMap)
                : replays;
            return replays;
        }

        public async Task GetHeroes()
        {
            var replays = GetFilteredReplays();
            var result = new Dictionary<string, int>();
            foreach (var replay in replays)
            {
                var player = replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName);
                if (player == null) continue;
                if (result.ContainsKey(player.Character))
                    result[player.Character]++;
                else
                    result.Add(player.Character, 1);
            }
            await dispatcherWrapper.BeginInvoke(() =>
                Heroes = result.OrderByDescending(x => x.Value).Select(x => x.Key).ToList());
        }
    }

    public interface IHeroSelectorViewModel
    {
        List<string> Heroes { get; set; }
        List<string> Maps { get; set; }
        List<SeasonViewModel> Seasons { get; }
        SeasonViewModel SelectedSeason { get; set; }
        string SelectedMap { get; set; }
        bool ShowHeroLeague { get; set; }
        bool ShowQuickMatches { get; set; }
        bool ShowUnranked { get; set; }
        bool ShowTeamLeague { get; set; }
        RelayCommand<string> SelectHeroCommand { get; }
        RelayCommand ReloadDataCommand { get; }
        RelayCommand LoadedCommand { get; }
    }
}
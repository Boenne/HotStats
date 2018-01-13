using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Heroes.ReplayParser;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Services;
using HotStats.Services.Interfaces;
using HotStats.Wrappers;
using Newtonsoft.Json;

namespace HotStats.ViewModels
{
    public class HeroSelectorViewModel : ViewModelBase, IHeroSelectorViewModel
    {
        private const string All = "All";
        private readonly IDataLoader dataLoader;
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IMessageBoxWrapper messageBoxWrapper;
        private readonly IMessenger messenger;
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
        private List<string> maps;
        private List<SeasonViewModel> seasons;
        private string selectedHero;
        private string selectedMap = All;
        private SeasonViewModel selectedSeason;
        private bool showHeroLeague = true;
        private bool showQuickMatches = true;
        private bool showTeamLeague = true;
        private bool showUnranked = true;

        public HeroSelectorViewModel(IMessenger messenger,
            IReplayRepository replayRepository,
            IDataLoader dataLoader,
            IDispatcherWrapper dispatcherWrapper,
            IMessageBoxWrapper messageBoxWrapper)
            : base(messenger)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;
            this.dataLoader = dataLoader;
            this.dispatcherWrapper = dispatcherWrapper;
            this.messageBoxWrapper = messageBoxWrapper;

            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                selectedHero = null;
                FilterReplays();
            });
        }

        public RelayCommand UpdateDataCommand => new RelayCommand(async () => await UpdateData());
        public RelayCommand LoadedCommand => new RelayCommand(async () => await Initialize());
        public RelayCommand<string> SelectHeroCommand => new RelayCommand<string>(SelectHero);
        public RelayCommand ReloadDataCommand => new RelayCommand(async () => await ReloadData());
        public RelayCommand ChangeGameModeCommand => new RelayCommand(async () => await ChangeGameMode());

        public List<string> Heroes
        {
            get => heroes;
            set { Set(() => Heroes, ref heroes, value); }
        }

        public List<string> Maps
        {
            get => maps;
            set { Set(() => Maps, ref maps, value); }
        }

        public List<SeasonViewModel> Seasons
        {
            get => seasons;
            set { Set(() => Seasons, ref seasons, value); }
        }

        public SeasonViewModel SelectedSeason
        {
            get => selectedSeason;
            set { Set(() => SelectedSeason, ref selectedSeason, value); }
        }

        public string SelectedMap
        {
            get => selectedMap;
            set { Set(() => SelectedMap, ref selectedMap, value); }
        }

        public bool ShowHeroLeague
        {
            get => showHeroLeague;
            set { Set(() => ShowHeroLeague, ref showHeroLeague, value); }
        }

        public bool ShowQuickMatches
        {
            get => showQuickMatches;
            set { Set(() => ShowQuickMatches, ref showQuickMatches, value); }
        }

        public bool ShowUnranked
        {
            get => showUnranked;
            set { Set(() => ShowUnranked, ref showUnranked, value); }
        }

        public bool ShowTeamLeague
        {
            get => showTeamLeague;
            set { Set(() => ShowTeamLeague, ref showTeamLeague, value); }
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

        public async Task UpdateData()
        {
            var filteredReplays = FilterReplays();
            await GetHeroes(filteredReplays);
        }

        public void GetMaps(IEnumerable<Replay> replays)
        {
            var temp = new List<string> {All};
            foreach (var replay in replays)
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
            try
            {
                Seasons = JsonConvert.DeserializeObject<List<SeasonViewModel>>(
                    File.ReadAllText(FilePaths.Seasons));
            }
            catch (Exception)
            {
                Seasons = new List<SeasonViewModel>();
                messageBoxWrapper.Show("Error parsing 'seasons.json'");
            }
            Seasons.ForEach(x =>
            {
                if (!x.Start.HasValue)
                    x.Start = earliestDate;
                if (!x.End.HasValue)
                    x.End = DateTime.Now;
            });
            Seasons.Insert(0, new SeasonViewModel
            {
                Season = All,
                Start = earliestDate,
                End = DateTime.Now
            });

            RemoveUnplayedSeasons();
            SelectedSeason = Seasons.First();

            var filteredReplays = FilterReplays();
            await GetHeroes(filteredReplays);
        }

        public async Task ReloadData()
        {
            await dataLoader.LoadDataAsync();
            var filteredReplays = FilterReplays();
            await GetHeroes(filteredReplays);
        }

        public void SelectHero(string hero)
        {
            selectedHero = hero;
            FilterReplays();
            messenger.Send(new HeroSelectedMessage(hero));
        }

        public async Task ChangeGameMode()
        {
            gameModes = new List<GameMode>
            {
                GameMode.QuickMatch,
                GameMode.HeroLeague,
                GameMode.UnrankedDraft,
                GameMode.TeamLeague
            };
            if (!ShowHeroLeague)
                gameModes.Remove(GameMode.HeroLeague);
            if (!ShowQuickMatches)
                gameModes.Remove(GameMode.QuickMatch);
            if (!ShowUnranked)
                gameModes.Remove(GameMode.UnrankedDraft);
            if (!ShowTeamLeague)
                gameModes.Remove(GameMode.TeamLeague);
            var filteredReplays = FilterReplays();
            await GetHeroes(filteredReplays);
        }

        public IEnumerable<Replay> FilterReplays()
        {
            var replays = GetFilteredReplays(true);
            var filteredReplays = selectedHero != null
                ? replays.Where(
                    x => x.Players.Any(y => y.Character == selectedHero && PlayerName.Matches(y.Name.ToLower()))).ToList()
                : replays;
            
            if (selectedHero != null && !filteredReplays.Any())
                messenger.Send(new HeroDeselectedMessage());
            else
            {
                replayRepository.SaveFilteredReplays(filteredReplays);
                messenger.Send(new DataFilterHasBeenAppliedMessage());
            }
            return replays;
        }

        public List<Replay> GetFilteredReplays(bool getMaps = false)
        {
            var replays = SelectedSeason.Season == All
                ? replayRepository.GetReplays()
                    .Where(x => gameModes.Contains(x.GameMode) &&
                                x.Timestamp >= earliestDate &&
                                x.Players.Any(y => PlayerName.Matches(y.Name.ToLower())))
                    .ToList()
                : replayRepository.GetReplays()
                    .Where(
                        x =>
                            gameModes.Contains(x.GameMode) &&
                            x.Timestamp >= SelectedSeason.Start &&
                            x.Timestamp <= SelectedSeason.End &&
                            x.Players.Any(y => PlayerName.Matches(y.Name.ToLower())))
                    .ToList();

            if (SelectedSeason.Season != All && !replays.Any())
            {
                SelectedSeason.Season = All;
                return GetFilteredReplays(getMaps);
            }

            GetMaps(replays);

            replays = SelectedMap != All
                ? replays.Where(x => x.Map == SelectedMap).ToList()
                : replays;

            return replays;
        }

        public async Task GetHeroes(IEnumerable<Replay> replays)
        {
            var result = new Dictionary<string, int>();
            foreach (var replay in replays)
            {
                var player = replay.Players.FirstOrDefault(x => PlayerName.Matches(x.Name.ToLower()));
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
        RelayCommand ChangeGameModeCommand { get; }
        RelayCommand UpdateDataCommand { get; }
        RelayCommand<string> SelectHeroCommand { get; }
        RelayCommand ReloadDataCommand { get; }
        RelayCommand LoadedCommand { get; }
    }
}
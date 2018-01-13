using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Services;
using HotStats.Services.Interfaces;
using LiveCharts;
using LiveCharts.Wpf;

namespace HotStats.ViewModels.Charts
{
    public interface IDifficultyChartViewModel
    {
        List<Color> Colors { get; set; }
    }

    public class DifficultyChartViewModel : ChartViewModelBase, IDifficultyChartViewModel
    {
        private readonly IHeroDataRepository heroDataRepository;
        private readonly IReplayRepository replayRepository;

        public DifficultyChartViewModel(IReplayRepository replayRepository, IMessenger messenger,
            IHeroDataRepository heroDataRepository) : base(messenger)
        {
            this.replayRepository = replayRepository;
            this.heroDataRepository = heroDataRepository;
            messenger.Register<DataFilterHasBeenAppliedMessage>(this, message =>
            {
                if (message.HeroSelected) return;
                LoadData();
            });
        }

        public List<Color> colors = new List<Color>
        {
            new Color {R = 173, G = 216, B = 230}, //easy
            new Color {R = 255, G = 255, B = 0},
            new Color {R = 0, G = 128, B = 0}, //medium,
            new Color {R = 255, G = 0, B = 0}
        };


        public List<Color> Colors
        {
            get { return colors; }
            set { Set(() => Colors, ref colors, value); }
        }

        public void LoadData()
        {
            var replays = replayRepository.GetFilteredReplays();
            var difficulties = new Dictionary<string, int>();
            var heroesData = heroDataRepository.GetData();
            foreach (var replay in replays)
            {
                var player = replay.Players.First(x => PlayerName.Matches(x.Name.ToLower()));
                var hero = heroesData.FirstOrDefault(x => x.Name == player.Character);
                if (hero?.Difficulty == null) continue;

                if (difficulties.ContainsKey(hero.Difficulty))
                    difficulties[hero.Difficulty]++;
                else
                    difficulties[hero.Difficulty] = 1;
            }

            Clear();
            SeriesCollection.AddRange(difficulties.Select(x =>
                new PieSeries {Values = new ChartValues<int>(new List<int> {x.Value}), Title = x.Key}));
        }
    }
}
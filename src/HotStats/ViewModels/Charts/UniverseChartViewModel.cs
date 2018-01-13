using System.Collections.Generic;
using System.Linq;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Services;
using HotStats.Services.Interfaces;
using LiveCharts;
using LiveCharts.Wpf;

namespace HotStats.ViewModels.Charts
{
    public interface IUniverseChartViewModel
    {
    }

    public class UniverseChartViewModel : ChartViewModelBase, IUniverseChartViewModel
    {
        private readonly IHeroDataRepository heroDataRepository;
        private readonly IReplayRepository replayRepository;

        public UniverseChartViewModel(IReplayRepository replayRepository, IMessenger messenger,
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

        public void LoadData()
        {
            var replays = replayRepository.GetFilteredReplays();
            var universes = new Dictionary<string, int>();
            var heroesData = heroDataRepository.GetData();
            foreach (var replay in replays)
            {
                var player = replay.Players.First(x => PlayerName.Matches(x.Name.ToLower()));
                var hero = heroesData.FirstOrDefault(x => x.Name == player.Character);
                if (hero?.Universe == null) continue;

                if (universes.ContainsKey(hero.Universe))
                    universes[hero.Universe]++;
                else
                    universes[hero.Universe] = 1;
            }

            Clear();
            SeriesCollection.AddRange(universes.Select(x =>
                new PieSeries {Values = new ChartValues<int>(new List<int> {x.Value}), Title = x.Key}));
        }
    }
}
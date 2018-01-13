using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Services;
using HotStats.Services.Interfaces;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;

namespace HotStats.ViewModels.Charts
{
    public interface IRolesChartViewModel
    {
    }

    public class RolesChartViewModel : ChartViewModelBase, IRolesChartViewModel
    {
        private readonly IHeroDataRepository heroDataRepository;
        private readonly IReplayRepository replayRepository;

        public RolesChartViewModel(IReplayRepository replayRepository, IMessenger messenger,
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
            var roles = new Dictionary<string, int>();
            var heroesData = heroDataRepository.GetData();
            foreach (var replay in replays)
            {
                var player = replay.Players.First(x => PlayerName.Matches(x.Name.ToLower()));
                var hero = heroesData.FirstOrDefault(x => x.Name == player.Character);
                if (hero?.Role == null) continue;

                if (roles.ContainsKey(hero.Role))
                    roles[hero.Role]++;
                else
                    roles[hero.Role] = 1;
            }

            Clear();
            SeriesCollection.AddRange(roles.Select(x =>
                new PieSeries {Values = new ChartValues<int>(new List<int> {x.Value}), Title = x.Key}));
        }

        public void LoadData1()
        {
            var replays = replayRepository.GetFilteredReplays();
            if (!replays.Any()) return;
            var roles = new Dictionary<string, Dictionary<int, int>>();
            var heroesData = heroDataRepository.GetData();
            var weeksTaken = new List<int>();
            foreach (var replay in replays)
            {
                var player = replay.Players.First(x => PlayerName.Matches(x.Name.ToLower()));
                var hero = heroesData.FirstOrDefault(x => x.Name == player.Character);
                if (hero == null) continue;

                var week = GetIso8601WeekOfYear(replay.Timestamp);
                if (!weeksTaken.Contains(week))
                    weeksTaken.Add(week);

                if (roles.ContainsKey(hero.Role))
                {
                    if (roles[hero.Role].ContainsKey(week))
                        roles[hero.Role][week]++;
                    else
                        roles[hero.Role].Add(week, 1);
                }
                else
                    roles[hero.Role] = new Dictionary<int, int> {{week, 1}};
            }

            weeksTaken = weeksTaken.OrderBy(x => x).ToList();
            var first = weeksTaken.First();
            var last = weeksTaken.Last();

            var seriesViews = new List<ISeriesView>();
            foreach (var role in roles.Keys)
            {
                var weeks = roles[role].OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                var values = new List<int>();
                for (var i = first; i <= last; i++)
                    values.Add(weeks.ContainsKey(i) ? weeks[i] : 0);
                seriesViews.Add(new ColumnSeries {Title = role, Values = new ChartValues<int>(values)});
            }

            Clear();

            SeriesCollection.AddRange(seriesViews);
            Labels.AddRange(weeksTaken.Select(x => x.ToString()));
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                time = time.AddDays(3);

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }
    }
}
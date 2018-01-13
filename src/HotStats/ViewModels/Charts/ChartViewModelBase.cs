using System.Collections.Generic;
using System.Linq;
using HotStats.Messaging;
using LiveCharts;

namespace HotStats.ViewModels.Charts
{
    public abstract class ChartViewModelBase : ViewModelBase
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public List<string> Labels { get; set; } = new List<string>();

        public void Clear()
        {
            while (SeriesCollection.Any())
                SeriesCollection.RemoveAt(0);

            Labels.Clear();
            SeriesCollection.CurrentSeriesIndex = 0;
        }

        protected ChartViewModelBase(IMessenger messenger) : base(messenger)
        {
        }
    }
}
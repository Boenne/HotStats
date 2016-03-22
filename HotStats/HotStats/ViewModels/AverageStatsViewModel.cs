using HotStats.Messaging;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class AverageStatsViewModel : IAverageStatsViewModel
    {
        private readonly IMessenger messenger;

        public AverageStatsViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
        }
    }
}
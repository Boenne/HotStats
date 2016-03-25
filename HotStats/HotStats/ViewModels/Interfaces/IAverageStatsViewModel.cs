using System.Collections.Generic;

namespace HotStats.ViewModels.Interfaces
{
    public interface IAverageStatsViewModel
    {
        List<AverageViewModel> AverageViewModels { get; set; }

        bool PlayerNameIsSet { get; set; }
    }
}